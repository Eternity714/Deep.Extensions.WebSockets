using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Internal;
using System;
using System.Net.WebSockets;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ClientWebSocketFactoryServiceCollectionExtensions
    {
        public static IServiceCollection AddClientWebSocket(this IServiceCollection services)
        {
            if (services == null) {
                throw new ArgumentNullException(nameof(services));
            }

            //services.AddLogging();
            services.AddOptions();

            services.TryAddSingleton<DefaultClientWebSocketFactory>();
            services.TryAddSingleton<IClientWebSocketFactory>(serviceProvider => serviceProvider.GetRequiredService<DefaultClientWebSocketFactory>());

            services.TryAdd(ServiceDescriptor.Transient(typeof(ITypedClientWebSocketFactory<>), typeof(DefaultTypedClientWebSocketFactory<>)));
            services.TryAdd(ServiceDescriptor.Transient(typeof(DefaultTypedClientWebSocketFactory<>.Cache), typeof(DefaultTypedClientWebSocketFactory<>.Cache)));

            services.TryAddSingleton(new ClientWebSocketMappingRegistry());

            services.TryAddTransient(s =>
            {
                return s.GetRequiredService<IClientWebSocketFactory>().CreateClient(string.Empty);
            });

            return services;
        }

        public static IClientWebSocketBuilder AddClientWebSocket(this IServiceCollection services, string name)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            AddClientWebSocket(services);

            return new DefaultClientWebSocketBuilder(services, name);
        }

        public static IClientWebSocketBuilder AddClientWebSocket(this IServiceCollection services, string name, Action<ClientWebSocket> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddClientWebSocket(services);

            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.ConfigureClientWebSocket(configure);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket(this IServiceCollection services, string name, Action<IServiceProvider, ClientWebSocket> configure)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddClientWebSocket(services);

            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.ConfigureClientWebSocket(configure);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient>(this IServiceCollection services) 
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            AddClientWebSocket(services);

            string name = TypeNameHelper.GetTypeDisplayName(typeof(TClient), fullName: false);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.AddTypedWebSocketCore<TClient>(validateSingleType: true);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient>(this IServiceCollection services, string name) 
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            AddClientWebSocket(services);

            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.AddTypedWebSocketCore<TClient>(validateSingleType: false);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient, TImplementation>(this IServiceCollection services) 
            where TClient : class 
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            AddClientWebSocket(services);

            string name = TypeNameHelper.GetTypeDisplayName(typeof(TClient), fullName: false);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.AddTypedWebSocketCore<TClient, TImplementation>(validateSingleType: true);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient, TImplementation>(this IServiceCollection services, string name)
            where TClient : class 
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            AddClientWebSocket(services);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.AddTypedWebSocketCore<TClient, TImplementation>(validateSingleType: false);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient>(this IServiceCollection services, Action<ClientWebSocket> configure) 
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddClientWebSocket(services);
            string name = TypeNameHelper.GetTypeDisplayName(typeof(TClient), fullName: false);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.ConfigureClientWebSocket(configure);
            builder.AddTypedWebSocketCore<TClient>(validateSingleType: true);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient>(this IServiceCollection services, Action<IServiceProvider, ClientWebSocket> configure) 
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddClientWebSocket(services);
            string name = TypeNameHelper.GetTypeDisplayName(typeof(TClient), fullName: false);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.ConfigureClientWebSocket(configure);
            builder.AddTypedWebSocketCore<TClient>(validateSingleType: true);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient>(this IServiceCollection services, string name, Action<ClientWebSocket> configure)
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddClientWebSocket(services);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.ConfigureClientWebSocket(configure);
            builder.AddTypedWebSocketCore<TClient>(validateSingleType: false);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient>(this IServiceCollection services, string name, Action<IServiceProvider, ClientWebSocket> configure)
            where TClient : class
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddClientWebSocket(services);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.ConfigureClientWebSocket(configure);
            builder.AddTypedWebSocketCore<TClient>(validateSingleType: false);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient, TImplementation>(this IServiceCollection services, Action<ClientWebSocket> configure) 
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddClientWebSocket(services);
            string name = TypeNameHelper.GetTypeDisplayName(typeof(TClient), fullName: false);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.ConfigureClientWebSocket(configure);
            builder.AddTypedWebSocketCore<TClient, TImplementation>(validateSingleType: true);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient, TImplementation>(this IServiceCollection services, Action<IServiceProvider, ClientWebSocket> configure) 
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddClientWebSocket(services);
            string name = TypeNameHelper.GetTypeDisplayName(typeof(TClient), fullName: false);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.ConfigureClientWebSocket(configure);
            builder.AddTypedWebSocketCore<TClient, TImplementation>(validateSingleType: true);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient, TImplementation>(this IServiceCollection services, string name, Action<ClientWebSocket> configure)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddClientWebSocket(services);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.ConfigureClientWebSocket(configure);
            builder.AddTypedWebSocketCore<TClient, TImplementation>(validateSingleType: false);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient, TImplementation>(this IServiceCollection services,  string name, Action<IServiceProvider, ClientWebSocket> configure)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            AddClientWebSocket(services);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.ConfigureClientWebSocket(configure);
            builder.AddTypedWebSocketCore<TClient, TImplementation>(validateSingleType: false);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient, TImplementation>(this IServiceCollection services, Func<ClientWebSocket, TImplementation> factory)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            AddClientWebSocket(services);
            string name = TypeNameHelper.GetTypeDisplayName(typeof(TClient), fullName: false);
            return AddClientWebSocket<TClient, TImplementation>(services, name, factory);
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient, TImplementation>(this IServiceCollection services, string name, Func<ClientWebSocket, TImplementation> factory)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            AddClientWebSocket(services);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.AddTypedWebSocket<TClient>(factory);
            return builder;
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient, TImplementation>(this IServiceCollection services, Func<ClientWebSocket, IServiceProvider, TImplementation> factory)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            AddClientWebSocket(services);
            string name = TypeNameHelper.GetTypeDisplayName(typeof(TClient), fullName: false);
            return AddClientWebSocket<TClient, TImplementation>(services, name, factory);
        }

        public static IClientWebSocketBuilder AddClientWebSocket<TClient, TImplementation>(this IServiceCollection services, string name, Func<ClientWebSocket, IServiceProvider, TImplementation> factory)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            AddClientWebSocket(services);
            var builder = new DefaultClientWebSocketBuilder(services, name);
            builder.AddTypedWebSocket<TClient>(factory);
            return builder;
        }
    }
}
