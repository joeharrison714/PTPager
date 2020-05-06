using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PTPager.Web2.Repository
{
	public class FileSnoozeService
	{
		public static string RepositoryFile { get; set; } = "Snooze.json";

		private static object _lock = new object();

		public DateTime? GetSnoozedUntil()
		{
			var sd = Load();

			if (sd == null || !sd.SnoozeUntil.HasValue) return null;

			if (sd.SnoozeUntil.Value > DateTime.Now)
			{
				return sd.SnoozeUntil.Value;
			}

			return null;
		}

		public bool IsSnoozed()
		{
			var d = GetSnoozedUntil();

			return d != null;
		}

		public void Snooze(int minutes) {
			var su = DateTime.Now.AddMinutes(minutes);

			SnoozeData sd = new SnoozeData()
			{
				SnoozeUntil = su
			};

			Save(sd);
		}

		public void CancelSnooze()
		{
			SnoozeData sd = new SnoozeData()
			{
				SnoozeUntil = null
			};

			Save(sd);
		}

		private void Save(SnoozeData data)
		{
			lock (_lock)
			{
				using (StreamWriter file = System.IO.File.CreateText(RepositoryFile))
				{
					JsonSerializer serializer = new JsonSerializer();
					serializer.Formatting = Formatting.Indented;
					serializer.Serialize(file, data);
				}
			}
		}

		private SnoozeData Load()
		{
			lock (_lock)
			{
				SnoozeData shd = null;

				if (File.Exists(RepositoryFile) == false)
				{
					shd = new SnoozeData();
				}
				else
				{
					using (StreamReader file = System.IO.File.OpenText(RepositoryFile))
					{
						JsonSerializer serializer = new JsonSerializer();
						shd = (SnoozeData)serializer.Deserialize(file, typeof(SnoozeData));
					}
				}

				return shd;
			}
		}
	}

	internal class SnoozeData
	{
		public DateTime? SnoozeUntil{ get; set; }
	}
}
