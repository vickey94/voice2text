using System;
using System.Windows.Forms;


namespace voice2text
{
    class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>  
        [STAThread]
        static void Main(string[] args)
        {


              
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Mainform());
       
        //      ActionListener AL = new ActionListener();
        //         AL.Monitor();
        //    AL.VoiceMonitor();
        //    Console.ReadKey();


    }



    }
}
