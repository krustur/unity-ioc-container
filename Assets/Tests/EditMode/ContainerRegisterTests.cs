using System;
using NUnit.Framework;
using UnityIoC;

namespace UnityIoC.Tests
{
    [TestFixture]
    public class ContainerRegisterTests
    {
        private IContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new Container();
        }

        // Test classes for registration
        public class TestService
        {
            public string GetMessage() => "TestService";
        }

        public class AnotherTestService
        {
            public int GetValue() => 42;
        }

        [Test]
        public void Register_TService_RegistersServiceAsItself()
        {
            // Arrange & Act
            _container.Register<TestService>();

            // Assert
            Assert.IsTrue(_container.IsRegistered<TestService>());
            var instance = _container.Resolve<TestService>();
            Assert.IsNotNull(instance);
            Assert.AreEqual("TestService", instance.GetMessage());
        }

        [Test]
        public void Register_TService_DefaultsToTransientLifetime()
        {
            // Arrange & Act
            _container.Register<TestService>();

            // Assert
            var instance1 = _container.Resolve<TestService>();
            var instance2 = _container.Resolve<TestService>();
            
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreNotSame(instance1, instance2, "Transient services should create new instances");
        }

        [Test]
        public void Register_TService_WithTransientLifetime_CreatesNewInstances()
        {
            // Arrange & Act
            _container.Register<TestService>(ServiceLifetime.Transient);

            // Assert
            var instance1 = _container.Resolve<TestService>();
            var instance2 = _container.Resolve<TestService>();
            
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreNotSame(instance1, instance2, "Transient services should create new instances");
        }

        [Test]
        public void Register_TService_WithSingletonLifetime_ReturnsSameInstance()
        {
            // Arrange & Act
            _container.Register<TestService>(ServiceLifetime.Singleton);

            // Assert
            var instance1 = _container.Resolve<TestService>();
            var instance2 = _container.Resolve<TestService>();
            
            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
            Assert.AreSame(instance1, instance2, "Singleton services should return the same instance");
        }

        [Test]
        public void Register_TService_MultipleTypes_RegistersEachCorrectly()
        {
            // Arrange & Act
            _container.Register<TestService>(ServiceLifetime.Transient);
            _container.Register<AnotherTestService>(ServiceLifetime.Singleton);

            // Assert
            Assert.IsTrue(_container.IsRegistered<TestService>());
            Assert.IsTrue(_container.IsRegistered<AnotherTestService>());

            var testService = _container.Resolve<TestService>();
            var anotherService = _container.Resolve<AnotherTestService>();
            
            Assert.IsNotNull(testService);
            Assert.IsNotNull(anotherService);
            Assert.AreEqual("TestService", testService.GetMessage());
            Assert.AreEqual(42, anotherService.GetValue());
        }

        [Test]
        public void Register_TService_GetImplementationType_ReturnsCorrectType()
        {
            // Arrange & Act
            _container.Register<TestService>();

            // Assert
            var implementationType = _container.GetImplementationType(typeof(TestService));
            Assert.IsNotNull(implementationType);
            Assert.AreEqual(typeof(TestService), implementationType);
        }
    }
}
