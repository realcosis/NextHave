using Dolphin.Core.Configurations;
using Dolphin.Core.Configurations.Models;
using Dolphin.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace NextHave.API.Conf
{
    public static class ConfigurationExtensions
    {
        public static IHostBuilder ConfigureDolphinApplication(this IHostBuilder builder, string environmentVariableName, string version)
        {
            builder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var configuration = configBuilder.Configure(environmentVariableName).Build();
                builder.ConfigureServices((context, services) => services.RegisterOptions(configuration, version));
            });

            return builder;
        }

        static IConfigurationBuilder Configure(this IConfigurationBuilder builder, string environmentVariableName)
        {
            builder.AddEnvironmentVariables();

            builder.Add(new DolphinConfigurationSource(environmentVariableName));

            return builder;
        }

        static void RegisterOptions(this IServiceCollection services, IConfiguration configuration, string version)
        {
            services.Configure<Configuration>(configuration);

            if (configuration.GetSection("Project") != default)
                services.Configure<ProjectConfiguration>(opt =>
                {
                    opt.Title = configuration.GetValue<string>("Project:Title");
                    opt.SetVersion(version);
                });

            if (configuration.GetSection("Network") != default)
                services.Configure<NetworkConfiguration>(configuration.GetSection("Network"));

            services.Configure<MongoConfiguration>(configuration.GetSection("Mongo"));
            services.Configure<MySQLConfiguration>(configuration.GetSection("MySQL"));
            services.Configure<PoolConfiguration>(configuration.GetSection("Pool"));
        }

        internal static Configuration? ReadConfigurationFile(this string environmentVariableName)
        {
            var path = Environment.GetEnvironmentVariable(environmentVariableName);
            if (!string.IsNullOrWhiteSpace(path))
            {
                var result = File.ReadAllBytes(path);
                using var stream = new MemoryStream(result);
                return stream.StreamDeserialize<Configuration>();
            }
            return default;
        }

        internal static IDictionary<string, string?> ToFlatDictionary(this object obj)
        {
            var result = new Dictionary<string, string?>();
            var json = JObject.FromObject(obj);
            FlattenJson(json, null, result);
            return result;
        }

        static void FlattenJson(JToken token, string? parentPath, IDictionary<string, string?> result)
        {
            if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    var path = parentPath == null ? property.Name : $"{parentPath}:{property.Name}";
                    FlattenJson(property.Value, path, result);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    var path = $"{parentPath}:{i}";
                    FlattenJson(array[i], path, result);
                }
            }
            else
            {
                result[parentPath!] = token.ToString();
            }
        }
    }
}