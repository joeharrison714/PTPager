using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTPager.Web3.Models
{
    public class ScreenData
    {
        public ScreenData()
        {
            Screens = new List<ScreenInfo>();
        }
        public List<ScreenInfo> Screens { get; set; }
    }

    public class ScreenInfo
    {
        public ScreenInfo()
        {
            Items = new List<ScreenItem>();
        }

        public string Id { get; set; }
        public string Label { get; set; }
        public List<ScreenItem> Items { get; set; }
    }

    public class ScreenItem
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
    }
}
