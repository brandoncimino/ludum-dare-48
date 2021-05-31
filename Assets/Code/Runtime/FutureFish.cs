using System;
using System.Collections.Generic;

using Code.Runtime.Bathymetry.Measurements;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Code.Runtime {
    public class FutureFish : Spacey.IWorldly, IComparable<FutureFish> {
        /// A much simpler and probably smarter implementation of <see cref="Code.Runtime.Bathymetry.Holder"/>
        public static readonly Lazy<Transform> FishHolder = new Lazy<Transform>(() => new GameObject(nameof(FishHolder)).transform);
        public static readonly SortedSet<FutureFish> Portents = new SortedSet<FutureFish>(Comparer<FutureFish>.Default);

        public Catchables      Catchables         { get; }
        public Spacey.IWorldly Position           { get; }
        public float           DistanceFromGround { get; }
        public Quaternion      Rotation           { get; }
        public float           Scale              { get; }
        public bool            Instantiated       { get; private set; }
        public bool            Reified            => Instantiated;
        public bool            Activated          { get; private set; }
        public bool            Consummated        => Activated;
        public Catchables      Instance           { get; private set; }

        /// <summary>
        /// Predicts the conditions under which a <see cref="FutureFish"/> will be <see cref="Reified"/>,
        /// i.e., transition from a theoretical <see cref="Catchables"/> to an actual <see cref="Catchables"/> instance.
        /// </summary>
        public Func<FutureFish, bool> Prescience => InstantiateWhen;

        /// <summary>
        /// An idiomatic, less-onanistic alias for <see cref="Prescience"/>.
        /// </summary>
        public readonly Func<FutureFish, bool> InstantiateWhen;

        /// <summary>
        /// Predicts the conditions under which a <see cref="FutureFish"/> will be <see cref="Consummated"/>,
        /// i.e., the <see cref="Catchables"/> instance will abraid.
        /// </summary>
        public Func<FutureFish, bool> Providence => ActivateWhen;

        /// <summary>
        /// An idiomatic, less-onanistic alias for <see cref="Providence"/>.
        /// </summary>
        public readonly Func<FutureFish, bool> ActivateWhen;

        public FutureFish(
            Catchables catchables,
            Spacey.IWorldly position,
            float distanceFromGround,
            Quaternion rotation,
            float scale
        ) {
            Catchables         = catchables;
            Position           = position;
            DistanceFromGround = distanceFromGround;
            Rotation           = rotation;
            Scale              = scale;

            Portents.Add(this);
        }

        public bool Reify() => Instantiate();

        public bool Instantiate() {
            if (Instantiated) {
                return false;
            }

            Catchables.gameObject.SetActive(false);

            Instance = InstantiateFish(
                Catchables,
                Position,
                DistanceFromGround,
                Rotation,
                Scale
            );

            Instantiated = true;
            return true;
        }

        public bool Activate() {
            Instantiate();

            if (Activated) {
                return false;
            }

            Instance.gameObject.SetActive(true);
            Activated = true;
            return true;
        }

        public bool Deactivate() {
            if (!Activated || !Instantiated) {
                return false;
            }

            Instance.gameObject.SetActive(false);
            Activated = false;
            return true;
        }

        public void Destroy() {
            Object.Destroy(Instance.gameObject);
            Portents.Remove(this);
        }

        /// <summary>
        /// "Instantiates" a <see cref="Runtime.Catchables"/> prefab.
        ///
        /// This method should not contain any "decision making", and should only handle the "engine-y" stuff.
        /// Decision making should go in <see cref="SpawnFish"/> instead.
        /// </summary>
        /// <param name="fishToInstantiate"></param>
        /// <param name="position"></param>
        /// <param name="distanceFromGround"></param>
        /// <param name="rotation"></param>
        /// <param name="fishScale"></param>
        /// <returns></returns>
        private static Catchables InstantiateFish(Catchables fishToInstantiate, Spacey.IWorldly position, float distanceFromGround, Quaternion rotation, float fishScale) {
            var newFish = Object.Instantiate(
                fishToInstantiate,
                position.ToWorldly() + (Vector3.up * distanceFromGround),
                rotation,
                FishHolder.Value
            );
            newFish.ScaleUp(fishScale);
            return newFish;
        }

        public Vector3 ToWorldly() {
            return Position.ToWorldly();
        }

        public int CompareTo(FutureFish other) {
            return Position.ToWorldly().z.CompareTo(other.ToWorldly().z);
        }
    }
}
