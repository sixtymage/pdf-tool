using System;
using System.IO;
using System.Windows.Media.Imaging;
using PdfiumViewer;
using PdfMerger.ViewModels;
using PdfMerger.Helpers;

namespace PdfMerger.Models
{
  public class PdfPage : ViewModelBase
  {
    private readonly string _filePath;
    private readonly int _pageNumber;
    private BitmapSource _thumbnail;
    private double _thumbnailProgress;
    private readonly Logger _logger;

    public PdfPage(string filePath, int pageNumber, Logger logger)
    {
      _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
      if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than 0");
      _pageNumber = pageNumber;
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      // Create an empty bitmap as placeholder
      _thumbnail = new BitmapImage();
      LoadThumbnailAsync();
    }

    public string FilePath => _filePath;
    public int PageNumber => _pageNumber;

    public BitmapSource Thumbnail
    {
      get => _thumbnail;
      private set => SetProperty(ref _thumbnail, value);
    }

    public double ThumbnailProgress
    {
      get => _thumbnailProgress;
      private set => SetProperty(ref _thumbnailProgress, value);
    }

    private async void LoadThumbnailAsync()
    {
      try
      {
        ThumbnailProgress = 0;
        await using var fileStream = File.OpenRead(_filePath);
        using var pdfDocument = PdfDocument.Load(fileStream);

        ThumbnailProgress = 33;
        var size = pdfDocument.PageSizes[_pageNumber - 1];
        var scale = Math.Min(100 / size.Width, 150 / size.Height);
        var width = (int)(size.Width * scale);
        var height = (int)(size.Height * scale);

        ThumbnailProgress = 66;
        using var image = pdfDocument.Render(_pageNumber - 1, width, height, 96, 96, false);
        var bitmap = new System.Drawing.Bitmap(image);

        ThumbnailProgress = 90;
        using var memory = new MemoryStream();
        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
        memory.Position = 0;

        var bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = memory;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        Thumbnail = bitmapImage;
        ThumbnailProgress = 100;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error generating thumbnail for page {PageNumber} of {Path.GetFileName(FilePath)}", ex);
        ThumbnailProgress = -1;
      }
    }
  }
}