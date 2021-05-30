using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public abstract class Point {
        private float _distance;
        public float Distance {
            get => _distance;
            set {
                OnDistanceChanged(_distance, value);
                this._distance = value;
                OnAnyChange();
            }
        }

        private float _breadth;
        public float Breadth {
            get => _breadth;
            set {
                OnBreadthChanged(_breadth, value);
                _breadth = value;
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

        public virtual void OnDistanceChanged(float from, float to) { }

        public virtual void OnBreadthChanged(float from, float to) { }

        public virtual void OnAnyChange() { }

        #endregion
    }
}