using System;
using System.Threading;
using voice2text.action;

namespace voice2text
{
    /// <summary>
    /// 这里主要实现线程控制录音，上传数据到服务器，获取结果，
    /// 类有有2个主要Listener，一个是启动后，获取到指令，开始进行识别
    /// 还有一个是本次识别会话监控Listener，超时，错误，结束，等状态，结束本次识别，可以让后续识别不受影响
    /// </summary>
    class ActionListener
    {

        private AudioAction audio;
        private MSCAction msc;
        /// <summary>
        /// 主监控，控制指令识别开始
        /// </summary>
        public void Monitor()
        {

            //使用其他接口前必须先调用MSPLogin，可以在应用程序启动时调用。
            int ret = MSCDll.MSPLogin(null, null, "appid=5964bde1");
            if (ret == 0) Console.WriteLine(Util.getNowTime() + " 讯飞语音会话登录成功！");
            else throw new Exception("MSPLogin失败 errCode=" + ret);

            StartSession();


        }

        /// <summary>
        /// 用于结束本次会话
        /// </summary>
        public void SessionMonitor()
        {

            while (true)
            {
                Thread.Sleep(1000); //每1000ms监控一次 ，这里要注意，防止数据还未获取完就结束线程！
                string res = msc.getNowResult();

                Console.WriteLine(Util.getNowTime() + " 获取结果：" + res);
                int status = msc.getNowEp_status();
                if (status == 3)
                {

                    StopSession();
                    Console.WriteLine(Util.getNowTime() + " 端点结束" + res);
                    Console.ReadKey();
                }

                if (res.Length > 10)
                {
                    StopSession();

                    Console.WriteLine(Util.getNowTime() + " 输入数据值达到上限，结束本次会话，最后结果为：" + res);
                    Console.ReadKey();
                    //  Thread.Sleep(100000);

                }


            }


        }

        private Thread audio_record;

        private Thread msc_result;

        private Thread session_monitor;

        private Thread msc_audiofile;

        private string outputPath = "";

        private void StartSession()
        {

            msc = new MSCAction();

            msc.SessionBegin(null);

            audio = new AudioAction();
            audio.init(msc);

            outputPath = audio.outputPath; //获取本次文件地址

            //录音，上传
            audio_record = new Thread(new ThreadStart(audio.StartRecordingHandler));

            //获取结果
            msc_result = new Thread(new ThreadStart(msc.getResultHandler));

            //用于监控，结束另外2个线程
            session_monitor = new Thread(new ThreadStart(SessionMonitor));

            audio_record.Start();
            msc_result.Start();
            session_monitor.Start();

        }

        

        private void StopSession()
        {

            audio_record.Abort();
            msc_result.Abort();

            audio.StopRecording();
            msc.SessionEnd();
        //    MSCDll.MSPLogout();

            audio = null;
            msc = null;

            Console.WriteLine("再次验证开始！"); 
            //StartSessionAgain();

            ASR();

        }

        private void StartSessionAgain()
        {
            msc = new MSCAction();
            msc.SessionBegin(null);
            msc.SetINFILE(outputPath);

            msc_audiofile = new Thread(new ThreadStart(msc.AudioWriteFile));
            msc_audiofile.Start();

            //获取结果
            msc_result = new Thread(new ThreadStart(msc.getResultHandler));
            msc_result.Start();

        }

        public void ASR()
        {
            msc = new MSCAction();

        /*    int ret = MSCDll.MSPLogin(null, null, "appid=5964bde1");
            if (ret == 0) Console.WriteLine(Util.getNowTime() + " 讯飞语音会话登录成功！");
            else throw new Exception("MSPLogin失败 errCode=" + ret);
*/

            msc.UploadData(@"C:\Users\admin\Desktop\xunfei\abnf\keynumber2.abnf");

            string param = "sub = asr, result_type = plain, sample_rate = 16000,aue = speex-wb,ent=sms16k";
            msc.SessionBegin(param);
            msc.SetINFILE(outputPath);

            audio_record = new Thread(new ThreadStart(msc.AudioWriteFile));
            audio_record.Start();
 

            //获取结果
            msc_result = new Thread(new ThreadStart(msc.getResultHandler));
            msc_result.Start();

        }
    }
}
