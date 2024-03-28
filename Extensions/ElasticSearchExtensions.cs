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

            var authUser = configuration["ElasticSearch:AuthUser"];
            var authPassword = configuration["ElasticSearch:AuthPassword"];
            var certificate = configuration["ElasticSearch:Certificate"];
            settings = settings.BasicAuthentication(authUser, authPassword);
            settings = settings.CertificateFingerprint(certificate);
            IElasticClient _elasticClient = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(_elasticClient);
        }
    }
}
