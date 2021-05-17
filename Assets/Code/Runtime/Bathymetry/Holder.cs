using System;

using JetBrains.Annotations;

using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

namespace Code.Runtime.Bathymetry {
    /// <summary>
    /// Essentially a "Lazy <see cref="UnityEngine.Transform"/>".
    ///
    /// It would be cool to add functionality into this specifically as a "holder", like object pooling, or querying the held objects.
    /// </summary>
    /// <remarks>
    /// There are several ways to access the backing <see cref="UnityEngine.Transform"/>:
    ///
    /// <li><b>Explicitly</b>, via <see cref="Get"/></li>
    /// <li><b>Implicitly</b>, via the getter of <see cref="Instance"/></li>
    /// <li><b>Magically</b>, via implicitly-casting to <see cref="UnityEngine.Transform"/></li>
    ///
    /// </remarks>
    [Serializable]
    public class Holder {
        /// <summary>
        /// The backing field for <see cref="Name"/> <b>when <see cref="_transform"/> does not exist.</b>
        /// </summary>
        private string _name;

        /// <summary>
        /// The <see cref="UnityEngine.Object.name"/> of the <see cref="Holder"/>.
        /// </summary>
        /// <remarks>
        /// If it <see cref="Exists"/>, this will get/set <see cref="_transform"/>.<see cref="UnityEngine.Object.name"/>.
        /// <p/>
        /// Otherwise, this will get/set <see cref="_name"/>.
        /// </remarks>
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

        /// <summary>
        /// The actual <see cref="UnityEngine.Transform"/> instance that the <see cref="Holder"/> is managing.
        /// </summary>
        private Transform _transform;

        /// <summary>
        /// The lazily-<see cref="Instantiate"/>-ed <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Instance {
            get {
                if (!_transform) {
                    _transform = Instantiate();
                }

                return _transform;
            }
        }

        /// <summary>
        /// An optional parent <see cref="Holder"/>.
        /// </summary>
        /// <remarks>
        /// This must be set when the holder is instantiated, to prevent it from getting messy down the line.
        /// <br/>
        /// However, the parent holder <b>does not need to <see cref="Exists">exist></see> yet.</b>
        /// <br/>
        /// The <see cref="Parent"/> as necessary when <see cref="Instantiate"/> is called.
        /// <p/>
        /// This doesn't have a setter, but the value can still be set in a constructor.
        /// </remarks>
        [CanBeNull]
        public Holder Parent { get; }

        public bool Exists => (bool) _transform;

        public Holder(Holder parent) : this(default, parent) { }

        public Holder(string name = default, Holder parent = default) {
            this.Name   = name ?? GUID.Generate().ToString();
            this.Parent = parent;
        }

        public static implicit operator Transform(Holder holder) {
            return holder.Get();
        }

        /// <summary>
        /// The most explicit way to get the backing <see cref="UnityEngine.Transform"/>.
        /// </summary>
        /// <returns></returns>
        public Transform Get() {
            return Instance;
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
