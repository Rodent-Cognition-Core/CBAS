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

            var authUser = Environment.GetEnvironmentVariable("ELASTICSEARCH_USERNAME");
            var authPassword = Environment.GetEnvironmentVariable("ELASTICSEARCH_PASSWORD");
            var certificate = Environment.GetEnvironmentVariable("ELASTICSEARCH_CERTIFICATE");
            settings = settings.BasicAuthentication(authUser, authPassword);
            settings = settings.CertificateFingerprint(certificate);
            IElasticClient _elasticClient = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(_elasticClient);
        }
    }
}
