using System;
using System.Diagnostics;

namespace PdfMerger.Helpers
{
  public class BindingErrorTraceListener : TraceListener
  {
    private readonly Logger _logger;

    public BindingErrorTraceListener(Logger logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override void Write(string? message)
    {
      // Do nothing with non-error messages
    }

    public override void WriteLine(string? message)
    {
      if (!string.IsNullOrEmpty(message))
      {
        _logger.Log(message, LogLevel.Warning);
      }
    }
  }
}