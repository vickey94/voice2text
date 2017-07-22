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



        private Thread msc_Uploadfile;

        private Thread msc_result;


        ///用于监控，结束另外2个线程
        private Thread session_monitor;
      



      
        AudioAction vMonitor;
        /// <summary>
        /// 主监控，控制指令识别开始
        /// </summary>
        public void Monitor()
        {


            MSPLogin();


            VoiceMonitor();

           

            ///捕获到条件，开始传送数据
            while (true)
            {

                Thread.Sleep(10);
              
                //   Console.WriteLine(Config.start);
                if (Config.isSpeeking)
                {
                    vMonitor.StopMonitoring();
                    //    vMonitor.StopMonitoring();
                    Config.isSpeeking = false;

                  

                    ///开启
                    StartSession_IAT();

                    ///开启监控本次Session结束
                    session_monitor = new Thread(new ThreadStart(SessionMonitor));
                    session_monitor.Start();
                }
           }
       
             


        }

        ~ActionListener()
        {
            MSCDll.MSPLogout();
        }

        public void VoiceMonitor()
        {
            vMonitor = vMonitor = new AudioAction();
            vMonitor.initMonitor();
            vMonitor.StartMonitoringHandler();

         //   voice_monitor = new Thread(new ThreadStart(vMonitor.StartMonitoringHandler));

         //   voice_monitor.Start();
          
    }
        string res;
        /// <summary>
        /// 用于结束本次会话，同时让主线程继续循环
        /// </summary>
        public void SessionMonitor()
        {

            
            while (true)
            {
                Thread.Sleep(500); //每1000ms监控一次 ，这里要注意，防止数据还未获取完就结束线程！
                if(msc!=null)
                res = msc.getNowResult();

             //   Console.WriteLine(Util.getNowTime() + " 获取结果：" + res);
                int status = msc.getNowEp_status();

                if (msc != null && audio != null)
                {
                    if (status == 3)
                    {

                        StopSession_IAT();
                        Console.WriteLine(Util.getNowTime() + " 端点结束" + res);

                      
                        Console.WriteLine("数据不合格！");
                        //   StartSession_ASR();


                        Thread.Sleep(200);
                        vMonitor.StartMonitoringHandler();
                        //  VoiceMonitor();
                        session_monitor.Abort();///结束本身线程
                    }

                    if (res.Length > 6)
                    {
                        StopSession_IAT();

                        Console.WriteLine(Util.getNowTime() + " 输入数据值达到上限，结束本次会话，最后结果为：" + res);

                        Console.WriteLine("再次验证开始！");
                        StartSession_ASR();


                        //  Thread.Sleep(100000);


                    }
                }
                if(msc != null && audio == null)
                {
                    if (msc.getNowResultStatus() == 1)
                    {
                        Console.WriteLine("再次验证结束！");
                        StopSession_ASR();

                   
             
                        Thread.Sleep(200);
                        vMonitor.StartMonitoringHandler();
                        //  VoiceMonitor();
                        session_monitor.Abort();///结束本身线程


                    }
                }

            }


        }



    
        private string outputPath = "";

        private void StartSession_IAT()
        {

            msc = new MSCAction();


            msc.SessionBegin(null,Config.PARAMS_SESSION_IAT);

            audio = new AudioAction();
            audio.init(msc);

            outputPath = audio.outputPath; //获取本次文件地址

            //录音，上传
            audio.StartRecordingHandler();
            //   audio_record = new Thread(new ThreadStart(audio.StartRecordingHandler));

            //获取结果
            msc_result = new Thread(new ThreadStart(msc.getResultHandler));

      

        //    audio_record.Start();
            msc_result.Start();
          

        }

        

        private void StopSession_IAT()
        {

        
            msc_result.Abort();

            audio.StopRecording();
            msc.SessionEnd();
        //    MSCDll.MSPLogout();

            audio = null;
            msc = null;

        }



        /// <summary>
        /// 语法验证是直接传入之前的音频文件，不需要录音,
        /// </summary>
        public void StartSession_ASR()
        {
            msc = new MSCAction();
            msc.UploadData(@"..\res\keynumber2.abnf");
            //  msc.UploadData(@"C:\Users\admin\Desktop\xunfei\abnf\keynumber2.abnf");

            Console.WriteLine("msc.getGrammarList_temp() is "+ msc.getGrammarList_temp());
          //  string param = "sub = asr, result_type = plain, sample_rate = 16000,aue = speex-wb,ent=sms16k";
            msc.SessionBegin(msc.getGrammarList_temp(),Config.PARAMS_SESSION_ASR);

            Console.WriteLine("文件地址为："+ outputPath);
            ///设置文件地址
            msc.SetINFILE(outputPath);

          
            msc_Uploadfile = new Thread(new ThreadStart(msc.AudioWriteFile));
            msc_Uploadfile.Start();
 

            ///获取结果
            msc_result = new Thread(new ThreadStart(msc.getResultHandler));
            msc_result.Start();

        }

        private void StopSession_ASR()
        {

            msc_Uploadfile.Abort();
            msc_result.Abort();
         
            msc.SessionEnd();     
 
            msc = null;

        }




        /// <summary>
        /// 全局监控开始调用登录
        /// </summary>
        private void MSPLogin()
        {
            //使用其他接口前必须先调用MSPLogin，可以在应用程序启动时调用。
            int ret = MSCDll.MSPLogin(null, null, Config.PARAMS_LOGIN);
            if (ret == 0) Console.WriteLine(Util.getNowTime() + " 讯飞语音会话登录成功！");
            else throw new Exception("MSPLogin失败 errCode=" + ret);
        }
    }
}
