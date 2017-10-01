using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemSpeech
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (args == null || args.Length != 2)
                {
                    throw new Exception("Incorrect number of arguments given");
                }

                string text = args[0];
                string filename = args[1];

                if (System.IO.File.Exists(filename))
                {
                    System.IO.File.Delete(filename);
                }

                MeWantSpeech meWantSpeech = new MeWantSpeech();
                meWantSpeech.GenerateToFile(text, filename);

                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }
        }
    }
}
