using UnityEngine;

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
        /// Creates an object in the scene as a child of the SceneRoot.
        /// </summary>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <returns>The instantiated GameObject, or null if creation failed.</returns>
        GameObject CreateObjectInScene(GameObject prefab);
    }
}
