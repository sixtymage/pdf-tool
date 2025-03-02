using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using PdfMerger.Models;

namespace PdfMerger.Helpers
{
    public static class FileHelper
    {
        public static void OpenFile(string filePath)
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
                    
                    Logger.Log($"Opened file: {filePath}");
                }
                else
                {
                    Logger.Log(LogLevel.Warning, $"File not found: {filePath}");
                    MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error opening file: {filePath}", ex);
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