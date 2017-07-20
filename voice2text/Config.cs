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
        public static readonly string PARAMS_LOGIN = "appid=5964bde1";


        /// <summary>
        /// SessionBegin 
        /// </summary>
        public static readonly string PARAMS_SESSION_IAT = "sub=iat,ptt = 0, rate=16000,ent=sms16k,rst=plain,vad_eos=5000,plain = utf-8 ,aue = speex-wb";
        /// <summary>
        /// 语法模式
        /// </summary>
        public static readonly string PARAMS_SESSION_ASR =  "sub = asr, result_type = plain, result_encoding = utf-8 ,sample_rate = 16000,aue = speex-wb,ent=sms16k";


        /// "sub = asr, result_type = plain, result_encoding = gb2312,sample_rate = 16000,aue = speex-wb,ent=sms16k";


        /// <summary>
        /// 上传语法后的获取的唯一grammarList，下次可以直接使用
        /// </summary>
        public static readonly string grammarList = string.Empty;

        /// <summary>
        /// 文件输出文件夹
        /// </summary>
        public static readonly string outputFolder = "D:";

        /// <summary>
        /// 上传语音文件地址
        /// </summary>
        public static readonly string uploadDataFile = "";
    }
}
