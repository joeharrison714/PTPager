using Newtonsoft.Json;
using PTPager.Web3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PTPager.Web3.Repository
{
    public class FileSpeechHistoryRepository
    {
        public static string RepositoryFile { get; set; } = "SpeechHistory.json";

        private static object _lock = new object();

        const int MaxItems = 100;

        public FileSpeechHistoryRepository()
        {
        }

        public void Save(SpeechHistoryItem item)
        {
            lock (_lock)
            {
                SpeechHistoryData shd = null;

                if (File.Exists(RepositoryFile) == false)
                {
                    shd = new SpeechHistoryData();
                }
                else
                {
                    using (StreamReader file = System.IO.File.OpenText(RepositoryFile))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        shd = (SpeechHistoryData)serializer.Deserialize(file, typeof(SpeechHistoryData));
                    }
                }

                shd.Items.Add(item);

                if (shd.Items.Count > MaxItems)
                {
                    shd.Items.OrderByDescending(p => p.Date).Take(MaxItems).ToList();
                }

                using (StreamWriter file = System.IO.File.CreateText(RepositoryFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(file, shd);
                }
            }
        }

        public IEnumerable<SpeechHistoryItem> GetAll()
        {
            lock (_lock)
            {
                SpeechHistoryData shd = null;

                if (File.Exists(RepositoryFile) == false)
                {
                    shd = new SpeechHistoryData();
                }
                else
                {
                    using (StreamReader file = System.IO.File.OpenText(RepositoryFile))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        shd = (SpeechHistoryData)serializer.Deserialize(file, typeof(SpeechHistoryData));
                    }
                }

                return shd.Items.OrderByDescending(p => p.Date).ToList();
            }
        }
    }
}
