using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using MyIgniteApi.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyIgniteApi.Services
{
    public class UniversityService
    {
        private const string CacheName = "universities";
        private readonly ICache<string, List<University>> _cache;

        public UniversityService()
        {
            var ignite = Ignition.TryGetIgnite() ?? Ignition.Start();
            _cache = ignite.GetOrCreateCache<string, List<University>>(new CacheConfiguration(CacheName));
        }

        public async Task<Dictionary<string, List<University>>> FetchAndStoreUniversitiesParallel(List<string> countries)
        {
            var tasks = new List<Task>();
            var data = new Dictionary<string, List<University>>();

            foreach (var country in countries)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var universities = await FetchUniversitiesForCountry(country);
                    _cache.Put(country, universities);
                    data[country] = universities;
                }));
            }

            await Task.WhenAll(tasks);
            return data;
        }

        private async Task<List<University>> FetchUniversitiesForCountry(string country)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync($"http://universities.hipolabs.com/search?country={country}");
                return JsonSerializer.Deserialize<List<University>>(response) ?? new List<University>();
            }
        }
    }
}
