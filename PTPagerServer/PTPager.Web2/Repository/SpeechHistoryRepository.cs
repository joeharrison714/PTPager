using PTPager.Web2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTPager.Web2.Repository
{
    public class SpeechHistoryRepository
    {
        private static object _lock = new object();
        private static List<SpeechHistoryItem> _data = new List<SpeechHistoryItem>();

        const int MaxItems = 100;

        public SpeechHistoryRepository()
        {
        }

        public void Save(SpeechHistoryItem item)
        {
            lock (_lock)
            {
                _data.Add(item);

                if (_data.Count > 100)
                {
                    _data = _data.OrderByDescending(p => p.Date).Take(MaxItems).ToList();
                }
            }
        }

        public IEnumerable<SpeechHistoryItem> GetAll()
        {
            lock (_lock)
            {
                return _data.OrderByDescending(p => p.Date).ToList();
            }
        }
    }
}
