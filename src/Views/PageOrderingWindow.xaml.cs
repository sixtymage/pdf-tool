using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PdfMerger.Models;
using PdfMerger.ViewModels;

namespace PdfMerger.Views
{
  public partial class PageOrderingWindow : Window
  {
    private Point _startPoint;
    private bool _isDragging;

    public PageOrderingWindow()
    {
      InitializeComponent();
    }

    private void ListView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      _startPoint = e.GetPosition(null);
    }

    private void ListView_PreviewMouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
      {
        Point position = e.GetPosition(null);

        if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
            Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
        {
          if (sender is ListView listView)
          {
            StartDrag(listView, e);
          }
        }
      }
    }

    private void StartDrag(ListView listView, MouseEventArgs e)
    {
      _isDragging = true;
      var item = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

      if (item != null)
      {
        PdfPage page = (PdfPage)listView.ItemContainerGenerator.ItemFromContainer(item);
        DragDrop.DoDragDrop(item, page, DragDropEffects.Move);
      }

      _isDragging = false;
    }

    private void ListView_DragOver(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(typeof(PdfPage)))
      {
        e.Effects = DragDropEffects.Move;
      }
      else
      {
        e.Effects = DragDropEffects.None;
      }

      e.Handled = true;
    }

    private void ListView_Drop(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(typeof(PdfPage)))
      {
        PdfPage droppedPage = (PdfPage)e.Data.GetData(typeof(PdfPage));
        var targetItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

        if (targetItem != null && sender is ListView listView)
        {
          var viewModel = (PageOrderingViewModel)DataContext;
          PdfPage targetPage = (PdfPage)listView.ItemContainerGenerator.ItemFromContainer(targetItem);
          int targetIndex = viewModel.Pages.IndexOf(targetPage);
          viewModel.MovePage(droppedPage, targetIndex);
        }
      }
    }

    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
      while (current != null)
      {
        if (current is T ancestor)
        {
          return ancestor;
        }
        current = VisualTreeHelper.GetParent(current);
      }
      return null;
    }
  }
}