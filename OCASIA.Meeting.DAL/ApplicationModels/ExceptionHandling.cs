using System;
using System.IO;

namespace OCASIA.Meeting.DAL.ApplicationModels
{
    public interface ILoggerFacade
    {
        void Error(Exception e);
        void Warning(Exception e);
    }

    public class SimpleLogger : ILoggerFacade
    {

        public void Error(Exception e)
        {
            try
            {
                using (StreamWriter outputFile = new StreamWriter(GetPath("AdminException.txt"), true))
                {
                    outputFile.WriteLine("---------------------------------Start------------------------------------");
                    outputFile.WriteLine("Erro Date Time:" + DateTime.Now.ToString());
                    outputFile.WriteLine("Source: " + e.Source ?? e.InnerException?.Source);
                    outputFile.WriteLine("Stack Trace: " + e.StackTrace);
                    outputFile.WriteLine("Message: " + e.Message);
                    outputFile.WriteLine("Inner Exception Message: " + e.InnerException?.Message);
                    outputFile.WriteLine("--------------------------------------------------------------------------");

                }
            }
            catch
            {
                using (StreamWriter outputFile = new StreamWriter(GetPath("AdminException_1.txt"), true))
                {
                    outputFile.WriteLine("---------------------------------Start------------------------------------");
                    outputFile.WriteLine("Erro Date Time:" + DateTime.Now.ToString());
                    outputFile.WriteLine("Source: " + e.Source ?? e.InnerException?.Source);
                    outputFile.WriteLine("Stack Trace: " + e.StackTrace);
                    outputFile.WriteLine("Message: " + e.Message);
                    outputFile.WriteLine("Inner Exception Message: " + e.InnerException?.Message);
                    outputFile.WriteLine("--------------------------------------------------------------------------");

                }
            }
        }

        public static string GetPath(string localPath)
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory + @"\Error\";
            if (!Directory.Exists(currentDir))
                Directory.CreateDirectory(currentDir);
            DirectoryInfo directory = new DirectoryInfo(
                Path.GetFullPath(Path.Combine(currentDir, DateTime.Today.ToString("dd'.'MM'.'yyyy") + "_" + localPath)));
            return directory.ToString();
        }

        public void Warning(Exception e)
        {
            throw new NotImplementedException();
        }
    }

    public class StaticLogger
    {

        static readonly ILoggerFacade _logger = new SimpleLogger();
        StaticLogger()
        {
        }
        public static ILoggerFacade Logger
        {
            get { return _logger; }
        }
    }
}
