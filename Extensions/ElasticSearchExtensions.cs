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

            var authUser = Environment.GetEnvironmentVariable("SEARCH_USER");
            var authPassword = Environment.GetEnvironmentVariable("SEARCH_PASS");
            var certificate = Environment.GetEnvironmentVariable("SEARCH_CERT");
            settings = settings.BasicAuthentication(authUser, authPassword);
            settings = settings.CertificateFingerprint(certificate);
            IElasticClient _elasticClient = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(_elasticClient);
        }
    }
}
