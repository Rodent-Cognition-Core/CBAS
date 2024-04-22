using AngularSPAWebAPI.Models;
using System.Collections.Generic;
using System.Reflection;
using System;
using Nest;
using IdentityServer4.Models;
using System.Linq;

namespace CBAS.Services
{
    public class ElasticSearchService
    {
        private readonly IElasticClient _client;

        public ElasticSearchService(IElasticClient client)
        {
            _client = client;
        }

        public void AddToIndex(PubScreen pubScreen)
        {
            _client.Index(pubScreen, p => p.Index("pubscreen"));
        }

        public void EditDocument(PubScreen pubScreen)
        {
            _client.Update<PubScreen>(pubScreen.ID, p => p.Index("pubscreen").Doc(pubScreen));
        }

        public void DeleteDocument(PubScreen pubScreen)
        {
            _client.Delete<PubScreen>(pubScreen.ID, p=> p.Index("pubscreen"));
        }
    }
}
