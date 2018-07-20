using Newtonsoft.Json;
using PTPager.Web2.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PTPager.Web2.Repository
{
    public class RoutineOrderRepository
    {
        public static string RepositoryFile { get; set; } = "RoutineOrder.json";

        public static RoutineOrder Get()
        {
            RoutineOrder oai = null;

            if (File.Exists(RepositoryFile) == false)
                return new RoutineOrder();

            // Read the settings now
            using (StreamReader file = System.IO.File.OpenText(RepositoryFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                oai = (RoutineOrder)serializer.Deserialize(file, typeof(RoutineOrder));
            }

            return oai;
        }

        public static RoutineOrder Save(RoutineOrder oai)
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
