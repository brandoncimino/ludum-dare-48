using UnityEditor;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public class Holder {
        private string _name;
        public string Name {
            get {
                if (_transform) {
                    _name = _transform.name;
                }

                return _name;
            }

            set {
                _name = value;

                if (_transform) {
                    _transform.name = _name;
                }
            }
        }
        private Transform _transform;

        public Transform Transform {
            get {
                if (!_transform) {
                    _transform = Instantiate();
                }

                return _transform;
            }
        }

        public Holder Parent;

        public bool Exists => (bool) _transform;

        public Holder(Holder parent) : this(default, parent) { }

        public Holder(string name = default, Holder parent = default) {
            this.Name   = name ?? GUID.Generate().ToString();
            this.Parent = parent;
        }

        public static implicit operator Transform(Holder holder) {
            return holder.Get();
        }

        public Transform Get() {
            return Transform;
        }

        public void Clear() {
            if (_transform) {
                Object.DestroyImmediate(_transform.gameObject);
            }
        }

        private Transform Instantiate() {
            var tf = new GameObject(Name).transform;

            if (Parent != null) {
                tf.SetParent(Parent);
            }

            return tf;
        }
    }
}
