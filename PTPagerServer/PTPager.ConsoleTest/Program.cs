﻿using Amazon.Polly;
using Amazon.Polly.Model;
using Microsoft.Extensions.Options;
using NAudio.FileFormats.Mp3;
using NAudio.Wave;
using PTPager.Alerting.Interfaces;
using PTPager.Alerting.Polycom;
using PTPager.Alerting.Polycom.Configuration;
using PTPager.Alerting.Services;
using PTPager.Alerting.SystemSpeech;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PTPager.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //SmartThingsService service = new SmartThingsService();
            //service.ListDevices().GetAwaiter().GetResult();



            //TestMp3();
            //TestWav();
            TestPolly3();
        }

        public static void TestPolly()
        {
            using (AmazonPollyClient pc = new AmazonPollyClient())
            {

                SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
                sreq.Text = "Your Sample Text Here 123";
                sreq.OutputFormat = OutputFormat.Mp3;
                sreq.VoiceId = VoiceId.Salli;
                SynthesizeSpeechResponse sres = pc.SynthesizeSpeechAsync(sreq).GetAwaiter().GetResult();

                using (var fileStream = File.Create(@".\TestAudio\output-from-polly.mp3"))
                {
                    sres.AudioStream.CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
        }

        public static void TestPolly2()
        {
            using (AmazonPollyClient pc = new AmazonPollyClient())
            {

                SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
                sreq.Text = "Your Sample Text Here 123";
                sreq.OutputFormat = OutputFormat.Mp3;
                sreq.VoiceId = VoiceId.Salli;
                SynthesizeSpeechResponse sres = pc.SynthesizeSpeechAsync(sreq).GetAwaiter().GetResult();

                using (var memoryStream = new MemoryStream())
                {
                    sres.AudioStream.CopyTo(memoryStream);
                    memoryStream.Flush();

                    memoryStream.Position = 0;

                    string outputFile = @".\TestAudio\output-from-polly.wav";

                    using (Mp3FileReader reader = new Mp3FileReader(memoryStream))
                    {
                        using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                        {
                            WaveFileWriter.CreateWaveFile(outputFile, pcmStream);
                        }
                    }
                }
            }
        }

        public static void TestPolly3()
        {
            Dictionary<uint, byte[]> audioBytes = new Dictionary<uint, byte[]>();

            var ulawFormat = WaveFormat.CreateMuLawFormat(8000, 1);

            string tempFile = Path.GetTempFileName();

            try
            {
                using (AmazonPollyClient pc = new AmazonPollyClient())
                {

                    SynthesizeSpeechRequest sreq = new SynthesizeSpeechRequest();
                    sreq.Text = "Something something something";
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
                                //WaveFileWriter.WriteWavFileToStream(pcmWaveStream, pcmStream);
                                WaveFileWriter.CreateWaveFile(tempFile, pcmStream);
                            }
                        }
                    }

                }


                var pcmFormat = new WaveFormat(8000, 16, 1);
                uint timestamp = 0;

                using (WaveFormatConversionStream pcmStm = new WaveFormatConversionStream(pcmFormat, new WaveFileReader(tempFile)))
                {
                    using (WaveFormatConversionStream ulawStm = new WaveFormatConversionStream(ulawFormat, pcmStm))
                    {
                        string fileName = @".\TestAudio\output-from-polly-mp3-then-wav.wav";
                        WaveFileWriter.CreateWaveFile(fileName, ulawStm);
                        //byte[] buffer = new byte[160];
                        //int bytesRead = ulawStm.Read(buffer, 0, 160);

                        //while (bytesRead > 0)
                        //{
                        //    byte[] sample = new byte[bytesRead];
                        //    Array.Copy(buffer, sample, bytesRead);
                        //    audioBytes.Add(timestamp, sample);
                        //    timestamp += 160;

                        //    bytesRead = ulawStm.Read(buffer, 0, 160);
                        //}
                    }
                }

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

            //string fileName = @".\TestAudio\output-from-polly-mp3-then-wav.wav";
            //using (WaveFileWriter writer = new WaveFileWriter(fileName, ulawFormat))
            //{
            //    var testSequence = audioBytes.SelectMany(p => p.Value).ToArray();
            //    writer.Write(testSequence, 0, testSequence.Length);
            //}
        }

        private static void TestWav()
        {
            Dictionary<uint, byte[]> audioBytes = new Dictionary<uint, byte[]>();

            uint timestamp = 0;

            string file = @".\TestAudio\output-from-mp3.wav";
            var pcmFormat = new WaveFormat(8000, 16, 1);
            var ulawFormat = WaveFormat.CreateMuLawFormat(8000, 1);

            using (WaveFormatConversionStream pcmStm = new WaveFormatConversionStream(pcmFormat, new WaveFileReader(file)))
            {
                using (WaveFormatConversionStream ulawStm = new WaveFormatConversionStream(ulawFormat, pcmStm))
                {
                    byte[] buffer = new byte[160];
                    int bytesRead = ulawStm.Read(buffer, 0, 160);

                    while (bytesRead > 0)
                    {
                        byte[] sample = new byte[bytesRead];
                        Array.Copy(buffer, sample, bytesRead);
                        //m_rtpChannel.AddSample(sample);
                        audioBytes.Add(timestamp, sample);
                        timestamp += 160;

                        bytesRead = ulawStm.Read(buffer, 0, 160);
                    }
                }
            }

            string fileName = @".\TestAudio\output-from-wav.wav";
            using (WaveFileWriter writer = new WaveFileWriter(fileName, ulawFormat))
            {
                var testSequence = audioBytes.SelectMany(p => p.Value).ToArray();
                writer.Write(testSequence, 0, testSequence.Length);
            }
        }

        private static void TestMp3()
        {

            string mp3File = @".\TestAudio\speech_20200512013308591.mp3";
            string outputFile = @".\TestAudio\output-from-mp3.wav";
            using (Mp3FileReader reader = new Mp3FileReader(mp3File))
            {
                using (WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader))
                {
                    WaveFileWriter.CreateWaveFile(outputFile, pcmStream);
                }
            }
            return;

            Dictionary<uint, byte[]> audioBytes = new Dictionary<uint, byte[]>();

            uint timestamp = 0;

            string file = @".\TestAudio\speech_20200512013308591.mp3";
            //var pcmFormat = new WaveFormat(8000, 16, 1);
            var ulawFormat = WaveFormat.CreateMuLawFormat(8000, 1);

            

            using (var pcmStm = WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(file)))
            {
                using (WaveFormatConversionStream ulawStm = new WaveFormatConversionStream(ulawFormat, pcmStm))
                {
                    byte[] buffer = new byte[160];
                    int bytesRead = ulawStm.Read(buffer, 0, 160);

                    while (bytesRead > 0)
                    {
                        byte[] sample = new byte[bytesRead];
                        Array.Copy(buffer, sample, bytesRead);
                        //m_rtpChannel.AddSample(sample);
                        audioBytes.Add(timestamp, sample);
                        timestamp += 160;

                        bytesRead = ulawStm.Read(buffer, 0, 160);
                    }
                }
            }

            //WaveFileWriter.CreateWaveFile(tempFile, WaveP);
            string fileName = @".\TestAudio\output.wav";
            using (WaveFileWriter writer = new WaveFileWriter(fileName, ulawFormat))
            {
                var testSequence = audioBytes.SelectMany(p => p.Value).ToArray();
                writer.Write(testSequence, 0, testSequence.Length);
            }
        }

        public void BasicTest()
        {
            var config = Options.Create<PolycomAudioTransmitterConfiguration>(new PolycomAudioTransmitterConfiguration()
            {
                BindingIp = "10.1.10.54"
            });

            ISynthesizeSpeech synthesizeSpeech = new SystemSpeechSynthesizer();
            IAudioTransmitter audioTransmitter = new PolycomAudioTransmitter(config);

            AlertingService aservice = new AlertingService(synthesizeSpeech, audioTransmitter);
            aservice.Speak(3, "Whatcha lookin at");
        }

        public void TestSpeech()
        {
            SystemSpeechSynthesizer speech = new SystemSpeechSynthesizer();
            var ai = speech.Synthesize("Hello, i am working");

            //File.WriteAllBytes(@"d:\temp\test.wav", ai.AudioData);
        }
    }
}
