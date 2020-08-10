using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Artcom.OpenSoundControl.Interfaces;
using Artcom.OpenSoundControl.Library;
using Artcom.OpenSoundControl.Scripts;
using UnityEditor;
using UnityEngine;

namespace Artcom.OpenSoundControl.Editor {
	internal enum PlayModeState { Play, Pause, Stop }

	public class OscObserverWindow : EditorWindow, IOscAdapter {
		[SerializeField] private PlayModeState playModeState;
		[SerializeField] private string[] messageBuffer;
		[SerializeField] private int indexer;
		private uint messageCounter;
		private int repaintCounter;
		private bool needsRepainting;
		private StringBuilder stringBuilder;
		[SerializeField] private List<OscReceiver> observedReceivers;
		
#region Unity Runtime + Bindings
		[MenuItem("Window/OSC Monitor")] 
		public static void ShowWindow() {
			var window = GetWindow<OscObserverWindow>();
			window.minSize = new Vector2(300, 300);
			window.titleContent = new GUIContent("OSC Monitor", Resources.Load<Texture>("monitor"));
		}

		private void Awake() {
			stringBuilder = new StringBuilder();
			observedReceivers = new List<OscReceiver>();
			stringBuilder = new StringBuilder();
			indexer = 0;
			messageBuffer = new string[32];
			ClearMessageBuffer();
			FindCurrentPlayModeState();
		}

		private void OnDestroy() {
			DetachEditorWindow();
		}

		private void OnGUI() {
			switch(playModeState) {
				case PlayModeState.Stop:
					DoDisabledEditor();
					break;
				default:
					DoPlayingEditor();
					break;
			}
		}

		private void Update() {
			needsRepainting |= messageCounter <= 0;
			// we can reset the message counter, we just don't want to run in the unlikely scenario of overflowing it
			messageCounter = 0;
			repaintCounter -= 1;
			// Schedule a repainting if it is really necessary, but just not too often
			if(repaintCounter < 0 && needsRepainting) {
				Repaint();
				repaintCounter = 3;
			}

			var newState = FindCurrentPlayModeState();
			// we went from playing to a stopped state
			if(playModeState == PlayModeState.Play && newState == PlayModeState.Stop) {
				needsRepainting = true;
				ClearMessageBuffer();
				observedReceivers = new List<OscReceiver>();
			}

			// we went from a stopped state to playing
			if(playModeState == PlayModeState.Stop && newState == PlayModeState.Play) {
				needsRepainting = true;
				ClearMessageBuffer();
				observedReceivers = new List<OscReceiver>();
			}

			playModeState = newState;
			// finally attach yourself to all new listeners
			if(playModeState != PlayModeState.Stop) {
				AttachEditorWindow();
			}
		}
#endregion
#region Unity UI Operations
		private void DoDisabledEditor() {
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.LabelField("You're currently no in a playing session, this inspector will not enable " +
			                           "itself without a scene being played.", EditorStyles.wordWrappedLabel);
			EditorGUILayout.Space();
			EditorGUILayout.EndVertical();	
		}

		private void DoPlayingEditor() {
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.LabelField(string.Format("Monitoring currently {0} receivers...", observedReceivers.Count));
			EditorGUILayout.EndVertical();
			EditorGUILayout.LabelField("Message Buffer: ");
			EditorGUILayout.BeginVertical("Box");
			for(var i = 0; i < messageBuffer.Length; i++) {
				EditorGUILayout.LabelField(messageBuffer[(i + indexer) % messageBuffer.Length]);
			}
			EditorGUILayout.EndVertical();
		}
#endregion
#region Helpers
		/// <summary>
		/// Set all values of the ringbuffer to string.Empty to have a constant speed (and easier handling) on the
		/// StringBuilding and displaying of the UI.
		/// </summary>
		private void ClearMessageBuffer() {
			for(var i = 0; i < messageBuffer.Length; i++) {
				messageBuffer[i] = string.Empty;
			}
		}
		
		private static PlayModeState FindCurrentPlayModeState() {
			if(EditorApplication.isPaused) {
				return PlayModeState.Pause;
			}
			if(Application.isPlaying) {
				return PlayModeState.Play;
			}
			return PlayModeState.Stop;
		}

		/// <summary>
		/// Detach from all receivers
		/// </summary>
		private void DetachEditorWindow() {
			foreach(var receiver in observedReceivers) {
				receiver.RemoveReader(this);
			}
		}

		/// <summary>
		/// Attach to all receivers
		/// </summary>
		private void AttachEditorWindow() {
			var receivers = OscReceiverManager.Instance.receivers;
			foreach(var receiver in receivers) {
				if(observedReceivers.Contains(receiver)) {
					continue;
				}
				
				receiver.RegisterReader(this);
				observedReceivers.Add(receiver);
			}
		}
		
#endregion
#region IOSCReader Implementation	
		public bool IsMatch(string address) {
			return true;
		}

		public bool MatchAndProcess(OscMessage message, string url, int port) {
			// We will always silently listen, but we spoof that we haven't matched - just to not throw off any runtime
			// processes. 
			Process(message, url, port);
			return false;
		}

		public void Process(OscMessage message, string url, int port) {
			if(stringBuilder == null) {
				stringBuilder = new StringBuilder();
			}
			// 'url:port: [' -> '127.0.0.1:8000: ['
			stringBuilder.AppendFormat("{0}:{1}", url, port).Append(message.Address).Append(": [");
			var oldLength = stringBuilder.Length;
			foreach(var element in message.Arguments) {
				// 'value, ' -> '0.02485, '
				stringBuilder.AppendFormat("{0}, ", element);
			}
			if(stringBuilder.Length != oldLength) {
				// remove the ', ' characters from the iteration
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
			}
			stringBuilder.Append("]");
			// write it into the message buffer
			messageBuffer[indexer] = stringBuilder.ToString();
			// reset the string builder
			stringBuilder.Length = 0;
			// safely increment the indexer
			indexer = (indexer + 1) % messageBuffer.Length;
			messageCounter += 1;
		}
#endregion
	}
}