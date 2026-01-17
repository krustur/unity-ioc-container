using NUnit.Framework;
using UnityIoC;
using UnityIoC.SceneManagement;

namespace UnityIoC.Tests
{
    [TestFixture]
    public class SceneContextManagerIoCTests
    {
        private IContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new Container();
        }

        [Test]
        public void Container_CanRegisterSceneContextManager_AsSingleton()
        {
            // Act
            _container.Register<ISceneContextManager, SceneContextManager>(ServiceLifetime.Singleton);

            // Assert
            Assert.IsTrue(_container.IsRegistered<ISceneContextManager>());
        }

        [Test]
        public void Container_ResolveSceneContextManager_ReturnsSameInstance()
        {
            // Arrange
            _container.Register<ISceneContextManager, SceneContextManager>(ServiceLifetime.Singleton);

            // Act
            var instance1 = _container.Resolve<ISceneContextManager>();
            var instance2 = _container.Resolve<ISceneContextManager>();

            // Assert
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreSame(instance1, instance2, "Singleton should return the same instance");
        }

        [Test]
        public void Container_ResolveSceneContextManager_ReturnsValidImplementation()
        {
            // Arrange
            _container.Register<ISceneContextManager, SceneContextManager>(ServiceLifetime.Singleton);

            // Act
            var instance = _container.Resolve<ISceneContextManager>();

            // Assert
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<SceneContextManager>(instance);
        }

        [Test]
        public void Container_GetImplementationType_ReturnsSceneContextManager()
        {
            // Arrange
            _container.Register<ISceneContextManager, SceneContextManager>(ServiceLifetime.Singleton);

            // Act
            var implementationType = _container.GetImplementationType(typeof(ISceneContextManager));

            // Assert
            Assert.AreEqual(typeof(SceneContextManager), implementationType);
        }
    }
}
