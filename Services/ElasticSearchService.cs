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
        private static string[] MULTISEARCHFIELDS = { "title", "keywords", "author" };
        private const int SEARCHRESULTSIZE = 10000;
        private readonly IElasticClient _client;

        public ElasticSearchService(IElasticClient client)
        {
            _client = client;
        }
        private QueryContainer ApplyQuery(PubScreen pubscreen, QueryContainerDescriptor<PubScreenSearch> query)
        {
            return query.Bool(boolQ => boolQ.Must(boolQM => boolQM
                                            .DisMax(dxq => dxq
                                                .Queries(JoinAllQuries(pubscreen, query).ToArray())))
                                            .Filter(AddFilter(pubscreen).ToArray()));
        }
        private List<QueryContainer> JoinAllQuries(PubScreen pubscreen, QueryContainerDescriptor<PubScreenSearch> query)
        {
            var container = new List<QueryContainer>();

            var multiMatchQuery = MultiMatchSearchField(pubscreen, query);
            if (multiMatchQuery.Count > 0)
            {
                foreach (var multiQuery in multiMatchQuery)
                {
                    container.Add(multiQuery);
                }
            }
            return container;
        }

        private List<Func<QueryContainerDescriptor<PubScreenSearch>, QueryContainer>> AddFilter(PubScreen pubscreen)
        {
            var filterQuery = new List<Func<QueryContainerDescriptor<PubScreenSearch>, QueryContainer>>();
            foreach (PropertyInfo pi in pubscreen.GetType().GetProperties())
            {
                if (pi.Name == "search")
                {
                    continue;
                }

                string value = Convert.ToString(pi.GetValue(pubscreen, null));
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (pi.Name == "YearFrom")
                {
                    double yearFrom = 0;
                    if (double.TryParse(value, out yearFrom))
                    {
                        filterQuery.Add(fq => fq
                        .Range(range => range
                            .Field(new Nest.Field("year"))
                        .GreaterThanOrEquals(yearFrom))
                        );
                    }
                    else
                    {
                        Console.WriteLine("Unable to parse Year From '{0}'", value);
                    }

                }

                else if (pi.Name == "YearTo")
                {
                    double yearTo = 0;
                    if (double.TryParse(value, out yearTo))
                    {
                        filterQuery.Add(fq => fq
                        .Range(range => range
                            .Field(new Nest.Field("year"))
                            .LessThanOrEquals(yearTo))
                        );
                    }
                    else
                    {
                        Console.WriteLine("Unable to parse Year To '{0}", value);
                    }
                }
                else if (pi.PropertyType == typeof(string))
                {
                    filterQuery.Add(fq => fq
                            .Bool(dxqm => dxqm
                                .Must(boolShould => boolShould
                                .QueryString(queryString => queryString
                                        .DefaultField(new Nest.Field(pi.Name.ToLower()))
                                          .Query(value.ToLower())))));
                }
                else
                {
                    filterQuery.Add(fq => fq.Terms(t => t.Field(new Nest.Field(pi.Name)).Terms(value)));
                }
            }
            return filterQuery;
        }
        public List<QueryContainer> MultiMatchSearchField(PubScreen pubscreen, QueryContainerDescriptor<PubScreenSearch> query) =>
            string.IsNullOrEmpty(pubscreen.search) ? new List<QueryContainer>() : ApplyMatchQuery(pubscreen.search, query);


        private List<QueryContainer> ApplyMatchQuery(string searchingFor, QueryContainerDescriptor<PubScreenSearch> query)
        {
            var queryContainer = new List<QueryContainer>();
            foreach (var field in MULTISEARCHFIELDS)
            {
                var listOfSearchQuery = MatchRelevance(searchingFor, query, field);
                foreach (var searchQuery in listOfSearchQuery)
                {
                    queryContainer.Add(searchQuery);
                }
            }
            return queryContainer;
        }

        private List<QueryContainer> MatchRelevance(object searchingFor, QueryContainerDescriptor<PubScreenSearch> query, string fieldName)
        {
            var queryContainer = new List<QueryContainer>();
            queryContainer.Add(MatchWithFuzziness(searchingFor, query, fieldName));
            queryContainer.Add(MatchWithWildCard(searchingFor, query, fieldName));
            return queryContainer;
        }

        private QueryContainer MatchWithFuzziness(object searchingFor, QueryContainerDescriptor<PubScreenSearch> query, string fieldName)
        {
            var queryField = new Nest.Field(fieldName);
            return query
                        .Match(dxqm => dxqm
                        .Field(queryField)
                        .Query(searchingFor.ToString())
                        .Fuzziness(Nest.Fuzziness.Auto)
                    );
        }


        private QueryContainer MatchWithWildCard(object searchingFor, QueryContainerDescriptor<PubScreenSearch> query, string fieldName) => query
        .Bool(boolq => boolq
                        .Should(boolShould => boolShould
                        .Wildcard(dxqm => dxqm
                        .Field(new Nest.Field(fieldName))
                        .Value("*" + searchingFor.ToString() + "*"))));

        public List<PubScreenSearch> Search(PubScreen pubScreen)
        {
            var results = new List<PubScreenSearch>();
            try
            {
                var searchResult = _client.Search<PubScreenSearch>(s => s.Index("pubscreen")
                    .Size(SEARCHRESULTSIZE)
                    .Query(q => ApplyQuery(pubScreen, q)
                        )
                    );

                results = searchResult.Hits.Select(hit => hit.Source).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return results;
        }
    }
}
