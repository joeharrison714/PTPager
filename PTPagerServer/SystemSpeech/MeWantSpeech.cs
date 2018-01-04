using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace SystemSpeech
{
    public class MeWantSpeech
    {
        SpeechSynthesizer _speechSynthesizer;
        SpeechAudioFormatInfo _formatInfo;

        public MeWantSpeech()
        {
            // convert text to audio stream using .net 3.x speechsynthesis g.711 u-law (pcm 64kb/s bit rate (u-law encodes 14-bit to 8-bit samples by adding 32 / binary 100000)
            _speechSynthesizer = new SpeechSynthesizer();

            // select (if it exists)
            _speechSynthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);

            // can also change voice with SelectVoice method
            _speechSynthesizer.Rate = 1;


            // encoding format enums are Pcm, ALaw, ULaw
            int samplesPerSecond = 8000;
            int bitsPerSample = 8;
            // System.Speech.AudioFormat.AudioBitsPerSample.Eight
            int channelCount = 1;
            // System.Speech.AudioFormat.AudioChannel.Mono
            int averageBytesPerSecond = 20;
            int blockAlign = 2;
            byte[] formatSpecificData = null;

            _formatInfo = new SpeechAudioFormatInfo(EncodingFormat.ULaw, samplesPerSecond, bitsPerSample, channelCount, averageBytesPerSecond, blockAlign, formatSpecificData);
        }

        public void GenerateToFile(string text, string filename)
        {
            _speechSynthesizer.SetOutputToWaveFile(filename, _formatInfo);

            _speechSynthesizer.Speak(text);
        }

        public byte[] GenerateToByteArray(string text)
        {
            byte[] result;

            using (MemoryStream streamDestination = new MemoryStream())
            {
                var speechSynthesizer = new SpeechSynthesizer();

                speechSynthesizer.SetOutputToAudioStream(streamDestination, _formatInfo);

                speechSynthesizer.Speak(text);

                result = streamDestination.ToArray();
            }

            return result;
        }
    }
}
