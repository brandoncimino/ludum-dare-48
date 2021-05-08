using BrandonUtils.Vectors;

using UnityEngine;

namespace Code.Runtime {
    public class Shadow : MonoBehaviour {
        private Transform Subject;
        public  float     MimicPositionRate;
        public  float     MimicRotationRate;

        private void Start() {
            Subject = new GameObject($"{name}_subject").transform;
            var tf  = transform;
            var stf = Subject.transform;
            stf.parent   = tf.parent;
            stf.position = tf.position;
            stf.rotation = tf.rotation;
            tf.parent    = null;
        }

        // Update is called once per frame
        void Update() {
            // Lerp position
            Transform tf = transform;
            tf.position = tf.position.Lerp(Subject.position, Time.deltaTime * MimicPositionRate);
            tf.rotation = Quaternion.Slerp(tf.rotation, Subject.rotation, Time.deltaTime * MimicRotationRate);
        }
    }
}
