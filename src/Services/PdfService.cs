using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PdfMerger.Models;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfMerger.Services
{
  public class PdfService
  {
    public bool IsPdfFile(string filePath)
    {
      return Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<string> MergePdfsAsync(List<PdfFile> pdfFiles, IProgress<int>? progress = null)
    {
      return await Task.Run(() =>
      {
        try
        {
          // Create a new PDF document
          using var outputDocument = new PdfDocument();

          for (int i = 0; i < pdfFiles.Count; i++)
          {
            // Open the document
            using var inputDocument = PdfReader.Open(pdfFiles[i].FilePath, PdfDocumentOpenMode.Import);

            // Add all pages from the input document to the output document
            int pageCount = inputDocument.PageCount;
            for (int j = 0; j < pageCount; j++)
            {
              outputDocument.AddPage(inputDocument.Pages[j]);
            }

            // Report progress
            progress?.Report((i + 1) * 100 / pdfFiles.Count);
          }

          // Generate output filename
          string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
          string outputPath = GetUniqueFilename(desktopPath, "Merged", ".pdf");

          // Save the document
          outputDocument.Save(outputPath);

          return outputPath;
        }
        catch (Exception ex)
        {
          throw new Exception($"Error merging PDF files: {ex.Message}", ex);
        }
      });
    }

    private string GetUniqueFilename(string folderPath, string baseFilename, string extension)
    {
      int counter = 1;
      string filePath;

      do
      {
        filePath = Path.Combine(folderPath, $"{baseFilename}_{counter}{extension}");
        counter++;
      } while (File.Exists(filePath));

      return filePath;
    }
  }
}