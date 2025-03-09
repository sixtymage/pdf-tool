using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PdfMerger.ViewModels;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfMerger.Models
{
  public class PdfPage : ViewModelBase
  {
    private ImageSource? _thumbnail;

    public string SourceFilePath { get; }
    public int PageNumber { get; }  // 1-based page number in source PDF
    public string DisplayName { get; }
    public string SourceFileName { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public ImageSource? Thumbnail
    {
      get => _thumbnail;
      private set => SetProperty(ref _thumbnail, value);
    }

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

    public void GenerateThumbnail(int width = 150, int height = 200)
    {
      try
      {
        using (var document = PdfReader.Open(SourceFilePath, PdfDocumentOpenMode.Import))
        {
          if (PageNumber <= document.Pages.Count)
          {
            var page = document.Pages[PageNumber - 1];

            // Create a bitmap to render the page
            using (var stream = new MemoryStream())
            {
              // Create XImage from the PDF page
              using (var gfx = XGraphics.FromPdfPage(page))
              {
                // Get page dimensions
                var pageWidth = page.Width.Point;
                var pageHeight = page.Height.Point;

                // Calculate scale to fit desired dimensions while maintaining aspect ratio
                var scaleX = width / pageWidth;
                var scaleY = height / pageHeight;
                var scale = Math.Min(scaleX, scaleY);

                // Create scaled bitmap
                var scaledWidth = (int)(pageWidth * scale);
                var scaledHeight = (int)(pageHeight * scale);

                // Create WPF bitmap
                var bitmap = new RenderTargetBitmap(
                    scaledWidth,
                    scaledHeight,
                    96, // DPI X
                    96, // DPI Y
                    PixelFormats.Pbgra32);

                // Create drawing visual
                var visual = new DrawingVisual();
                using (var context = visual.RenderOpen())
                {
                  // Draw white background
                  context.DrawRectangle(
                      Brushes.White,
                      null,
                      new System.Windows.Rect(0, 0, scaledWidth, scaledHeight));

                  // Draw the page content
                  context.PushTransform(new ScaleTransform(scale, scale));
                  // Add page rendering here when we figure out how to render PDF content
                }

                // Render the visual to the bitmap
                bitmap.Render(visual);

                // Create a PNG encoder
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                // Save to memory stream
                encoder.Save(stream);

                // Create bitmap image from stream
                stream.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // Make it thread-safe

                // Set the thumbnail
                Thumbnail = bitmapImage;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        // Log error but don't throw - thumbnail generation is not critical
        System.Diagnostics.Debug.WriteLine($"Error generating thumbnail: {ex.Message}");
      }
    }
  }
}