using System;
using System.Runtime.InteropServices;


namespace voice2text
{
    class MSCDll
    {
        ///<summary>
        ///初始化msc，用户登录。使用其他接口前必须先调用MSPLogin，可以在应用程序启动时调用。
        ///</summary>
        ///<param name="usr">此参数保留，传入NULL即可。</param>
        ///<param name="pwd">此参数保留，传入NULL即可。</param>
        ///<param name="_params">appid = 5964bde1</param>
        ///<returns></returns>
        [DllImport("msc.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MSPLogin(string usr, string pwd, string _params);

        /// <summary>
        ///上传数据。 
        /// </summary>
        [DllImport("msc.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr MSPUploadData(string dataName, IntPtr data, uint dataLen, string _params, ref int errorCode);

        /// <summary>
        ///开始一次语音识别。 
        /// </summary>
         /// <param name="grammarList">进行连续语音识别(sub=iat)时，此参数设为NULL</param>
        /// <param name="_params">本次识别参数</param>
        /// <param name="errorCode">函数调用成功则其值为MSP_SUCCESS，否则返回错误代码</param>
        /// <returns></returns>
        [DllImport("msc.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr QISRSessionBegin(string grammarList, string _params, ref int errorCode);


        /// <summary>
        /// 写入本次识别的音频
        /// </summary>
        /// <param name="sessionID">由QISRSessionBegin返回的句柄</param>
        /// <param name="waveData">音频数据缓冲区起始地址</param>
        /// <param name="waveLen">音频数据长度,单位字节</param>
        /// <param name="audioStatus">来告知MSC音频发送是否完成</param>
        /// <param name="epStatus">端点检测（End-point detected）器所处的状态</param>
        /// <param name="recogStatus">识别器返回的状态</param>
        /// <returns></returns>
        [DllImport("msc.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int QISRAudioWrite(string sessionID, IntPtr waveData, uint waveLen, int audioStatus, ref int epStatus, ref int recogStatus);

        /// <summary>
        /// 获取识别结果。
        /// </summary>
        /// <param name="sessionID">由QISRSessionBegin返回的句柄</param>
        /// <param name="rsltStatus">识别结果的状态,其取值范围和含义请参考QISRAudioWrite的参数recogStatus</param>
        /// <param name="waitTime">此参数做保留用</param>
        /// <param name="errorCode">函数调用成功返回MSP_SUCCESS，否则返回错误代码</param>
        /// <returns></returns>
        [DllImport("msc.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr QISRGetResult(string sessionID, ref int rsltStatus, int waitTime, ref int errorCode);

        /// <summary>
        /// 结束本次语音识别。
        /// </summary>
        /// <param name="sessionID">        由QISRSessionBegin返回的句柄</param>
        /// <param name="hints">结束本次语音识别的原因描述，为用户自定义内容。</param>
        /// <returns></returns>
        [DllImport("msc.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int QISRSessionEnd(string sessionID, string hints);

        /// <summary>
        /// 获取当次语音识别信息，如上行流量、下行流量等。
        /// </summary>
        /// <param name="sessionID">由QISRSessionEnd返回的句柄，如果为NULL，获取MSC的设置信息</param>
        /// <param name="paramName">参数名，一次调用只支持查询一个参数</param>
        /// <param name="paramValue">输入:buffer首地址 输出:向该buffer写入获取到的信息</param>
        /// <param name="valueLen">输入:buffer的大小，valueLen大小需根据结果大小做调整  输出:信息实际长度(不含’\0’),长度需+1</param>
        /// <returns></returns>
        [DllImport("msc.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int QISRGetParam(string sessionID, string paramName, string paramValue, ref uint valueLen);

        /// <summary>
        /// 退出
        /// </summary>
        [DllImport("msc.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern int MSPLogout();


    }
  
}
