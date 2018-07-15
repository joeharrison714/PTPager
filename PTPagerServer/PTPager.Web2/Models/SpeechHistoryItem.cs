using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTPager.Web2.Models
{
    public class SpeechHistoryItem
    {
        public DateTime Date { get; set; }
        public string Speech { get; set; }
        public int Channel { get; set; }
    }
}
