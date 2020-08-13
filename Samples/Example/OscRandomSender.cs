using Artcom.OpenSoundControl.Components;
using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Example {
    public class OscRandomSender : MonoBehaviour {
        [SerializeField] private OscSender sender;
        
        private void Reset() {
            sender = GetComponent<OscSender>();
        }

        private void Update() {
            if (sender == null) {
                return;
            }
            
            sender.Send(new OscMessage("/somewhere", Random.value));
        }
    }
}