using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using PdfMerger.Helpers;
using PdfMerger.Models;

namespace PdfMerger.ViewModels
{
  public class PageOrderingViewModel : ViewModelBase
  {
    private readonly Logger _logger;
    private bool _isGeneratingThumbnails;
    private double _thumbnailProgress;
    private readonly ObservableCollection<PdfPage> _pages = new();

    public ObservableCollection<PdfPage> Pages => _pages;
    public ICommand MoveUpCommand { get; }
    public ICommand MoveDownCommand { get; }
    public ICommand RemovePageCommand { get; }
    public ICommand ApplyOrderCommand { get; }
    public ICommand CancelCommand { get; }

    public event EventHandler<PageOrderResult>? OrderingComplete;

    public bool IsGeneratingThumbnails
    {
      get => _isGeneratingThumbnails;
      private set => SetProperty(ref _isGeneratingThumbnails, value);
    }

    public double ThumbnailProgress
    {
      get => _thumbnailProgress;
      private set => SetProperty(ref _thumbnailProgress, value);
    }

    public PageOrderingViewModel(ObservableCollection<PdfFile> pdfFiles, Logger logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      // Initialize commands
      MoveUpCommand = new RelayCommand(MovePageUp, CanMovePageUp);
      MoveDownCommand = new RelayCommand(MovePageDown, CanMovePageDown);
      RemovePageCommand = new RelayCommand(RemovePage, _ => true);
      ApplyOrderCommand = new RelayCommand(_ => ApplyOrder(), _ => Pages.Any());
      CancelCommand = new RelayCommand(_ => Cancel());

      // Load pages from PDF files
      LoadPages(pdfFiles);

      _logger.Log("PageOrderingViewModel initialized", LogLevel.Info);
    }

    private void LoadPages(ObservableCollection<PdfFile> pdfFiles)
    {
      try
      {
        foreach (var file in pdfFiles)
        {
          for (int i = 1; i <= file.PageCount; i++)
          {
            var page = new PdfPage(file.FilePath, i, _logger);
            Pages.Add(page);
          }
        }

        _logger.Log($"Loaded {Pages.Count} pages from {pdfFiles.Count} files", LogLevel.Info);

        // Start monitoring thumbnail progress
        _ = MonitorThumbnailProgressAsync();
      }
      catch (Exception ex)
      {
        _logger.LogError("Error loading pages", ex);
      }
    }

    private async Task MonitorThumbnailProgressAsync()
    {
      try
      {
        IsGeneratingThumbnails = true;
        ThumbnailProgress = 0;

        _logger.Log("Starting thumbnail generation", LogLevel.Info);

        var totalPages = Pages.Count;
        var completedPages = 0;

        while (completedPages < totalPages)
        {
          completedPages = Pages.Count(p => p.ThumbnailProgress == 100);
          ThumbnailProgress = (completedPages / (double)totalPages) * 100.0;
          await Task.Delay(100); // Check progress every 100ms
        }

        _logger.Log("Thumbnail generation complete", LogLevel.Info);
      }
      catch (Exception ex)
      {
        _logger.LogError("Error monitoring thumbnails", ex);
      }
      finally
      {
        IsGeneratingThumbnails = false;
      }
    }

    private bool CanMovePageUp(object parameter)
    {
      if (parameter is PdfPage page)
      {
        int index = Pages.IndexOf(page);
        return index > 0;
      }
      return false;
    }

    private void MovePageUp(object parameter)
    {
      if (parameter is PdfPage page)
      {
        int index = Pages.IndexOf(page);
        if (index > 0)
        {
          Pages.Move(index, index - 1);
          _logger.Log($"Moved page up: {Path.GetFileName(page.FilePath)} page {page.PageNumber}", LogLevel.Info);
        }
      }
    }

    private bool CanMovePageDown(object parameter)
    {
      if (parameter is PdfPage page)
      {
        int index = Pages.IndexOf(page);
        return index >= 0 && index < Pages.Count - 1;
      }
      return false;
    }

    private void MovePageDown(object parameter)
    {
      if (parameter is PdfPage page)
      {
        int index = Pages.IndexOf(page);
        if (index < Pages.Count - 1)
        {
          Pages.Move(index, index + 1);
          _logger.Log($"Moved page down: {Path.GetFileName(page.FilePath)} page {page.PageNumber}", LogLevel.Info);
        }
      }
    }

    private void RemovePage(object parameter)
    {
      if (parameter is PdfPage page)
      {
        Pages.Remove(page);
        _logger.Log($"Removed page: {Path.GetFileName(page.FilePath)} page {page.PageNumber}", LogLevel.Info);
      }
    }

    public void MovePage(PdfPage page, int newIndex)
    {
      if (newIndex >= 0 && newIndex < Pages.Count)
      {
        int oldIndex = Pages.IndexOf(page);
        if (oldIndex >= 0 && oldIndex != newIndex)
        {
          Pages.Move(oldIndex, newIndex);
          _logger.Log($"Moved page from {oldIndex} to {newIndex}", LogLevel.Info);
        }
      }
    }

    private void ApplyOrder()
    {
      OrderingComplete?.Invoke(this, new PageOrderResult(true, Pages.ToList()));
      _logger.Log("Page order applied", LogLevel.Info);
    }

    private void Cancel()
    {
      OrderingComplete?.Invoke(this, new PageOrderResult(false, Pages.ToList()));
      _logger.Log("Page ordering cancelled", LogLevel.Info);
    }
  }

  public class PageOrderResult
  {
    public bool IsConfirmed { get; }
    public List<PdfPage> Pages { get; }

    public PageOrderResult(bool isConfirmed, List<PdfPage> pages)
    {
      IsConfirmed = isConfirmed;
      Pages = pages;
    }
  }
}