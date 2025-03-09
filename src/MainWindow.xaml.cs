using System;
using System.IO;
using System.Linq;
using System.Windows;
using PdfMerger.Helpers;
using PdfMerger.Models;
using PdfMerger.ViewModels;
using PdfMerger.Services;

namespace PdfMerger;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
  private readonly MainViewModel _viewModel;
  private readonly Logger _logger;
  private readonly FileHelper _fileHelper;

  public MainWindow()
  {
    InitializeComponent();

    // Initialize services
    _logger = new Logger();
    var pdfService = new PdfService(_logger);
    _fileHelper = new FileHelper(_logger);

    // Initialize view model
    _viewModel = new MainViewModel(pdfService, _logger);
    DataContext = _viewModel;

    _logger.Log("MainWindow initialized", LogLevel.Info);
  }

  private void Window_Drop(object sender, DragEventArgs e)
  {
    if (e.Data.GetDataPresent(DataFormats.FileDrop))
    {
      string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

      foreach (string file in files)
      {
        try
        {
          if (FileHelper.IsPdfFile(file))
          {
            _viewModel.PdfFiles.Add(FileHelper.CreatePdfFile(file));
            _logger.Log($"File dropped: {file}", LogLevel.Info);
          }
          else
          {
            _logger.Log($"Invalid file dropped: {file}", LogLevel.Warning);
            MessageBox.Show($"The file {Path.GetFileName(file)} is not a valid PDF file.",
                "Invalid File", MessageBoxButton.OK, MessageBoxImage.Warning);
          }
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error processing dropped file: {file}", ex);
          MessageBox.Show($"Error processing file: {ex.Message}",
              "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
    }
  }

  private void Window_DragOver(object sender, DragEventArgs e)
  {
    if (e.Data.GetDataPresent(DataFormats.FileDrop))
    {
      string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
      if (files.Any(file => FileHelper.IsPdfFile(file)))
      {
        e.Effects = DragDropEffects.Copy;
      }
      else
      {
        e.Effects = DragDropEffects.None;
      }
    }
    else
    {
      e.Effects = DragDropEffects.None;
    }

    e.Handled = true;
  }
}