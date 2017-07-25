using NAudio.Wave;
using System;
using System.Collections;

namespace voice2text.action
{
    /// <summary>
    /// 音频类
    /// </summary>
    class AudioAction
    {
        /// <summary>
        /// MSCAction主要实现向服务器实时传送数据，
        /// </summary>
        private MSCAction msc = null;
        private IWaveIn waveIn { get; set; }        //

        private WaveFileWriter waveWriter;  //数据输出流
                                            // private byte[] temp_waveBuffer;
    //    private readonly string outputFolder = "D:";


        public string outputPath;

        private const int Recording_BUFFER_NUM = 3200;

        ///<summary>
        ///播放音频
        ///</summary>
        ///<param name="file">文件地址</param>
        public void PlayAudio(string file)
        {
            AudioFileReader audio = new AudioFileReader(file);
            IWavePlayer player = new WaveOut(WaveCallbackInfo.FunctionCallback());
            player.Init(audio);
            player.Play();
     
        }

        /// <summary>
        /// 设置本次参数，传递MSC,如果为NULL，则只为录音
        /// </summary>
        public void init(MSCAction msc)
        {
            this.msc = msc;

            string outputFilename = Util.getNowTime() + ".wav";
           // outputPath = Path.Combine(Config.outputFolder, outputFilename);
            outputPath = Config.outputFolder + "\\" + outputFilename;

          
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(16000, 16, 1); //

            waveWriter = new WaveFileWriter(outputPath, waveIn.WaveFormat);
            
            waveIn.DataAvailable += OnDataAvailable;
            

            //这个地方可以重写stop,但我们的只要waveIn.Dispose()关闭清空数据就可以，所有并没有重写
            //waveIn.RecordingStopped += OnRecordingStopped; 
        }

        public Mainform MForm;
        public void setMForm(Mainform m) { this.MForm = m; }

        ///<summary>
        ///开始录音 需要一个线程
        ///</summary>
        public void StartRecordingHandler()
        {
            Console.WriteLine(Util.getNowTime() + " 开始录音");

            waveIn.StartRecording();

            Console.WriteLine("正在录音......");
         
        }

  

        ///<summary>
        ///录音数据输出
        ///</summary>
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] temp_waveBuffer = null;

            int length = e.BytesRecorded;


            if (Config.advData1 != null)
            {
                Console.WriteLine("预录音数据传输");

                   temp_waveBuffer = Config.advData1;
                   waveWriter.Write(temp_waveBuffer, 0, length);
                   if (msc != null) msc.AudioWrite(temp_waveBuffer);

                temp_waveBuffer = Config.advData2;
                waveWriter.Write(temp_waveBuffer, 0, length);
                if (msc != null) msc.AudioWrite(temp_waveBuffer);

                Config.advData1 = null;
                Config.advData2 = null;
            }
          
                temp_waveBuffer = e.Buffer;

                waveWriter.Write(temp_waveBuffer, 0, e.BytesRecorded);
                if (msc != null) msc.AudioWrite(temp_waveBuffer);



            long sh = System.BitConverter.ToInt64(temp_waveBuffer, 0);

            long width = (long)Math.Pow(2, 50);
            float svolume = Math.Abs(sh / width);
              if (svolume > 1500.0f) { svolume = 1500.0f; }

            svolume = svolume / 15.0f;
            MForm.setProgressBar((int)svolume);
            //  int secondsRecorded = (int)(writer.Length / writer.WaveFormat.AverageBytesPerSecond);//录音时间获取 
        }

        /// <summary>
        /// 停止录音
        /// </summary>
        public void StopRecording()
        {

            if (waveIn != null) // 关闭录音对象
            {
                waveIn.StopRecording();
                waveIn.Dispose();
                waveIn = null;
            }
            if (waveWriter != null)//关闭文件流
            {
                waveWriter.Close();
                waveWriter = null;
            }
            // waveIn.StopRecording();
            Console.WriteLine(Util.getNowTime() + " 录音结束");
        }

        
        public IWaveIn waveMonitor;
        public float volume = 0.0f;

        public void initMonitor()
        {

            waveMonitor = new WaveInEvent();
            waveMonitor.WaveFormat = new WaveFormat(16000, 16, 1); //
            waveMonitor.DataAvailable += OnDataAvailableMonitor;

        }

        public void StartMonitoringHandler()
        {
            Console.WriteLine(Util.getNowTime() + " 开始监听环境声音");

            waveMonitor.StartRecording();
         

        }
       

        private void OnDataAvailableMonitor(object sender, WaveInEventArgs e)
        {

            byte[] temp_waveBuffer = e.Buffer;
          

            long sh = System.BitConverter.ToInt64(temp_waveBuffer, 0);
      
            long width = (long)Math.Pow(2, 50);
            float svolume = Math.Abs(sh / width);
            //  if (svolume > 1500.0f) { svolume = 1500.0f; }
            //   if (svolume < 50.0f) { svolume = 50.0f; }
            //   this.volume = svolume / 15.0f;
            this.volume = svolume;

            SetAdvData(temp_waveBuffer);
        
            if (volume > 300)
            {             
                System.Console.WriteLine(Util.getNowTime() + " 监听到较大声音" + string.Format("{0}", this.volume));
                Config.isSpeeking = true;
            }
               


           //   int secondsRecorded = (int)(waveWriter.Length / waveWriter.WaveFormat.AverageBytesPerSecond);//录音时间获取 
        }

        public void StopMonitoring()
        {
            waveMonitor.StopRecording();

        }

        public void FinshMonitoring()
        {
            if (waveMonitor != null) // 关闭录音对象
            {
                waveMonitor.Dispose();
                waveMonitor = null;
            }

            Console.WriteLine(Util.getNowTime() + " 录音结束");
        }


        /// <summary>
        /// 用于监听时，提前存入数据
        /// </summary>
        /// <param name="temp"></param>
        private void SetAdvData(byte[] temp)
        {
            if (Config.advData2 != null)
            {
                Config.advData1 = Config.advData2;
                Config.advData2 = temp;
            }
            else
            {
                if (Config.advData1 == null) Config.advData1 = temp;
                else Config.advData2 = Config.advData1 = temp;
            }
        }
       

    }
}
