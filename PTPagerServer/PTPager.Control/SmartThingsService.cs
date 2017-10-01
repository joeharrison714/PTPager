using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PTPager.Control.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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


        public async Task<IEnumerable<Device>> ListDevices()
        {
            Flurl.Url url = new Flurl.Url(_apiUrl);
            string content = await url.AppendPathSegment("switches")
                .WithHeader("Authorization", string.Format("Bearer " + _apiToken))
                .GetStringAsync();

            //var client = new RestClient(_apiUrl);
            //RestRequest request = new RestRequest("switches", Method.GET);
            //request.AddParameter("Authorization",
            //string.Format("Bearer " + _apiToken),
            //            ParameterType.HttpHeader);
            //request.AddHeader("Accept", "application/json");
            //request.RequestFormat = DataFormat.Json;
            //var resp = await client.ExecuteAsync(request);

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
