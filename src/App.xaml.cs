using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using PdfMerger.Helpers;
using PdfMerger.Services;
using PdfMerger.ViewModels;

namespace PdfMerger;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
  private Logger? _logger;
  private PdfService? _pdfService;

  public App()
  {
    // Initialize logger first since it's needed for binding error tracking
    _logger = new Logger();

    // Add handler for binding errors
    PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
    PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorTraceListener(_logger));

    // Add handlers for unhandled exceptions
    AppDomain.CurrentDomain.UnhandledException += (s, e) =>
      LogUnhandledException("AppDomain.CurrentDomain.UnhandledException", e.ExceptionObject as Exception);

    DispatcherUnhandledException += (s, e) =>
    {
      LogUnhandledException("Application.Current.DispatcherUnhandledException", e.Exception);
      e.Handled = true;  // Prevent the application from crashing
    };

    TaskScheduler.UnobservedTaskException += (s, e) =>
    {
      LogUnhandledException("TaskScheduler.UnobservedTaskException", e.Exception);
      e.SetObserved();  // Prevent the application from crashing
    };
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    // Initialize services
    if (_logger == null)
    {
      _logger = new Logger();
    }
    _pdfService = new PdfService(_logger);

    // Set up main window with view model
    var mainWindow = new MainWindow();
    mainWindow.DataContext = new MainViewModel(_pdfService, _logger);

    // Show the main window
    MainWindow = mainWindow;
    mainWindow.Show();
  }

  private void LogUnhandledException(string source, Exception? ex)
  {
    if (_logger == null)
    {
      _logger = new Logger();
    }

    if (ex != null)
    {
      _logger.LogError($"Unhandled exception from {source}", ex);

      MessageBox.Show(
        $"An unexpected error occurred: {ex.Message}\n\nThe error has been logged.",
        "Error",
        MessageBoxButton.OK,
        MessageBoxImage.Error);
    }
  }
}

