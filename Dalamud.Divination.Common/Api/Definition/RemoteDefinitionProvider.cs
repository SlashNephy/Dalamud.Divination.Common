﻿using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Timers;
using Newtonsoft.Json.Linq;

namespace Dalamud.Divination.Common.Api.Definition
{
    public class RemoteDefinitionProvider<TContainer> : DefinitionProvider<TContainer> where TContainer : DefinitionContainer, new()
    {
        private const string DefaultBaseUrl = "https://shard.horoscope.dev/";
        private readonly string url;
        private readonly Timer timer = new(60 * 60 * 1000);

        public RemoteDefinitionProvider(string baseUrl = DefaultBaseUrl, string filename = DefaultFilename)
        {
            Filename = filename;
            url = $"{baseUrl}{filename}";

            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Update();
        }

        public override string Filename { get; }

        protected override JObject Fetch()
        {
            var request = WebRequest.CreateHttp(url);
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.Method = "GET";

            using var response = request.GetResponse();
            using var stream = response.GetResponseStream();
            using var reader = new StreamReader(stream!, Encoding.UTF8);

            var content = reader.ReadToEnd();
            return JObject.Parse(content);
        }

        public override void Dispose()
        {
            base.Dispose();
            timer.Dispose();
        }
    }
}