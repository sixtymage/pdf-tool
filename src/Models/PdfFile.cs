using System;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfMerger.Models
{
  public class PdfFile
  {
    public string FilePath { get; }
    public string FileName { get; }
    public long FileSize { get; }
    public DateTime LastModified { get; }
    public int PageCount { get; }

    public PdfFile(string filePath)
    {
      if (string.IsNullOrEmpty(filePath))
        throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

      if (!File.Exists(filePath))
        throw new FileNotFoundException("File not found", filePath);

      FilePath = filePath;
      FileName = Path.GetFileName(filePath);

      var fileInfo = new FileInfo(filePath);
      FileSize = fileInfo.Length;
      LastModified = fileInfo.LastWriteTime;

      // Get page count using PDFsharp
      using (var document = PdfReader.Open(filePath, PdfDocumentOpenMode.Import))
      {
        PageCount = document.Pages.Count;
      }
    }

    public string FormattedSize
    {
      get
      {
        const long KB = 1024;
        const long MB = KB * 1024;

        if (FileSize < KB)
          return $"{FileSize} B";
        if (FileSize < MB)
          return $"{FileSize / KB:F1} KB";

        return $"{FileSize / MB:F1} MB";
      }
    }

    public override string ToString()
    {
      return FileName;
    }
  }
}