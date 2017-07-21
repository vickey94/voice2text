using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace voice2text
{
    class Program
    {
        static void Main(string[] args)
        {

              ActionListener AL = new ActionListener();
              AL.Monitor();
        //    AL.VoiceMonitor();
        //    Console.ReadKey();


        }



    }
}
