using PTPager.Alerting.Interfaces;
using PTPager.Alerting.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PTPager.Alerting.SystemSpeech
{
    public class SystemSpeechSynthesizer : ISynthesizeSpeech
    {
        public AudioInfo Synthesize(string text)
        {
            FileInfo fi = new FileInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), ".\\exe\\systemspeech.exe"));
            if (!fi.Exists)
            {
                throw new Exception("Unable to find systemspeech.exe");
            }

            string tempFilename = Path.GetTempFileName();
            string args = string.Format("\"{0}\" \"{1}\"", text, tempFilename);

            Process p = Process.Start(fi.FullName, args);
            p.WaitForExit();

            if (p.ExitCode == 1)
            {
                throw new Exception("systemspeech.exe failed");
            }

            byte[] allBytes = File.ReadAllBytes(tempFilename);

            File.Delete(tempFilename);

            AudioInfo ai = new AudioInfo()
            {
                AudioData = allBytes,
                Codec = Codec.G711U
            };

            return ai;
        }
    }
}
