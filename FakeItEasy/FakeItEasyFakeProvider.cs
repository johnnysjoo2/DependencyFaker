using FakeItEasy.Sdk;
using Vanilla.DependencyFaker.Sdk;

namespace Vanilla.DependencyFaker;

internal class FakeItEasyFakeProvider : IFakeProvider
{
    public object CreateFake(Type type) =>
        Create.Fake(type);

    public bool IsFake(object instance) =>
        instance.GetType().Name.StartsWith("ObjectProxy");
}