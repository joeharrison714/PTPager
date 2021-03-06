﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PTPager.Alerting.Polycom.Configuration
{
	public class PolycomAudioTransmitterConfiguration
	{
		public string BindingIp { get; set; }

		public int SleepBetweenAlertPackets { get; set; }
		public int SleepBetweenPackets { get; set; }
		public int SleepBeforeEndPackets { get; set; }
		public int SleepBetweenEndPackets { get; set; }
	}
}
