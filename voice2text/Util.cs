using System;
using System.Collections.Generic;
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
    }
}
