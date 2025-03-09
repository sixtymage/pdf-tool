using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using PdfMerger.Models;

namespace PdfMerger.Helpers
{
  public class FileHelper
  {
    private readonly Logger _logger;

    public FileHelper(Logger logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OpenFile(string filePath)
    {
      try
      {
        if (File.Exists(filePath))
        {
          Process.Start(new ProcessStartInfo
          {
            FileName = filePath,
            UseShellExecute = true
          });

          _logger.Log($"Opened file: {filePath}", LogLevel.Info);
        }
        else
        {
          _logger.Log($"File not found: {filePath}", LogLevel.Warning);
          MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error opening file: {filePath}", ex);
        MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }

    public static bool IsPdfFile(string filePath)
    {
      try
      {
        string extension = Path.GetExtension(filePath);
        return extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
      }
      catch
      {
        return false;
      }
    }

    public static PdfFile CreatePdfFile(string filePath)
    {
      if (!IsPdfFile(filePath))
      {
        throw new ArgumentException("File is not a PDF", nameof(filePath));
      }

      return new PdfFile(filePath);
    }
  }
}