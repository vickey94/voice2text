using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace voice2text.model
{
    class Auth
    {
        private string time; //时间 -->对应音频文件


        public void setTime(string time) { this.time = time; }
      //  public void setId(string id) { this.id = id; }
       // public void setText(string text) { this.text = text; }

        //   public string getTime() { return time; }

        public string getTrainPath_wav() { return Config.outputFolder + "\\train\\wav\\" + time + ".wav"; }
        public string getTrainPath_txt() { return Config.outputFolder + "\\train\\" + Config.outputTXT; }

        public string getRunPath_wav() { return Config.outputFolder + "\\run\\wav\\" + time + ".wav"; }
        public string getRunPath_txt() { return Config.outputFolder + "\\run\\" + Config.outputTXT; }
    }
}
