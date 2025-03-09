using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PdfMerger.Models;
using PdfMerger.Helpers;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfMerger.Services
{
  public class PdfService
  {
    private readonly Logger _logger;

    public PdfService(Logger logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsPdfFile(string filePath)
    {
      return Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<List<Models.PdfPage>> LoadPdfPagesAsync(string filePath)
    {
      return await Task.Run(() =>
      {
        var pages = new List<Models.PdfPage>();

        using (var document = PdfReader.Open(filePath, PdfDocumentOpenMode.Import))
        {
          for (int i = 0; i < document.Pages.Count; i++)
          {
            // Page numbers are 1-based
            pages.Add(new Models.PdfPage(filePath, i + 1));
          }
        }

        return pages;
      });
    }

    public async Task<string> MergePdfsAsync(
      List<PdfFile> pdfFiles,
      string? outputPath = null,
      IProgress<int>? progress = null,
      List<Models.PdfPage>? customPageOrder = null)
    {
      return await Task.Run(() =>
      {
        try
        {
          // If no output path is provided, generate one
          if (string.IsNullOrEmpty(outputPath))
          {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            outputPath = GetUniqueFilename(desktopPath, "Merged", ".pdf");
          }

          // Create a new PDF document
          using (var outputDocument = new PdfDocument())
          {
            if (customPageOrder != null && customPageOrder.Count > 0)
            {
              // Use custom page order
              var openDocuments = new Dictionary<string, PdfDocument>();

              try
              {
                int processedPages = 0;
                foreach (var page in customPageOrder)
                {
                  // Open source document if not already open
                  if (!openDocuments.TryGetValue(page.SourceFilePath, out var sourceDocument))
                  {
                    sourceDocument = PdfReader.Open(page.SourceFilePath, PdfDocumentOpenMode.Import);
                    openDocuments[page.SourceFilePath] = sourceDocument;
                  }

                  // Add the specified page
                  outputDocument.AddPage(sourceDocument.Pages[page.PageNumber - 1]);

                  // Report progress
                  processedPages++;
                  progress?.Report(processedPages * 100 / customPageOrder.Count);
                }
              }
              finally
              {
                // Close all open documents
                foreach (var doc in openDocuments.Values)
                {
                  doc.Dispose();
                }
              }
            }
            else
            {
              // Use default order (all pages from each file)
              for (int i = 0; i < pdfFiles.Count; i++)
              {
                using (var inputDocument = PdfReader.Open(pdfFiles[i].FilePath, PdfDocumentOpenMode.Import))
                {
                  for (int j = 0; j < inputDocument.Pages.Count; j++)
                  {
                    outputDocument.AddPage(inputDocument.Pages[j]);
                  }
                }

                progress?.Report((i + 1) * 100 / pdfFiles.Count);
              }
            }

            // Ensure output directory exists
            string? directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
              Directory.CreateDirectory(directory);
            }

            // Save the document
            outputDocument.Save(outputPath);
          }

          return outputPath;
        }
        catch (Exception ex)
        {
          throw new Exception($"Error merging PDF files: {ex.Message}", ex);
        }
      });
    }

    private string GetUniqueFilename(string directory, string baseName, string extension)
    {
      int counter = 1;
      string filename = Path.Combine(directory, $"{baseName}_{counter}{extension}");

      while (File.Exists(filename))
      {
        counter++;
        filename = Path.Combine(directory, $"{baseName}_{counter}{extension}");
      }

      return filename;
    }
  }
}