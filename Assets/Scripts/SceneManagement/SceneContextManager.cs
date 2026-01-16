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
        private const string SceneRootName = "SceneRoot";
        
        /// <summary>
        /// Gets the singleton instance of the SceneContextManager.
        /// </summary>
        public static SceneContextManager Instance { get; private set; }

        /// <summary>
        /// Gets the root transform for the current scene.
        /// All dynamically created objects in the scene are parented to this transform.
        /// </summary>
        public Transform SceneRoot { get; private set; }

        /// <summary>
        /// Tracks the scene that owns the current SceneRoot.
        /// </summary>
        private Scene _currentScene;

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
        /// For single scene loads, replaces the existing SceneRoot.
        /// For additive loads, keeps the existing SceneRoot to avoid orphaning objects.
        /// </summary>
        /// <param name="scene">The loaded scene.</param>
        /// <param name="mode">The scene load mode.</param>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // For additive loading, keep the existing SceneRoot to avoid orphaning objects
            if (mode == LoadSceneMode.Additive && SceneRoot != null)
            {
                Debug.Log($"Scene {scene.name} loaded additively. Keeping existing SceneRoot.");
                return;
            }

            // Create a new SceneRoot GameObject
            GameObject sceneRootObject = new GameObject(SceneRootName);
            SceneRoot = sceneRootObject.transform;
            
            // Move the SceneRoot to the loaded scene
            SceneManager.MoveGameObjectToScene(sceneRootObject, scene);
            _currentScene = scene;
            
            Debug.Log($"Scene {scene.name} loaded. SceneRoot created and moved to scene.");
        }

        /// <summary>
        /// Called when a scene is unloaded. Destroys the SceneRoot only if it belongs to the unloaded scene.
        /// This prevents destroying the SceneRoot when other scenes are unloaded in multi-scene setups.
        /// </summary>
        /// <param name="scene">The unloaded scene.</param>
        private void OnSceneUnloaded(Scene scene)
        {
            // Only destroy the SceneRoot if it belongs to the scene being unloaded
            if (SceneRoot != null && _currentScene.IsValid() && _currentScene == scene)
            {
                Destroy(SceneRoot.gameObject);
                SceneRoot = null;
                _currentScene = default;
                Debug.Log($"Scene {scene.name} unloaded. SceneRoot destroyed.");
            }
            else
            {
                Debug.Log($"Scene {scene.name} unloaded. SceneRoot belongs to a different scene.");
            }
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
