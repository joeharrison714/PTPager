using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTPager.Control.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PTPager.Control
{
    public class SmartThingsService
    {
        private string _apiToken;
        private string _apiUrl;

        public SmartThingsService(string apiToken, string apiUrl)
        {
            _apiToken = apiToken;
            _apiUrl = apiUrl;
        }

        public async Task<string> GetCurrentMode()
        {
            Flurl.Url url = new Flurl.Url(_apiUrl);
            string content = await url.AppendPathSegment("locations")
                .WithHeader("Authorization", string.Format("Bearer " + _apiToken))
                .GetStringAsync();

            Console.WriteLine(content);

            var json = JObject.Parse(content);

            string currentMode = json["currentMode"]["name"].Value<string>();

            return currentMode;
        }

        public async Task ExecuteRoutine(string id)
        {
            Flurl.Url url = new Flurl.Url(_apiUrl);
            await url.AppendPathSegments("routines", id)
                .WithHeader("Authorization", string.Format("Bearer " + _apiToken))
                .PostAsync(new StringContent(""));
        }

        public async Task<IEnumerable<Routine>> ListRoutines()
        {
            Flurl.Url url = new Flurl.Url(_apiUrl);
            string content = await url.AppendPathSegment("routines")
                .WithHeader("Authorization", string.Format("Bearer " + _apiToken))
                .GetStringAsync();

            Console.WriteLine(content);

            var array = JArray.Parse(content);

            List<Routine> items = new List<Routine>();

            foreach (var item in array)
            {
                items.Add(new Routine()
                {
                    Label = item["label"].Value<string>(),
                    Id = item["id"].Value<string>(),
                });
            }

            return items;
        }


            public async Task<IEnumerable<Device>> ListDevices()
        {
            Flurl.Url url = new Flurl.Url(_apiUrl);
            string content = await url.AppendPathSegment("switches")
                .WithHeader("Authorization", string.Format("Bearer " + _apiToken))
                .GetStringAsync();

            Console.WriteLine(content);

            var array = JArray.Parse(content);

            List<Device> devices = new List<Device>();

            foreach (var item in array)
            {
                devices.Add(new Device()
                {
                    Name = item["name"].Value<string>(),
                    State = string.Equals(item["value"].Value<string>(), "on", StringComparison.OrdinalIgnoreCase) ? DeviceState.On : DeviceState.Off
                });
            }

            return devices;
        }


        internal class Rootobject
        {
            public Class1[] Property1 { get; set; }
        }

        internal class Class1
        {
            public string name { get; set; }
            public string value { get; set; }
        }

    }
}
