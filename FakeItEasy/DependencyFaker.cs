using Microsoft.Extensions.DependencyInjection;
using Vanilla.DependencyFaker.Sdk;

namespace Vanilla.DependencyFaker;

public class DependencyFaker<TSut> : DependencyFakerBase<TSut>
    where TSut : class
{
    public DependencyFaker(Action<IServiceCollection> configureDependencies) : base(configureDependencies, new FakeItEasyFakeProvider()) { }
}