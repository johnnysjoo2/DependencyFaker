using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace Vanilla.DependencyFaker.Sdk;

internal sealed class ServiceProviderWithFakeFallback(IServiceCollection services, IFakeProvider fakeProvider) : IServiceProvider
{
    private readonly IServiceCollection _services = services;
    private readonly IFakeProvider _fakeProvider = fakeProvider;
    private readonly ICollection<(Type, object)> _instances = [];

    public object? GetService(Type serviceType)
    {
        if (typeof(IEnumerable).IsAssignableFrom(serviceType))
        {
            return GetServices(serviceType.GenericTypeArguments.First());
        }

        var serviceDescriptor = GetFirstDescriptor(serviceType);
        return GetServiceOrFake(serviceType, serviceDescriptor);
    }

    public bool IsFake<T>(T instance)
        where T : class =>
        _fakeProvider.IsFake(instance);

    internal void ReplaceSingleton<TService>(TService implementation)
        where TService : class
    {
        var foundService = _services.SingleOrDefault(x => x.ServiceType == typeof(TService));
        if (foundService != null)
        {
            _services.Remove(foundService);
        }

        foreach(var instance in _instances.Where(_ => _.Item1 == typeof(TService)).ToList())
        {
            _instances.Remove(instance);
        }

        _services.Add(new ServiceDescriptor(typeof(TService), implementation));
    }

    private IEnumerable GetServices(Type serviceType)
    {
        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(serviceType))!;

        var serviceDescriptors = _services.Where(x => x.ServiceType == serviceType);
        foreach (var serviceDescriptor in serviceDescriptors)
        {
            list.Add(GetServiceOrFake(serviceType, serviceDescriptor));
        }

        var instances = _instances.Where(_ => _.Item1 == serviceType);
        foreach (var instance in instances)
        {
            list.Add(instance.Item2);
        }

        return list;
    }

    private object? GetServiceOrFake(Type serviceType, ServiceDescriptor? serviceDescriptor)
    {
        if (serviceDescriptor != null)
        {
            return GetOrCreateInstance(serviceDescriptor);
        }
        else if (serviceType.IsInterface)
        {
            return GetOrCreateInstance(serviceType, true, CreateFake);
        }

        return null;
    }

    private object CreateFake(Type type) =>
        _fakeProvider.CreateFake(type);

    private IEnumerable<ServiceDescriptor> GetDescriptors(Type type) =>
        _services.Where(x => x.ServiceType == type);

    private ServiceDescriptor? GetFirstDescriptor(Type type) =>
        GetDescriptors(type).FirstOrDefault();

    private object GetOrCreateInstance(ServiceDescriptor serviceDescriptor)
    {
        var type = serviceDescriptor.ImplementationType ?? serviceDescriptor.ServiceType;
        bool isSingleton = serviceDescriptor.Lifetime == ServiceLifetime.Singleton;
        return serviceDescriptor.ImplementationInstance ?? GetOrCreateInstance(type, isSingleton, CreateInstance);
    }

    private object GetOrCreateInstance(Type serviceType, bool singleton, Func<Type, object> createInstance)
    {
        if (!singleton)
        {
            return createInstance(serviceType);
        }

        var instance = _instances.FirstOrDefault(_ => _.Item1 == serviceType);
        if (instance == default)
        {
            instance = new (serviceType, createInstance(serviceType));
            _instances.Add(instance);
        }

        return instance.Item2;
    }

    private object CreateInstance(Type serviceType) =>
        ActivatorUtilities.CreateInstance(this, serviceType);
}