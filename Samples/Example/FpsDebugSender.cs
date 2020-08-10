using Artcom.OpenSoundControl.Library;
using Artcom.OpenSoundControl.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace {
    public class FpsDebugSender : MonoBehaviour {
        [SerializeField] private OscLatcher latcher;
        [SerializeField] private Text text;
        
        private void Update() {
            text.text = $"{1f / Time.deltaTime:###.##} fps";
            latcher.Send(new OscMessage("/debug/fps", 1f / Time.deltaTime));
        }
    }
}