using UnityEngine;

namespace Artcom.OpenSoundControl.Example.Cursor {
    public class CursorFollower : MonoBehaviour {
        [SerializeField] private Transform target;
        internal Vector3 velocity;

        private void Update() {
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, 0.05f);
        }
    }
}