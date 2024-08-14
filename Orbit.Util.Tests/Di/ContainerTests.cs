/*
 Converted from Kotlin to C#
*/

using NUnit.Framework;

namespace Orbit.Util.Di;

public class ContainerTests
{
    [Test]
    public void RegisteredFactoryGetsCalledOnResolve()
    {
        var container = new ComponentContainer();

        var test = new TestClass();
        container.Configure(() => { container.Register<TestClass>(() => test); });

        Assert.AreSame(container.Resolve<TestClass>(), test);
    }

    [Test]
    public void RegisteredFactoryIsUniquelyCalledEachResolve()
    {
        var container = new ComponentContainer();

        container.Configure(() => { container.Register<ComponentContainer>(() => new TestClass()); });

        Assert.AreNotSame(container.Resolve<TestClass>(), container.Resolve<TestClass>());
    }

    [Test]
    public void UnregisteredTypeIsConstructedThroughDefaultConstructor()
    {
        var container = new ComponentContainer();

        var test = container.Resolve<TestClass>();

        test.Message("test message");
    }

    [Test]
    public void UnregisteredTypeWithDependenciesIsConstructedRecursively()
    {
        var container = new ComponentContainer();

        var test = container.Resolve<TestClassWithDependencies>();

        test.Message("Test Message through dependency");
    }

    [Test]
    public void UnregisteredTypeUsesRegisteredDependency()
    {
        var container = new ComponentContainer();

        var dependency = new TestClass();
        dependency.SetPrefix("registered:");
        container.Configure(() => { container.Register<TestClass>(() => dependency); });

        Assert.AreEqual(container.Resolve<TestClassWithDependencies>().Message("message"), "registered:message");
    }

    [Test]
    public void RegisteringAnInstanceReturnsThatInstance()
    {
        var container = new ComponentContainer();

        var test = new TestClass();
        container.Configure(containerRoot => { containerRoot.Instance(test); });

        Assert.AreSame(container.Resolve<TestClass>(), container.Resolve<TestClass>());
    }

    [Test]
    public void RegisteringASingletonReturnsSameInstance()
    {
        var container = new ComponentContainer();

        container.Configure(containerRoot => { containerRoot.Singleton<TestClass>(); });

        Assert.AreSame(container.Resolve<TestClass>(), container.Resolve<TestClass>());
    }

    [Test]
    public void ExternallyConfiguredClassResolved()
    {
        var container = new ComponentContainer();

        container.Configure(containerRoot =>
        {
            containerRoot.ExternallyConfigured(
                new ExternallyConfiguredClass.ExternallyConfiguredClassConfig("registered:"));
        });

        var test = container.Resolve<ExternallyConfiguredClass>();

        Assert.AreEqual(test.Message("message"), "registered:message");
    }

    public class TestClass
    {
        private string _prefix = "";

        public void SetPrefix(string prefix)
        {
            _prefix = prefix;
        }

        public string Message(string msg)
        {
            Console.WriteLine(_prefix + msg);
            return _prefix + msg;
        }
    }

    public class TestClassWithDependencies
    {
        private readonly TestClass _dependency;

        public TestClassWithDependencies(TestClass dependency)
        {
            _dependency = dependency;
        }

        public string Message(string msg)
        {
            return _dependency.Message(msg);
        }
    }

    public class ExternallyConfiguredClass
    {
        private readonly string _prefix;

        public ExternallyConfiguredClass(ExternallyConfiguredClassConfig config)
        {
            _prefix = config.Prefix;
        }

        public string Message(string msg)
        {
            Console.WriteLine(_prefix + msg);
            return _prefix + msg;
        }

        public class ExternallyConfiguredClassConfig : ExternallyConfigured<ExternallyConfiguredClass>
        {
            public ExternallyConfiguredClassConfig(string prefix)
            {
                Prefix = prefix;
            }

            public string Prefix { get; }

            public override Type InstanceType => typeof(ExternallyConfiguredClass);
        }
    }
}