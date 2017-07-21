using NAudio.Wave;
using System;


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
            
            byte[] temp_waveBuffer = e.Buffer;

            waveWriter.Write(temp_waveBuffer, 0, e.BytesRecorded);
            if (msc != null)  msc.AudioWrite(temp_waveBuffer);

            //  int secondsRecorded = (int)(writer.Length / writer.WaveFormat.AverageBytesPerSecond);//录音时间获取 
        }

        /// <summary>
        /// 停止录音
        /// </summary>
        public void StopRecording()
        {

            if (waveIn != null) // 关闭录音对象
            {
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
    }
}
