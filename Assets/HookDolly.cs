using UnityEngine;

public class HookDolly : MonoBehaviour {
    private Vector3 RelativePosition;

    public Transform Hook;

    // Start is called before the first frame update
    void Start() {
        RelativePosition = transform.position - Hook.position;
    }

    // Update is called once per frame
    void Update() {
        var transform1 = transform;
        var newPos     = transform1.position;
        newPos.y            = Hook.position.y + RelativePosition.y;
        transform1.position = newPos;
    }
}
