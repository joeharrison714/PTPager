using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTPager.Web2.Models
{
    public class SpeechHistoryData
    {
        public SpeechHistoryData()
        {
            Items = new List<SpeechHistoryItem>();
        }
        public List<SpeechHistoryItem> Items { get; set; }
    }

    public class SpeechHistoryItem
    {
        public DateTime Date { get; set; }
        public string Speech { get; set; }
        public int Channel { get; set; }
    }
}
