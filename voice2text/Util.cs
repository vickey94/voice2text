using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace voice2text
{
    /// <summary>
    /// 工具类
    /// </summary>
    class Util
    {
        public static string getNowTime()
        {
            return String.Format("{0:yyy-MM-dd HH-mm-ss}", DateTime.Now);
        }



        /// <summary>
        /// 指针转字符串
        /// </summary>
        /// <param name="p">指向非托管代码字符串的指针</param>
        /// <returns>返回指针指向的字符串</returns>
        public static string Ptr2Str(IntPtr p)
        {
            List<byte> lb = new List<byte>();
            while (Marshal.ReadByte(p) != 0)
            {
                lb.Add(Marshal.ReadByte(p));
                p = p + 1;
            }
            byte[] bs = lb.ToArray();

            return Encoding.Default.GetString(bs);
        }


        /// <summary>
        /// 全局监控开始调用登录
        /// </summary>
        public static void MSPLogin()
        {
            //使用其他接口前必须先调用MSPLogin，可以在应用程序启动时调用。
            int ret = MSCDll.MSPLogin(null, null, Config.PARAMS_LOGIN);
            if (ret == 0) Console.WriteLine(Util.getNowTime() + " 讯飞语音会话登录成功！");
            else throw new Exception("MSPLogin失败 errCode=" + ret);
        }

        /// <summary>
        /// 文本输出
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        public static void Write2TXT(string path, string text)
        {
            FileStream fs = new FileStream(path, FileMode.Append);
            //获得字节数组
            byte[] data = Encoding.UTF8.GetBytes(text + "\r\n");
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }
    }
}
