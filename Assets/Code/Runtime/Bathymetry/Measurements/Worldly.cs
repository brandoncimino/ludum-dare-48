using UnityEngine;

namespace Code.Runtime.Bathymetry.Measurements {
    public class WorldPoint : Spacey.IWorldly {
        public Vector3 Worldly { get; set; }

        public static implicit operator WorldPoint(Vector3 vector3) {
            return new WorldPoint() {
                Worldly = vector3
            };
        }
    }
}
