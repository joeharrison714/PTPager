using PTPager.Alerting.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTPager.Alerting.Interfaces
{
    public interface IAudioTransmitter
    {
        void Transmit(int channel, AudioInfo audioInfo);
    }
}
