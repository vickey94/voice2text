using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using voice2text.action;

namespace voice2text
{
    public partial class Mainform : Form
    {
        private MSCAction msc;
        private AudioAction audio;
        private string outputPath;


        public void setProgressBar(int value)
        {
            progressBar1.Invoke(new MethodInvoker(delegate ()
            {
                progressBar1.Value = value;
            }));
               
        }
        public Mainform()
        {
            InitializeComponent();
            Util.MSPLogin();
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {

         



            string text = Util.getNowTime()+" 鼠标已经按下，正在录音\n";
         
            richTextBox1.AppendText(text);


            msc = new MSCAction();
            msc.SessionBegin(null, Config.PARAMS_SESSION_IAT);
            audio = new AudioAction();
            audio.setMForm(this);
            audio.init(null);
            
            audio.StartRecordingHandler();
            outputPath = audio.outputPath;
         
            Console.WriteLine(outputPath);
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {

            string text = Util.getNowTime() + " 鼠标已经松开，停止录音\n";
            richTextBox1.AppendText(text);
            

            audio.StopRecording();


            msc.SetINFILE(outputPath);

            msc.AudioWriteFile();


            msc.getResultHandler();

            
            richTextBox1.AppendText(msc.getNowResult()+"\n");
            msc.SessionEnd();
            richTextBox1.ScrollToCaret();
        }
    }
}
