using System;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Sikiro.Tookits.Consul
{
    public static class ConfigurationExtensions
    {
        private static ConsulConfiguration _consulConfiguration;

        public static IApplicationBuilder UseConsul(this IApplicationBuilder app, IApplicationLifetime lifetime, Action<ServerConfiguration> optionAction)
        {
            var consulClient = app.ApplicationServices.GetService<ConsulClient>();
            if (consulClient == null)
                throw new Exception("please AddConsul first");

            var serverConfiguration = new ServerConfiguration();
            optionAction(serverConfiguration);

            var serviceRegistration = GetServiceRegistration(serverConfiguration);

            //添加注册
            consulClient.Agent.ServiceRegister(serviceRegistration).Wait();

            //取消注册
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceRegistration.ID).Wait();
            });
            return app;
        }

        public static IServiceCollection AddConsul(this IServiceCollection serviceCollection, Action<ConsulConfiguration> optionAction)
        {
            _consulConfiguration = new ConsulConfiguration();
            optionAction(_consulConfiguration);

            var consulClient = new ConsulClient(x =>
                x.Address = new Uri(_consulConfiguration.Host));

            serviceCollection.AddSingleton(consulClient);

            return serviceCollection;
        }

        private static Uri GetSelfUri(string uristring)
        {
            return new Uri(uristring);
        }

        private static AgentServiceRegistration GetServiceRegistration(ServerConfiguration serverConfiguration)
        {
            var localIp = GetSelfUri(serverConfiguration.SelfHost);

            var serviceRegistration = new AgentServiceRegistration
            {
                Check = new AgentServiceCheck
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(60),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = $"http://{localIp.Host}:{localIp.Port}/api/health",
                    Timeout = TimeSpan.FromSeconds(3)
                },
                ID = Guid.NewGuid().ToString("N"),
                Name = serverConfiguration.ServerName,
                Address = localIp.Host,
                Port = localIp.Port,
                Tags =
                    new[]
                    {
                        serverConfiguration.ServerName
                    }
            };

            return serviceRegistration;
        }
    }

    public class ConsulConfiguration
    {
        internal string Host { get; set; }

        public void WithHost(string host)
        {
            Host = host;
        }
    }

    public class ServerConfiguration
    {
        internal string ServerName { get; set; }

        internal string SelfHost { get; set; }

        public void WithServerName(string serverName)
        {
            ServerName = serverName;
        }

        public void WithSelfHost(string selfHost)
        {
            SelfHost = selfHost;
        }
    }

    [Route("api/[Controller]")]
    public class HealthController : Controller
    {
        [HttpGet]
        public OkResult Get()
        {
            return Ok();
        }
    }
}
