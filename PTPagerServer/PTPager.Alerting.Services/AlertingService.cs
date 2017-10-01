using PTPager.Alerting.Interfaces;
using PTPager.Alerting.Model;
using System;

namespace PTPager.Alerting.Services
{
    public class AlertingService
    {
        private readonly ISynthesizeSpeech _synthesizeSpeech;
        private readonly IAudioTransmitter _audioTransmitter;

        public AlertingService(ISynthesizeSpeech synthesizeSpeech, IAudioTransmitter audioTransmitter)
        {
            _synthesizeSpeech = synthesizeSpeech;
            _audioTransmitter = audioTransmitter;
        }

        public void Speak(int channel, string text)
        {
            AudioInfo audioInfo = _synthesizeSpeech.Synthesize(text);

            _audioTransmitter.Transmit(channel, audioInfo);
        }
    }
}
