using Microsoft.Extensions.DependencyInjection;

namespace Vanilla.DependencyFaker.Sdk;

public abstract class DependencyFakerBase<TSut>(Action<IServiceCollection> configureDependencies, IFakeProvider fakeProvider) : DependencyFakerBase(services =>
    {
        configureDependencies?.Invoke(services);
        services.AddSingleton<TSut>();
    }, fakeProvider)
    where TSut : class
{
    public TSut Sut => 
        Service<TSut>();
}

public abstract class DependencyFakerBase
{
    private readonly ServiceProviderWithFakeFallback _services;
    private readonly ServiceCollection _serviceCollection;

    public DependencyFakerBase(Action<IServiceCollection> configureDependencies, IFakeProvider fakeProvider)
    {
        _serviceCollection = new ServiceCollection();
        configureDependencies?.Invoke(_serviceCollection);

        _services = new ServiceProviderWithFakeFallback(_serviceCollection, fakeProvider);
    }

    private T ServiceOrFake<T>() 
        where T: class =>
        _services.GetRequiredService<T>();

    public void ReplaceSingleton<TService>(TService implementation)
        where TService : class => 
        _services.ReplaceSingleton(implementation);

    public T Fake<T>()
        where T: class
    {
        var service = ServiceOrFake<T>();
        if (!_services.IsFake(service))
        {
            throw new InvalidOperationException("The requested service is not a fake. It's an implementation.");
        }

        return service;
    }

    public T Service<T>()
        where T : class
    {
        var service = ServiceOrFake<T>();
        if (_services.IsFake(service))
        {
            throw new InvalidOperationException("The requested service is not an implementation. It's a fake.");
        }

        return service;
    }
}