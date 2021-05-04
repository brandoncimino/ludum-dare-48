using UnityEngine;

namespace Code.Runtime {
    public class Treeish : MonoBehaviour {
        private void Start() {
            transform.localScale *= Random.Range(0.9f, 1.1f);
        }
    }
}
