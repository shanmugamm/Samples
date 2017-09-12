using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Pivotal.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.CloudFoundry;
using StructureMap;
using StructureMapCloundFoundry.Models;
using StructureMapCloundFoundry;

namespace StructureMapCloundFoundry
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            System.Environment.SetEnvironmentVariable("VCAP_SERVICES", "{\"p-config-server\":[{\"credentials\":{\"uri\":\"https://config-59cfb7fe-6c88-4497-acd1-0db5fd7b87ea.cfapps.pcf1.vc1.pcf.dell.com\",\"client_secret\":\"M0ofPZxjCll3\",\"client_id\":\"p-config-server-526a125b-bd3f-495e-84c8-55f4656d5b28\",\"access_token_uri\":\"https://p-spring-cloud-services.uaa.sys.pcf1.vc1.pcf.dell.com/oauth/token\"},\"syslog_drain_url\":null,\"volume_mounts\":[],\"label\":\"p-config-server\",\"provider\":null,\"plan\":\"standard\",\"name\":\"TestConfigServer\",\"tags\":[\"configuration\",\"spring-cloud\"]}]}");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            // Get the configuration from the Spring Cloud Config Server using the "development" environment
            ConfigServerConfig.RegisterConfig("development");
            RegisterIoc(ConfigServerConfig.Configuration);
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(ObjectFactory.Container));
        }

        public static void RegisterIoc(IConfigurationRoot configuration)
        {
            StructureMap.ObjectFactory.Container.Configure(x =>
            {
                x.For(typeof(IOptions<>)).Use(typeof(OptionsManager<>)).Singleton();
                x.For(typeof(IOptionsMonitor<>)).Use(typeof(OptionsMonitor<>)).Singleton();
                x.For(typeof(IOptionsSnapshot<>)).Use(typeof(OptionsSnapshot<>));


                x.For<IOptionsChangeTokenSource<ConfigServerClientSettingsOptions>>()
                    .Use(new ConfigurationChangeTokenSource<ConfigServerClientSettingsOptions>(configuration)).Singleton();
                x.For<IConfigureOptions<ConfigServerClientSettingsOptions>>()
                    .Use(new ConfigureFromConfigurationOptions<ConfigServerClientSettingsOptions>(configuration)).Singleton();


                x.For<IOptionsChangeTokenSource<CloudFoundryApplicationOptions>>()
                    .Use(new ConfigurationChangeTokenSource<CloudFoundryApplicationOptions>(configuration)).Singleton();
                x.For<IConfigureOptions<CloudFoundryApplicationOptions>>()
                    .Use(new ConfigureFromConfigurationOptions<CloudFoundryApplicationOptions>(configuration)).Singleton();

                x.For<IOptionsChangeTokenSource<CloudFoundryServicesOptions>>()
                    .Use(new ConfigurationChangeTokenSource<CloudFoundryServicesOptions>(configuration)).Singleton();
                x.For<IConfigureOptions<CloudFoundryServicesOptions>>()
                    .Use(new ConfigureFromConfigurationOptions<CloudFoundryServicesOptions>(configuration)).Singleton();
                x.For<IOptionsChangeTokenSource<ConfigServerData>>()
                    .Use(new ConfigurationChangeTokenSource<ConfigServerData>(configuration)).Singleton();
                x.For<IConfigureOptions<ConfigServerData>>()
                    .Use(new ConfigureFromConfigurationOptions<ConfigServerData>(configuration)).Singleton();
                x.For<IConfigurationRoot>().Use(configuration).Singleton();


            });
        }
    }
}
