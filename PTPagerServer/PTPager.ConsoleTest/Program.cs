using PTPager.Alerting.Interfaces;
using PTPager.Alerting.Polycom;
using PTPager.Alerting.Services;
using PTPager.Alerting.SystemSpeech;
using System;
using System.IO;

namespace PTPager.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            //SmartThingsService service = new SmartThingsService();
            //service.ListDevices().GetAwaiter().GetResult();
            
            SystemSpeechSynthesizer speech = new SystemSpeechSynthesizer();
            var ai = speech.Synthesize("Hello, i am working");

            //File.WriteAllBytes(@"d:\temp\test.wav", ai.AudioData);

            ISynthesizeSpeech synthesizeSpeech = new SystemSpeechSynthesizer();
            IAudioTransmitter audioTransmitter = new PolycomAudioTransmitter();

            AlertingService aservice = new AlertingService(synthesizeSpeech, audioTransmitter);
            aservice.Speak(3, "Testing 123");
        }
    }
}
