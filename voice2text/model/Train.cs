using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace voice2text.model
{
    /// <summary>
    /// 一次训练的对象
    /// </summary>
    class Train
    {
        private string time; //时间 -->对应音频文件
        private string id;   //工号
        private string text; //识别结果


        public Train() { }



        Train(string time ,string id ,string text)
        {
            this.time = time;
            this.id = id;
            this.text = text;
        }

        /// <summary>
        /// 获取写出的训练信息
        /// </summary>
        /// <returns></returns>
        public string getWriteText()
        {
            return time + "," + id + "," + text;
        }

        public void setTime(string time) { this.time = time; }
        public void setId(string id) { this.id = id; }
        public void setText(string text) { this.text = text; }

     //   public string getTime() { return time; }

        public string getTrainPath_wav() { return Config.outputFolder + "\\train\\wav\\" + time + ".wav"; }
        public string getTrainPath_txt() { return Config.outputFolder + "\\train\\" + Config.outputTXT; }

    }
}
