using UnityEngine;

namespace Artcom.OpenSoundControl.Example.Cursor {
    public class CursorMover : MonoBehaviour {

        internal RectTransform parentTransform;
        internal RectTransform rectTransform;
        
        private void Start() {
            parentTransform = (RectTransform) transform.parent;
            rectTransform = (RectTransform) transform;
        }

        public void OnNewPosition(Vector2 position) {
            rectTransform.anchoredPosition = position * parentTransform.sizeDelta - parentTransform.sizeDelta / 2;
        }
    }
}