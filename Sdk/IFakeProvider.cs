using System;

namespace Vanilla.DependencyFaker.Sdk;

public interface IFakeProvider
{
    object CreateFake(Type type);
    bool IsFake(object instance);
}