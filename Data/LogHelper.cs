using System;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Reflection;
using System.Web;

public class Utf8StringWriter : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}

public sealed class LogHelper
{


    public static void Log(string Message)
    {

        StreamWriter sWrite;
        FileInfo fileInfo;

        try
        {
            void Write()
            {
                if (Directory.Exists(@"C:\Logs\"))
                {
                    if (File.Exists(@"C:\Logs\logFile.log"))
                    {
                        fileInfo = new FileInfo(@"C:\Logs\logFile.log");
                    }

                    sWrite = new StreamWriter(@"C:\Logs\logFile.log", true);
                    sWrite.Write(DateTime.Now.ToString() + "(Process ID=" + Process.GetCurrentProcess().Id.ToString() + ") ___________ " + Message + "\r\n");
                    sWrite.Close();
                }
            }

            Write();
        }
        catch
        {
            sWrite = null;
            fileInfo = null;
        }
    }


}
