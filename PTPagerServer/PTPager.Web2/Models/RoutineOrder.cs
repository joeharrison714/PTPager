using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTPager.Web2.Models
{
    public class RoutineOrder
    {
        public RoutineOrder()
        {
            Items = new List<RoutineOrderItem>();
        }
        public List<RoutineOrderItem> Items { get; set; }
    }

    public class RoutineOrderItem
    {
        public string Label { get; set; }
        public bool Visible { get; set; }
        public int Order { get; set; }
    }
}
