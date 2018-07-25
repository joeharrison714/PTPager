using Newtonsoft.Json;
using PTPager.Web2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PTPager.Web2.Repository
{
    public class ScreensRepository
    {
        public static string RepositoryFile { get; set; } = "Screens.json";

        public static ScreenData Get()
        {
            ScreenData oai = null;

            if (File.Exists(RepositoryFile) == false)
            {
                ScreenData sd = new ScreenData();
                sd.Screens.Add(new ScreenInfo()
                {
                    Id = "1",
                    Label= "Routines",
                    Items = new List<ScreenItem>()
                    {
                        new ScreenItem()
                        {
                            Label="I'm Back",
                            Type="routine",
                            Action="I'm Back"
                        },
                        new ScreenItem()
                        {
                            Label="Goodbye",
                            Type="routine",
                            Action="Goodbye!"
                        }
                    }
                });
                Save(sd);
            }

            // Read the settings now
            using (StreamReader file = System.IO.File.OpenText(RepositoryFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                oai = (ScreenData)serializer.Deserialize(file, typeof(ScreenData));
            }

            return oai;
        }

        public static ScreenData Save(ScreenData oai)
        {
            using (StreamWriter file = System.IO.File.CreateText(RepositoryFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, oai);
            }

            return oai;
        }
    }
}
