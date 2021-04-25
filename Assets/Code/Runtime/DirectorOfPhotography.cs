using BrandonUtils.Cinematography;

using UnityEngine;

public class DirectorOfPhotography : MonoBehaviour {
    public DollyCrew DollyCrew;
    public Cameraman Cameraman;
    public Transform Hook;
    public float     FollowDistance;

    // Start is called before the first frame update
    void Awake() { }

    // Update is called once per frame
    void Update() {
        var transform1 = DollyCrew.transform;
        var newPos     = transform1.position;
        newPos.y            = Hook.position.y + FollowDistance;
        transform1.position = newPos;
    }
}
