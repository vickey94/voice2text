using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace voice2text
{
    class Config
    {
        /// <summary>
        ///  MSPLogin 登录 appid 参数
        /// </summary>
        public readonly string PARAMS_LOGIN = "appid=5964bde1";


        /// <summary>
        /// SessionBegin 
        /// </summary>
        public readonly string PARAMS_SESSION = "sub=iat,rate=16000,ent=sms16k,rst=plain,vad_eos=5000,plain = gb2312,aue = speex-wb";
        /// "sub = asr, result_type = plain, result_encoding = gb2312,sample_rate = 16000,aue = speex-wb,ent=sms16k";

        /// <summary>
        /// 文件输出文件夹
        /// </summary>
        public readonly string outputFolder = "F:";

        /// <summary>
        /// 上传语音文件地址
        /// </summary>
        public readonly string uploadDataFile = "";
    }
}
