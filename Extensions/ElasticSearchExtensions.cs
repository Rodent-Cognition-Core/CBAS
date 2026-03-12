using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace CBAS.Extensions
{
    public static class ElasticSearchExtensions
    {
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new ConnectionSettings(new Uri(configuration["ElasticSearch:uri"]));

            var defaultIndex = Environment.GetEnvironmentVariable("SEARCH_INDEX") ?? configuration["ElasticSearch:defaultIndex"];

            if (!string.IsNullOrEmpty(defaultIndex) && defaultIndex != "%DEFAULT_INDEX%")
            {
                settings = settings.DefaultIndex(defaultIndex);
            }

            var authUser = Environment.GetEnvironmentVariable("SEARCH_USER") ?? configuration["ElasticSearch:Username"];
            var authPassword = Environment.GetEnvironmentVariable("SEARCH_PASS") ?? configuration["ElasticSearch:Password"];
            var certificate = Environment.GetEnvironmentVariable("SEARCH_CERT") ?? configuration["ElasticSearch:Certificate"];

            if (!string.IsNullOrEmpty(authUser) && authUser != "%SEARCH_USER%")
            {
                settings = settings.BasicAuthentication(authUser, authPassword);
            }

            if (!string.IsNullOrEmpty(certificate) && certificate != "%SEARCH_CERT%")
            {
                settings = settings.CertificateFingerprint(certificate);
            }
            IElasticClient _elasticClient = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(_elasticClient);
        }
    }
}
