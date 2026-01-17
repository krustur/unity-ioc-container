using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityIoC.SceneManagement
{
    /// <summary>
    /// Interface for managing scene-specific dependencies and objects in Unity projects.
    /// </summary>
    public interface ISceneContextManager
    {
        /// <summary>
        /// Gets the root transform for the current scene.
        /// All dynamically created objects in the scene are parented to this transform.
        /// </summary>
        Transform SceneRoot { get; }

        /// <summary>
        /// Initializes the SceneContextManager.
        /// Should be called once during application startup.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Begins a new scene by creating a SceneRoot for it.
        /// This should be called explicitly when starting a new scene to ensure
        /// the SceneRoot is ready before any objects are instantiated.
        /// </summary>
        void BeginNewScene();

        /// <summary>
        /// Loads a scene and automatically sets up the SceneRoot for it.
        /// This is a wrapper around Unity's SceneManager.LoadScene that ensures
        /// proper scene context initialization.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="mode">The scene load mode (Single or Additive).</param>
        void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single);

        /// <summary>
        /// Loads a scene asynchronously and automatically sets up the SceneRoot for it.
        /// This is a wrapper around Unity's SceneManager.LoadSceneAsync that ensures
        /// proper scene context initialization.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <param name="mode">The scene load mode (Single or Additive).</param>
        /// <returns>An AsyncOperation that can be used to track the loading progress.</returns>
        AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single);

        /// <summary>
        /// Creates an object in the scene as a child of the SceneRoot.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <returns>The instantiated GameObject, or null if creation failed.</returns>
        GameObject CreateObjectInScene(GameObject prefab);
    }
}
