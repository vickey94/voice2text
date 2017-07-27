using System;
using System.Threading;
using voice2text.action;
using voice2text.model;

namespace voice2text
{
    /// <summary>
    /// 这里主要实现线程控制录音，上传数据到服务器，获取结果，
    /// 类有有2个主要Listener，一个是启动后，获取到指令，开始进行识别
    /// 还有一个是本次识别会话监控Listener，超时，错误，结束，等状态，结束本次识别，可以让后续识别不受影响
    /// </summary>
    class ActionListener
    {
        private Mainform mform;

        private AudioAction audio;
        private MSCAction msc;

        private Auth auth; ///保存维持一次运行中的语音识别信息



        private Thread msc_Uploadfile;

        private Thread msc_result;


        ///用于监控，结束另外2个线程
        private Thread session_monitor;
      
     
        /// <summary>
        /// 环境声音监控
        /// </summary>
        private AudioAction voice_Monitor;

        string res;

        public ActionListener(Mainform mform) { this.mform = mform; }
        /// <summary>
        /// 主监控，控制指令识别开始
        /// </summary>
        public void Monitor()
        {


          //  Util.MSPLogin(); //登录，所有的开始


            ///创建监控
            voice_Monitor = voice_Monitor = new AudioAction();
            voice_Monitor.initMonitor();
            voice_Monitor.StartMonitoringHandler(); //自带线程



            ///捕获到条件，开始传送数据
            while (true)
            {

                Thread.Sleep(50);
              
                //   Console.WriteLine(Config.start);
                if (Config.isSpeeking)
                {
                    voice_Monitor.StopMonitoring();
                    //    vMonitor.StopMonitoring();
                    Config.isSpeeking = false;

                 
                    ///开启连续语音识别
                    StartSession_IAT();

                    ///开启监控本次Session
                    session_monitor = new Thread(new ThreadStart(SessionMonitor));
                    session_monitor.Start();
                }
           }
       
             


        }

        ~ActionListener()
        {
            MSCDll.MSPLogout();
        }

      
        /// <summary>
        /// 用于结束本次会话，同时让主线程继续循环
        /// </summary>
        public void SessionMonitor()
        {
            int i = 0;
            while (true)
            {
                i++;
                if (i > 5)
                { StopSession_IAT();
                    Console.WriteLine(Util.getNowTime() + " 端点结束" + res);

                    Console.WriteLine("数据不合格！请再次尝试");

                    Thread.Sleep(100);
                    voice_Monitor.StartMonitoringHandler();

                    session_monitor.Abort();///结束本身线程

                }

                    Thread.Sleep(500); //每500ms监控一次 ，这里要注意，防止数据还未获取完就结束线程！

                if (msc!=null)  res = msc.getNowResult();

             //   Console.WriteLine(Util.getNowTime() + " 获取结果：" + res);
                int status = msc.getNowEp_status();

                ///msc ,audio 都不为空为连续语音识别情况（IAT）
                if (msc != null && audio != null)
                {
                    if (status == 3)
                    {
                        StopSession_IAT();
                        Console.WriteLine(Util.getNowTime() + " 端点结束" + res);
                    
                        Console.WriteLine("数据不合格！请再次尝试");                    

                        Thread.Sleep(100);
                        voice_Monitor.StartMonitoringHandler();
                      
                        session_monitor.Abort();///结束本身线程
                    }

                    if (res.Length > 6) //这里验证连续语音识别结束条件
                    {
                        StopSession_IAT();

                        Console.WriteLine(Util.getNowTime() + " 输入数据值达到上限，结束本次会话，最后结果为：" + res);

                        Console.WriteLine("ASR再次验证开始！");
                        StartSession_ASR();


                    }
                }
                if(msc != null && audio == null) ///这里为ASR情况的验证结束
                {
                    if (msc.getNowResultStatus() == 1)
                    {
                      
                        StopSession_ASR();
                        Console.WriteLine("再次验证结束！");

                        Thread.Sleep(200);
                        voice_Monitor.StartMonitoringHandler(); ///再次开启环境监听
                      
                        session_monitor.Abort();///结束本身线程


                    }
                }

            }

        }



    
     //   private string outputPath = "";

        /// <summary>
        /// 连续语音识别开始
        /// </summary>
        private void StartSession_IAT()
        {
            auth = new Auth();
            auth.setTime(Util.getNowTime());

            msc = new MSCAction();


            msc.SessionBegin(null,Config.PARAMS_SESSION_IAT);

            audio = new AudioAction();

         
            audio.init(msc, auth.getRunPath_wav(),mform);

       //    outputPath = audio.outputPath; //获取本次文件地址

            //录音，上传
            audio.StartRecordingHandler();

            //获取结果
            msc_result = new Thread(new ThreadStart(msc.getResultHandler));
    
            msc_result.Start();
          

        }

        
        /// <summary>
        /// 连续语音识别结束
        /// </summary>
        private void StopSession_IAT()
        {

            msc_result.Abort();

            audio.StopRecording();
            msc.SessionEnd();
     

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

            Console.WriteLine("文件地址为："+auth.getRunPath_wav());
            ///设置文件地址
            msc.SetINFILE(auth.getRunPath_wav());
   
            msc_Uploadfile = new Thread(new ThreadStart(msc.AudioWriteFile));
            msc_Uploadfile.Start();
 
            ///获取结果
            msc_result = new Thread(new ThreadStart(msc.getResultHandler));
            msc_result.Start();

        }
        /// <summary>
        /// 语义识别结束
        /// </summary>
        private void StopSession_ASR()
        {
          
            msc_Uploadfile.Abort();
            msc_result.Abort();
         
            msc.SessionEnd();     
 
            msc = null;
            auth = null;
        }


    }
}
