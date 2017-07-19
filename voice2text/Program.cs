using System;
using System.Collections.Generic;
using System.Linq;
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
        }
    }
}
