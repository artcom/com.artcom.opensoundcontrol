using System.Collections.Generic;
using UnityEngine;

namespace Artcom.OpenSoundControl.Scripts {
    /// <summary>
    /// Simple singleton to keep track of all receivers
    /// </summary>
    public class OscReceiverManager : MonoBehaviour {
        public List<OscReceiver> receivers;

        public void Add(OscReceiver component) {
            if (receivers.Contains(component)) {
                return;
            }

            receivers.Add(component);
        }

        public void Remove(OscReceiver component) {
            if (receivers.Contains(component)) {
                receivers.Add(component);
            }
        }


        private static OscReceiverManager instance = null;
        private static bool isInitDone = false;
        private static bool isInitInProgress = false;
        private static bool wasDestroyed = false;

        /// <summary>
        /// [GET] Accesses the singleton instance.
        /// 
        /// If none exists yet, a new instance will be created 
        /// on-demand and used globally across all scenes.
        /// </summary>
        public static OscReceiverManager Instance {
            get {
                if (isInitDone) {
                    return instance;
                }

                if (isInitInProgress) {
                    Debug.LogWarning(
                        $"Singleton \'{nameof(OscReceiverManager)}\' is accessed while in the process of being initialized due to a pending access. This suggests that there is a circular dependency where the creation of the singleton requires an instance of it to be present already. Returning null instead.");
                    return null;
                }

                // Theoretically, the instance's constructor or Awake script can cause someone
                // to access the Instance property before we get to assign the newly
                // created instance back to the field, leading to an endless loop.
                // To fix this, we'll set a flag checking for a creation in progress
                // and refuse to create another instance in the process.
                isInitInProgress = true;
                InitSingleton();
                isInitInProgress = false;
                isInitDone = true;

                return instance;
            }
        }

        /// <summary>
        /// [GET] Returns whether a singleton instance is currently available.
        /// </summary>
        public static bool IsAvailable {
            get { return instance != null; }
        }

        private static void InitSingleton() {
            Debug.Log($"Preparing singleton access for '{nameof(OscReceiverManager)}'...");

            // Retrieve an existing instance. This is only necessary when
            // we access an existing singleton before its own Awake script was run,
            // in which it would register itself.
            if (instance == null) {
                instance = FindExistingInstance();
            }

            // Create an instance on-demand if none is existing.
            if (instance != null) {
                return;
            }

            if (wasDestroyed) {
                Debug.LogWarning($"Singleton '{nameof(OscReceiverManager)}' is accessed after destruction. Not creating a new instance.");
            } else {
                CreateInstance();
            }
        }

        private static void CreateInstance() {
            var hostObj = new GameObject {
                name = $"[Non-Persistent] {typeof(OscReceiverManager)}", hideFlags = HideFlags.DontSaveInEditor
            };
            hostObj.AddComponent<OscReceiverManager>();

            // When a singleton instance is created on-demand, we assume that
            // it's supposed to be independent from any specific scene.
            DontDestroyOnLoad(hostObj);
        }

        private static OscReceiverManager FindExistingInstance() {
            var existingInstances = FindObjectsOfType<OscReceiverManager>();
            if (existingInstances.Length > 1) {
                Debug.LogError($"There is more than one instance of singleton {nameof(OscReceiverManager)} present in the current scene. " + "This should never happen.");
            }

            return existingInstances.Length > 0 ? existingInstances[0] : null;
        }

        /// <summary>
        /// Unity event to handle internal singleton initialization.
        /// When deriving, call the base implementation as usual.
        /// </summary>
        private void Awake() {
            if (instance != null && instance != this) {
                Debug.LogError($"Singleton {nameof(OscReceiverManager)} instance '{name}' is waking up, but instance '{instance.name}' is already active.");
            } else {
                Debug.LogFormat($"Singleton {nameof(OscReceiverManager)} instance '{name}' is waking up.");
                instance = this;
                isInitDone = false;
            }

            receivers = new List<OscReceiver>();
        }

        /// <summary>
        /// Unity event to handle internal singleton shutdown.
        /// When deriving, call the base implementation as usual.
        /// </summary>
        private void OnDestroy() {
            Debug.Log($"Singleton {nameof(OscReceiverManager)} instance '{name}' is being destroyed.");

            // When Unity quits, it destroys objects in a random order.
            // In the case on on-demand created (global, DontDestroyOnLoad) singletons,
            // the only way they are destroyed is when the application shuts down, so
            // we won't create any on-demand instances after a destruction was detected.
            wasDestroyed = true;
        }
    }
}