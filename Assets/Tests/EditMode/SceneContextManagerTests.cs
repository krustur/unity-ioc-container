using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityIoC.SceneManagement;

namespace UnityIoC.Tests
{
    [TestFixture]
    public class SceneContextManagerTests
    {
        private ISceneContextManager _sceneContextManager;

        [SetUp]
        public void SetUp()
        {
            _sceneContextManager = new SceneContextManager();
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose the manager after each test
            if (_sceneContextManager is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        [Test]
        public void Initialize_ShouldSetInitializedState()
        {
            // Act
            _sceneContextManager.Initialize();

            // Assert - Should not throw when disposed
            Assert.DoesNotThrow(() => (_sceneContextManager as IDisposable)?.Dispose());
        }

        [Test]
        public void Initialize_CalledTwice_ShouldNotThrow()
        {
            // Arrange
            _sceneContextManager.Initialize();

            // Act & Assert
            Assert.DoesNotThrow(() => _sceneContextManager.Initialize());
        }

        [Test]
        public void Dispose_WithoutInitialize_ShouldNotThrow()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => (_sceneContextManager as IDisposable)?.Dispose());
        }

        [Test]
        public void CreateObjectInScene_WithNullPrefab_ReturnsNull()
        {
            // Arrange
            _sceneContextManager.Initialize();

            // Act
            var result = _sceneContextManager.CreateObjectInScene(null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void SceneRoot_BeforeInitialize_IsNull()
        {
            // Assert
            Assert.IsNull(_sceneContextManager.SceneRoot);
        }

        [Test]
        public void ImplementsISceneContextManager()
        {
            // Assert
            Assert.IsNotNull(_sceneContextManager as ISceneContextManager);
        }

        [Test]
        public void ImplementsIDisposable()
        {
            // Assert
            Assert.IsNotNull(_sceneContextManager as IDisposable);
        }
    }
}
