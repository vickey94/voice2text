using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using voice2text.action;
using voice2text.model;

namespace voice2text
{
    public partial class Mainform : Form
    {
 



        public Mainform()
        {
            InitializeComponent();
            init();
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            TrainingStart();
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            TrainingStop();
        }


        public void setProgressBar(int value)
        {
            progressBar1.Invoke(new MethodInvoker(delegate ()
            {
                progressBar1.Value = value;
            }));

        }

        private MSCAction msc;
        private AudioAction audio;
        private Train train;

        /// <summary>
        /// 创建文件夹
        /// </summary>
        public void init()
        {
          
            ///本次输出文件夹          
            Config.outputFolder = "D:\\" + Util.getNowTime();

            if (Directory.Exists((Config.outputFolder)) == false)//如果不存在就创建文件夹
            {
                Directory.CreateDirectory(Config.outputFolder);   //本次文件夹
                File.Create(Config.outputFolder + "\\" + Config.outputTXT).Close(); //本次语音记录
                Directory.CreateDirectory(Config.outputFolder + "\\wav");  //本次语音文件
            }

            Util.MSPLogin(); //登录

        }

        /// <summary>
        /// 一次训练开始
        /// </summary>
        public void TrainingStart()
        {
            string text = Util.getNowTime() + " 鼠标已经按下，正在录音\n";

            richTextBox1.AppendText(text);

            msc = new MSCAction();
            audio = new AudioAction();
            train = new Train();

            train.setTime(Util.getNowTime());

            msc.SessionBegin(null, Config.PARAMS_SESSION_IAT);
            audio.init(null, Config.outputFolder + "\\wav\\" + train.getTime() + ".wav", this);

            audio.StartRecordingHandler();
        }
        /// <summary>
        /// 训练结束
        /// </summary>
        private void TrainingStop()
        {

            string text = Util.getNowTime() + " 鼠标已经松开，停止录音\n";
            richTextBox1.AppendText(text);

            audio.StopRecording();

            msc.SetINFILE(Config.outputFolder + "\\wav\\" + train.getTime() + ".wav");

            //此处要考虑线程问题
            msc.AudioWriteFile();


            try
            {
                msc.getResultHandler();
                text = msc.getNowResult();
            }
            catch (Exception exp)
            {
                if (exp.Message == "QISRAudioWrite err,errCode=10118")
                {
                    text = "录音效果不好，请再次尝试\n";
                 
                }
            }
            finally
            {
                if(text == "") text = "录音效果不好，请再次尝试\n"; 
                richTextBox1.AppendText(text);
                msc.SessionEnd();
                richTextBox1.ScrollToCaret();
                train.setId(textBox1.Text);
                train.setText(text);
                Write2TXT(train.getWriteText());
                setProgressBar(0);
            }
           

        }

        private void Write2TXT(string text)
        {
            FileStream fs = new FileStream(Config.outputFolder + "\\" + Config.outputTXT, FileMode.Append);
            //获得字节数组
            byte[] data = Encoding.UTF8.GetBytes(text + "\r\n");
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ActionListener AL = new ActionListener();
                   AL.Monitor();
          
        }

    
    }
}
