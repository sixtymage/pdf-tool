using System;
using System.IO;

namespace PdfMerger.Models
{
  public class PdfPage
  {
    public string SourceFilePath { get; }
    public int PageNumber { get; }  // 1-based page number in source PDF
    public string DisplayName { get; }
    public string SourceFileName => Path.GetFileName(SourceFilePath);

    public PdfPage(string sourceFilePath, int pageNumber)
    {
      if (string.IsNullOrEmpty(sourceFilePath))
        throw new ArgumentException("Source file path cannot be null or empty", nameof(sourceFilePath));

      if (pageNumber < 1)
        throw new ArgumentException("Page number must be greater than 0", nameof(pageNumber));

      if (!File.Exists(sourceFilePath))
        throw new FileNotFoundException("Source PDF file not found", sourceFilePath);

      SourceFilePath = sourceFilePath;
      PageNumber = pageNumber;
      DisplayName = $"{Path.GetFileNameWithoutExtension(sourceFilePath)} - Page {pageNumber}";
    }
  }
}