using Code.Runtime.Bathymetry.Measurements;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public abstract class Point : IPoint {
        private float _distance;
        public float Distance {
            get => _distance;
            set {
                var old = _distance;
                this._distance = value;
                OnDistanceChanged(old, value);
                OnAnyChange();
            }
        }

        private float _breadth;
        public float Breadth {
            get => _breadth;
            set {
                var old = _breadth;
                _breadth = value;
                OnBreadthChanged(old, value);
                OnAnyChange();
            }
        }
        public abstract DimensionSpace DimensionSpace { get; }

        public virtual Vector3 ToWorldAxes() {
            return new Vector3(
                Breadth,
                0,
                Distance
            );
        }

        #region "Events"

        protected virtual void OnDistanceChanged(float from, float to) { }

        protected virtual void OnBreadthChanged(float from, float to) { }

        protected virtual void OnAnyChange() { }

        #endregion
    }
}