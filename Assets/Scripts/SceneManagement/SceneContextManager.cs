using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityIoC.SceneManagement
{
    /// <summary>
    /// Manages scene-specific dependencies and objects in Unity projects.
    /// Provides functionality for handling the root Transform for the scene and
    /// for dynamically creating objects in the scene hierarchy.
    /// Uses a singleton pattern to ensure a single instance per application.
    /// </summary>
    public class SceneContextManager : MonoBehaviour
    {
        /// <summary>
        /// Gets the singleton instance of the SceneContextManager.
        /// </summary>
        public static SceneContextManager Instance { get; private set; }

        /// <summary>
        /// Gets the root transform for the current scene.
        /// All dynamically created objects in the scene are parented to this transform.
        /// </summary>
        public Transform SceneRoot { get; private set; }

        private void Awake()
        {
            // Implement singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            // Subscribe to Unity's scene lifecycle events
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            // Unsubscribe from Unity's scene lifecycle events
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        /// <summary>
        /// Called when a scene is loaded. Creates the SceneRoot for the new scene.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The scene load mode.</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneRoot = new GameObject("SceneRoot").transform;
            Debug.Log($"Scene {scene.name} loaded. SceneRoot created.");
        }

        /// <summary>
        /// Called when a scene is unloaded. Destroys the SceneRoot.
        /// </summary>
        /// <param name="scene">The unloaded scene.</param>
        private void OnSceneUnloaded(Scene scene)
        {
            if (SceneRoot != null)
            {
                Destroy(SceneRoot.gameObject);
            }
            SceneRoot = null;
            Debug.Log($"Scene {scene.name} unloaded. SceneRoot destroyed.");
        }

        /// <summary>
        /// Creates an object in the scene as a child of the SceneRoot.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <returns>The instantiated GameObject, or null if creation failed.</returns>
        public GameObject CreateObjectInScene(GameObject prefab)
        {
            if (SceneRoot == null)
            {
                Debug.LogError("SceneRoot is not initialized. Cannot create object.");
                return null;
            }

            if (prefab == null)
            {
                Debug.LogError("Prefab is null. Cannot create object.");
                return null;
            }

            return Instantiate(prefab, SceneRoot);
        }
    }
}
