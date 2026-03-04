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

            var defaultIndex = configuration["ElasticSearch:defaultIndex"];

            if (!string.IsNullOrEmpty(defaultIndex))
            {
                settings = settings.DefaultIndex(defaultIndex);
            }

            var authUser = configuration["ElasticSearch:Username"] ?? Environment.GetEnvironmentVariable("SEARCH_USER");
            var authPassword = configuration["ElasticSearch:Password"] ?? Environment.GetEnvironmentVariable("SEARCH_PASS");
            var certificate = configuration["ElasticSearch:Certificate"] ?? Environment.GetEnvironmentVariable("SEARCH_CERT");

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
