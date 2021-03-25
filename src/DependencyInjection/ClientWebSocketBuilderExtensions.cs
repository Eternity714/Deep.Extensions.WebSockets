using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ClientWebSocketBuilderExtensions
    {
        public static IClientWebSocketBuilder ConfigureClientWebSocket(this IClientWebSocketBuilder builder, Action<ClientWebSocket> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.Services.Configure<ClientWebSocketFactoryOptions>(builder.Name, options => options.ClientWebSocketActions.Add(configure));

            return builder;
        }

        public static IClientWebSocketBuilder ConfigureClientWebSocket(this IClientWebSocketBuilder builder, Action<IServiceProvider, ClientWebSocket> configure)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.Services.AddTransient<IConfigureOptions<ClientWebSocketFactoryOptions>>(services =>
            {
                return new ConfigureNamedOptions<ClientWebSocketFactoryOptions>(builder.Name, (options) =>
                {
                    options.ClientWebSocketActions.Add(client => configure(services, client));
                });
            });

            return builder;
        }

        public static IClientWebSocketBuilder AddTypedWebSocket<TClient>(this IClientWebSocketBuilder builder) 
			where TClient : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddTypedWebSocketCore<TClient>(builder, false);
        }

        public static IClientWebSocketBuilder AddTypedWebSocketCore<TClient>(this IClientWebSocketBuilder builder, bool validateSingleType)
            where TClient : class
        {
            ReserveClient(builder, typeof(TClient), builder.Name, validateSingleType);

            builder.Services.AddTransient<TClient>(s => {
                IClientWebSocketFactory httpClientFactory = s.GetRequiredService<IClientWebSocketFactory>();
                ClientWebSocket client = httpClientFactory.CreateClient(builder.Name);

                ITypedClientWebSocketFactory<TClient> factory = s.GetRequiredService<ITypedClientWebSocketFactory<TClient>>();
                return factory.CreateClient(client);
            });

            return builder;
        }

        public static IClientWebSocketBuilder AddTypedWebSocket<TClient, TImplementation>(this IClientWebSocketBuilder builder) 
			where TClient : class 
			where TImplementation : class, TClient
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return AddTypedWebSocketCore<TClient, TImplementation>(builder, false);
        }

        public static IClientWebSocketBuilder AddTypedWebSocketCore<TClient, TImplementation>(this IClientWebSocketBuilder builder, bool validateSingleType)
            where TClient : class
            where TImplementation : class, TClient
        {
            ReserveClient(builder, typeof(TClient), builder.Name, validateSingleType);

            builder.Services.AddTransient<TClient>(s => {
                IClientWebSocketFactory httpClientFactory = s.GetRequiredService<IClientWebSocketFactory>();
                ClientWebSocket client = httpClientFactory.CreateClient(builder.Name);

                ITypedClientWebSocketFactory<TImplementation> factory = s.GetRequiredService<ITypedClientWebSocketFactory<TImplementation>>();
                return factory.CreateClient(client);
            });

            return builder;
        }

        public static IClientWebSocketBuilder AddTypedWebSocket<TClient>(this IClientWebSocketBuilder builder, Func<ClientWebSocket, TClient> factory) 
			where TClient : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return AddTypedWebSocketCore<TClient>(builder, factory, false);
        }

        public static IClientWebSocketBuilder AddTypedWebSocketCore<TClient>(this IClientWebSocketBuilder builder, Func<ClientWebSocket, TClient> factory, bool validateSingleType)
            where TClient : class
        {
            ReserveClient(builder, typeof(TClient), builder.Name, validateSingleType);

            builder.Services.AddTransient<TClient>(s => {
                IClientWebSocketFactory httpClientFactory = s.GetRequiredService<IClientWebSocketFactory>();
                ClientWebSocket client = httpClientFactory.CreateClient(builder.Name);

                return factory(client);
            });

            return builder;
        }

        public static IClientWebSocketBuilder AddTypedWebSocket<TClient>(this IClientWebSocketBuilder builder, Func<ClientWebSocket, IServiceProvider, TClient> factory) 
			where TClient : class
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return AddTypedWebSocketCore<TClient>(builder, factory, false);
        }

        internal static IClientWebSocketBuilder AddTypedWebSocketCore<TClient>(this IClientWebSocketBuilder builder, Func<ClientWebSocket, IServiceProvider, TClient> factory, bool validateSingleType)
            where TClient : class
        {
            ReserveClient(builder, typeof(TClient), builder.Name, validateSingleType);

            builder.Services.AddTransient<TClient>(s =>
            {
                IClientWebSocketFactory httpClientFactory = s.GetRequiredService<IClientWebSocketFactory>();
                ClientWebSocket client = httpClientFactory.CreateClient(builder.Name);

                return factory(client, s);
            });

            return builder;
        }

        private static void ReserveClient(IClientWebSocketBuilder builder, Type type, string name, bool validateSingleType)
        {
            var registry = (ClientWebSocketMappingRegistry)builder.Services.Single(sd => sd.ServiceType == typeof(ClientWebSocketMappingRegistry)).ImplementationInstance;
            Debug.Assert(registry != null);

            // Check for same name registered to two types. This won't work because we rely on named options for the configuration.
            if (registry.NamedClientRegistrations.TryGetValue(name, out Type otherType) &&

                // Allow using the same name with multiple types in some cases (see callers).
                validateSingleType &&

                // Allow registering the same name twice to the same type.
                type != otherType)
            {
                string message =
                    $"The ClientWebSocket factory already has a registered client with the name '{name}', bound to the type '{otherType.FullName}'. " +
                    $"Client names are computed based on the type name without considering the namespace ('{otherType.Name}'). " +
                    $"Use an overload of AddClientWebSocket that accepts a string and provide a unique name to resolve the conflict.";
                throw new InvalidOperationException(message);
            }

            if (validateSingleType)
            {
                registry.NamedClientRegistrations[name] = type;
            }
        }
    }
}
