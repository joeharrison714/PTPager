using Amazon.Polly;
using Amazon.Polly.Model;
using Microsoft.Extensions.Options;
using NAudio.FileFormats.Mp3;
using NAudio.Wave;
using PTPager.Alerting.Interfaces;
using PTPager.Alerting.Model;
using PTPager.Alerting.PollySpeech.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PTPager.Alerting.PollySpeech
{
	public class PollySpeechSynthesizer : ISynthesizeSpeech
	{
        VoiceId voiceId = VoiceId.Salli;

        public PollySpeechSynthesizer(IOptions<PollySpeechSynthesizerConfiguration> configOptions)
        {
            if (configOptions != null && configOptions.Value != null)
            {
                voiceId = VoiceId.FindValue(configOptions.Value.VoiceId);
            }
        }

		public AudioInfo Synthesize(string text)
		{
            Dictionary<uint, byte[]> audioBytes = new Dictionary<uint, byte[]>();

            var ulawFormat = WaveFormat.CreateMuLawFormat(8000, 1);

            string tempFile = Path.GetTempFileName();

            try
            {
                using (AmazonPollyClient pc = new AmazonPollyClient())
                {

                    SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
                    sreq.Text = text + "...";
                    sreq.OutputFormat = OutputFormat.Mp3;
                    sreq.VoiceId = VoiceId.Salli;
                    SynthesizeSpeechResponse sres = pc.SynthesizeSpeechAsync(sreq).GetAwaiter().GetResult();


                    using (var pollyMemoryStream = new MemoryStream())
                    {
                        sres.AudioStream.CopyTo(pollyMemoryStream);
                        pollyMemoryStream.Flush();

                        pollyMemoryStream.Position = 0;

                        using (Mp3FileReader reader = new Mp3FileReader(pollyMemoryStream, wave => new DmoMp3FrameDecompressor(wave)))
                        {
                            using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                            {
                                WaveFileWriter.CreateWaveFile(tempFile, pcmStream);
                            }
                        }
                    }

                }


                var pcmFormat = new WaveFormat(8000, 16, 1);

                List<byte[]> allBytes = new List<byte[]>();

                using (WaveFormatConversionStream pcmStm = new WaveFormatConversionStream(pcmFormat, new WaveFileReader(tempFile)))
                {
                    using (WaveFormatConversionStream ulawStm = new WaveFormatConversionStream(ulawFormat, pcmStm))
                    {
                        byte[] buffer = new byte[160];
                        int bytesRead = ulawStm.Read(buffer, 0, 160);

                        while (bytesRead > 0)
                        {
                            byte[] sample = new byte[bytesRead];
                            Array.Copy(buffer, sample, bytesRead);
                            allBytes.Add(sample);

                            bytesRead = ulawStm.Read(buffer, 0, 160);
                        }

                        int secondsToAdd = 1;
                        var silentBytes = new byte[ulawStm.WaveFormat.AverageBytesPerSecond * secondsToAdd];
                        allBytes.Add(silentBytes);
                    }
                }

                AudioInfo ai = new AudioInfo()
                {
                    AudioData = allBytes.SelectMany(p => p).ToArray(),
                    Codec = Codec.G711U
                };

                return ai;

                //string fileName = @".\TestAudio\output-from-polly-mp3-then-wav.wav";
                //using (WaveFileWriter writer = new WaveFileWriter(fileName, ulawFormat))
                //{
                //    var testSequence = audioBytes.SelectMany(p => p.Value).ToArray();
                //    writer.Write(testSequence, 0, testSequence.Length);
                //}
            }
            finally
            {
                try
                {
                    File.Delete(tempFile);
                }
                catch { }
            }
        }
	}
}
