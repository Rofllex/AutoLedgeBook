using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;


namespace AutoLedgeBook.Utils
{
    public class ExactTime : IExactTimeSource
    {
        private readonly IExactTimeSource[] _timeSources;

        public ExactTime() : this (new InternetExactTimeSource(), new DirectExactTimeSource())
        {
            
        }

        public ExactTime(params IExactTimeSource[] sources)
        {
            _timeSources = sources;
        }

        public DateTime GetUTC()
        {
            for (var i = 0; i < _timeSources.Length; i++)
            {
                try
                {
                    return _timeSources[i].GetUTC();
                }
                catch { }
            }

            throw new Exception("Не удалось получить время");
        }

        public async Task<DateTime> GetUTCAsync()
        {
            for (var i = 0; i < _timeSources.Length; i++)
            {
                return await _timeSources[i].GetUTCAsync();
            }

            throw new Exception("Не удалось получить время");
        }
    }

    public interface IExactTimeSource
    {
        Task<DateTime> GetUTCAsync();
        DateTime GetUTC();
    }

    public class InternetExactTimeSource : IExactTimeSource
    {
        public InternetExactTimeSource()
        {

        }

        public async Task<DateTime> GetUTCAsync()
        {
            const string TIME_REQUEST_URI = @"https://worldtimeapi.org/api/timezone/Europe/London";
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(TIME_REQUEST_URI);

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Не удалось получить дату и время из интернета... CODE: { response.StatusCode }");

            string stringContent = await response.Content.ReadAsStringAsync();

            WorldTimeApiResponse timeResponse = JsonConvert.DeserializeObject<WorldTimeApiResponse>(stringContent)!;

            return timeResponse.UtcDateTime;
        }

        public DateTime GetUTC() => GetUTCAsync().GetAwaiter().GetResult();

        [Serializable]
        private class WorldTimeApiResponse 
        {
            [JsonConstructor]
            private WorldTimeApiResponse() { }

            [JsonProperty("utc_datetime")]
            public DateTime UtcDateTime { get; private set; }
        }

    }

    public class DirectExactTimeSource : IExactTimeSource
    {
        public DateTime GetUTC() => DateTime.UtcNow;

        public Task<DateTime> GetUTCAsync() => Task.Run(() => DateTime.UtcNow);
    }
}
