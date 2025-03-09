using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace PdfMerger.Helpers
{
  public enum LogLevel
  {
    Debug,
    Info,
    Warning,
    Error
  }

  public class Logger
  {
    private readonly string _logFilePath;
    private static readonly object LockObject = new object();

    public Logger()
    {
      string appDirectory = AppContext.BaseDirectory;
      _logFilePath = Path.Combine(appDirectory, "PdfMerger.log");

      // Ensure log directory exists
      string? directory = Path.GetDirectoryName(_logFilePath);
      if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
      {
        Directory.CreateDirectory(directory);
      }
    }

    public void Log(string message, LogLevel level = LogLevel.Info)
    {
      try
      {
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

        lock (LockObject)
        {
          File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }

        // Also output to debug console
        Debug.WriteLine(logEntry);
      }
      catch
      {
        // Silently fail if logging fails
      }
    }

    public void LogError(string message, Exception ex)
    {
      var sb = new StringBuilder();
      sb.AppendLine(message);
      sb.AppendLine($"Exception: {ex.Message}");
      sb.AppendLine($"Stack Trace: {ex.StackTrace}");

      if (ex.InnerException != null)
      {
        sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
        sb.AppendLine($"Inner Stack Trace: {ex.InnerException.StackTrace}");
      }

      Log(sb.ToString(), LogLevel.Error);
    }
  }
}