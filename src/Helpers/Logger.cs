using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace PdfMerger.Helpers
{
  public static class Logger
  {
    private static readonly string LogFilePath;
    private static readonly object LockObject = new object();

    static Logger()
    {
      string appDirectory = AppContext.BaseDirectory;
      LogFilePath = Path.Combine(appDirectory, "PdfMerger.log");
    }

    public static void Log(string message)
    {
      Log(LogLevel.Info, message);
    }

    public static void Log(LogLevel level, string message)
    {
      try
      {
        string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}";

        lock (LockObject)
        {
          File.AppendAllText(LogFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
        }
      }
      catch
      {
        // Silently fail if logging fails
      }
    }

    public static void LogError(string message, Exception ex)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine(message);
      sb.AppendLine($"Exception: {ex.Message}");
      sb.AppendLine($"StackTrace: {ex.StackTrace}");

      if (ex.InnerException != null)
      {
        sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
        sb.AppendLine($"Inner StackTrace: {ex.InnerException.StackTrace}");
      }

      Log(LogLevel.Error, sb.ToString());
    }
  }

  public enum LogLevel
  {
    Debug,
    Info,
    Warning,
    Error
  }
}