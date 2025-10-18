using System;
using System.Web;
using System.IO;
using System.Threading;

namespace SDBS3000.Log
{
    public class Log
    {
        //日志类型
        public enum LogType
        {
            DEBUG = 0,
            INFO = 1,
            ERROR = 2
        }

        //日志目录
        private static string Path =   @"D:/Logs3100";

        //日志登记 0：所有 1：系统信息与错误信息 2：错误信息
        private static int LOGLEVEL = 0;

        //读写锁，锁定文件写入权限，每个线程依次等待上个写入完成
        static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();

        public static void Write(LogType type, string logInfo)
        {
            try
            {
                //设置读写锁为写入模式独占资源
                //因写入模式的进入与释放在同一个代码块内，请保证在块内进入写入模式前不会触发异常，否则会因为进入与释放次数不符从而触发异常
                //请勿长时间占用读写锁否则https://dimg04.c-ctrip.com/images/0203d12000a61xm93A89F_W_1280_853_R5_Q70.jpg?proc=autoorient会导致其他线程饥饿。
                LogWriteLock.EnterWriteLock();
                if (LOGLEVEL == 1 && type == LogType.DEBUG)
                    return;
                if (LOGLEVEL == 1 && (type == LogType.DEBUG || type == LogType.INFO))
                    return;
                //日志文件名称，每天一个
                string filename = @"/Log_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                //不存在目录则创建
                if (!Directory.Exists(Path))
                {
                    Directory.CreateDirectory(Path);
                }
                //不存在文件则创建
                if (!File.Exists(Path + filename))
                {
                    File.Create(Path + filename).Close();
                }
                using (FileStream file = new FileStream(Path + filename, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(file))
                    {
                        sw.WriteLine("日志类型：{0}", type == LogType.DEBUG ? "调试日志" : (type == LogType.INFO ? "系统信息" : "错误日志"));
                        sw.WriteLine("日志时间：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        sw.WriteLine("日志信息：{0}", logInfo);
                        sw.WriteLine("---------------------------------------------------------------------------------------------------------------------\n");
                        sw.Dispose();
                    }
                    file.Close();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                //退出写入模式，释放资源占用
                //注意释放与进入次数相同否则会触发异常
                LogWriteLock.ExitWriteLock();
            }
        }
    }
}


