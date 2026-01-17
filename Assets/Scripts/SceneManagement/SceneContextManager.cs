using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityIoC.SceneManagement
{
    /// <summary>
    /// Manages scene-specific dependencies and objects in Unity projects.
    /// Provides functionality for handling the root Transform for the scene and
    /// for dynamically creating objects in the scene hierarchy.
    /// Designed to be registered as a singleton in the IoC container.
    /// </summary>
    public class SceneContextManager : ISceneContextManager, IDisposable
    {
        private const string SceneRootName = "SceneRoot";

        /// <summary>
        /// Gets the root transform for the current scene.
        /// All dynamically created objects in the scene are parented to this transform.
        /// </summary>
        public Transform SceneRoot { get; private set; }

        /// <summary>
        /// Tracks the scene that owns the current SceneRoot.
        /// </summary>
        private Scene _currentScene;

        /// <summary>
        /// Tracks whether the SceneContextManager has been initialized.
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Initializes the SceneContextManager.
        /// Should be called once during application startup.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("SceneContextManager is already initialized.");
                return;
            }

            _isInitialized = true;
            
            // Subscribe to Unity's scene lifecycle events for cleanup only
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            
            Debug.Log("SceneContextManager initialized.");
        }

        /// <summary>
        /// Disposes the SceneContextManager and unsubscribes from events.
        /// </summary>
        public void Dispose()
        {
            if (!_isInitialized)
            {
                return;
            }

            // Unsubscribe from Unity's scene lifecycle events
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            // Clean up the SceneRoot if it exists
            if (SceneRoot != null)
            {
                UnityEngine.Object.Destroy(SceneRoot.gameObject);
                SceneRoot = null;
            }

            _isInitialized = false;
            Debug.Log("SceneContextManager disposed.");
        }

        /// <summary>
        /// Begins a new scene by creating a SceneRoot for it.
        /// This should be called explicitly when starting a new scene to ensure
        /// the SceneRoot is ready before any objects are instantiated.
        /// </summary>
        public void BeginNewScene()
        {
            if (!_isInitialized)
            {
                Debug.LogError("SceneContextManager is not initialized. Call Initialize() first.");
                return;
            }

            // Clean up existing SceneRoot if present
            if (SceneRoot != null)
            {
                UnityEngine.Object.Destroy(SceneRoot.gameObject);
                SceneRoot = null;
            }

            // Get the active scene
            Scene activeScene = SceneManager.GetActiveScene();
            
            // Create a new SceneRoot GameObject
            GameObject sceneRootObject = new GameObject(SceneRootName);
            SceneRoot = sceneRootObject.transform;
            
            // Move the SceneRoot to the active scene
            SceneManager.MoveGameObjectToScene(sceneRootObject, activeScene);
            _currentScene = activeScene;
            
            Debug.Log($"SceneRoot created for scene '{activeScene.name}'.");
        }

        /// <summary>
        /// Loads a scene and automatically sets up the SceneRoot for it.
        /// This is a wrapper around Unity's SceneManager.LoadScene that ensures
        /// proper scene context initialization.
        /// Note: For better control, consider using LoadSceneAsync instead.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="mode">The scene load mode (Single or Additive).</param>
        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (!_isInitialized)
            {
                Debug.LogError("SceneContextManager is not initialized. Call Initialize() first.");
                return;
            }

            // For additive loading, keep the existing SceneRoot
            if (mode == LoadSceneMode.Additive && SceneRoot != null)
            {
                Debug.Log($"Loading scene '{sceneName}' additively. Keeping existing SceneRoot.");
                SceneManager.LoadScene(sceneName, mode);
                return;
            }

            // Subscribe to sceneLoaded event temporarily for this load operation
            void OnSceneLoadedHandler(Scene scene, LoadSceneMode loadMode)
            {
                // Unsubscribe immediately to prevent multiple calls
                SceneManager.sceneLoaded -= OnSceneLoadedHandler;
                
                // Create SceneRoot for the newly loaded scene
                BeginNewScene();
            }
            
            SceneManager.sceneLoaded += OnSceneLoadedHandler;
            
            // Load the scene
            SceneManager.LoadScene(sceneName, mode);
        }

        /// <summary>
        /// Loads a scene asynchronously and automatically sets up the SceneRoot for it.
        /// This is a wrapper around Unity's SceneManager.LoadSceneAsync that ensures
        /// proper scene context initialization.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="mode">The scene load mode (Single or Additive).</param>
        /// <returns>An AsyncOperation that can be used to track the loading progress.</returns>
        public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (!_isInitialized)
            {
                Debug.LogError("SceneContextManager is not initialized. Call Initialize() first.");
                return null;
            }

            // For additive loading, keep the existing SceneRoot
            if (mode == LoadSceneMode.Additive && SceneRoot != null)
            {
                Debug.Log($"Loading scene '{sceneName}' additively. Keeping existing SceneRoot.");
                return SceneManager.LoadSceneAsync(sceneName, mode);
            }

            // Start loading the scene asynchronously
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            
            // Subscribe to completion to create SceneRoot
            // Use a local callback that unsubscribes itself to prevent memory leaks
            void OnCompleted(AsyncOperation op)
            {
                op.completed -= OnCompleted;
                BeginNewScene();
            }
            
            asyncOperation.completed += OnCompleted;
            
            return asyncOperation;
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
                UnityEngine.Object.Destroy(SceneRoot.gameObject);
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

            return UnityEngine.Object.Instantiate(prefab, SceneRoot);
        }
    }
}
