using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
    private ListViewItem? _draggedItem;

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
      var item = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
      if (item == null) return;

      var page = listView.ItemContainerGenerator.ItemFromContainer(item) as PdfPage;
      if (page == null) return;

      _isDragging = true;
      _draggedItem = item;

      // Store the original opacity and set a new one for visual feedback
      var originalOpacity = item.Opacity;
      item.Opacity = 0.5;

      try
      {
        DragDrop.DoDragDrop(item, page, DragDropEffects.Move);
      }
      finally
      {
        // Restore the original opacity
        item.Opacity = originalOpacity;
        _isDragging = false;
        _draggedItem = null;
      }
    }

    private void ListView_DragOver(object sender, DragEventArgs e)
    {
      if (!e.Data.GetDataPresent(typeof(PdfPage)))
      {
        e.Effects = DragDropEffects.None;
        e.Handled = true;
        return;
      }

      e.Effects = DragDropEffects.Move;
      e.Handled = true;

      // Get the position relative to the ListView
      if (sender is ListView listView)
      {
        // Get the item under the cursor
        var targetItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);

        // Clear previous adorners
        var adornerLayer = AdornerLayer.GetAdornerLayer(listView);
        if (adornerLayer != null)
        {
          var adorners = adornerLayer.GetAdorners(listView);
          if (adorners != null)
          {
            foreach (var adorner in adorners)
            {
              adornerLayer.Remove(adorner);
            }
          }
        }

        // If we're not over an item, we might be at the end of the list
        if (targetItem == null)
        {
          var lastItem = listView.ItemContainerGenerator.ContainerFromIndex(listView.Items.Count - 1) as ListViewItem;
          if (lastItem != null)
          {
            var position = e.GetPosition(lastItem);
            if (position.Y > lastItem.ActualHeight / 2)
            {
              // Show drop indicator at the end of the list
              if (adornerLayer != null)
              {
                var lastItemPos = lastItem.TranslatePoint(new Point(), listView);
                var bottomY = lastItemPos.Y + lastItem.ActualHeight;
                var adorner = new InsertionAdorner(listView, bottomY);
                adornerLayer.Add(adorner);
              }
            }
          }
        }
        else if (targetItem != _draggedItem)
        {
          // Show drop indicator between items
          if (adornerLayer != null)
          {
            var adorner = new InsertionAdorner(listView, targetItem.TranslatePoint(new Point(), listView).Y);
            adornerLayer.Add(adorner);
          }
        }
      }
    }

    private void ListView_Drop(object sender, DragEventArgs e)
    {
      if (!e.Data.GetDataPresent(typeof(PdfPage)))
        return;

      var droppedPage = e.Data.GetData(typeof(PdfPage)) as PdfPage;
      if (droppedPage == null || sender is not ListView listView)
        return;

      var viewModel = DataContext as PageOrderingViewModel;
      if (viewModel == null)
        return;

      var targetItem = FindAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
      int targetIndex;

      if (targetItem != null)
      {
        // Drop onto an item
        var targetPage = listView.ItemContainerGenerator.ItemFromContainer(targetItem) as PdfPage;
        if (targetPage != null)
        {
          // Check if we're dropping after this item
          var position = e.GetPosition(targetItem);
          if (position.Y > targetItem.ActualHeight / 2)
          {
            targetIndex = viewModel.Pages.IndexOf(targetPage) + 1;
          }
          else
          {
            targetIndex = viewModel.Pages.IndexOf(targetPage);
          }
        }
        else
        {
          targetIndex = viewModel.Pages.Count;
        }
      }
      else
      {
        // We're not over any item - check if we're below the last item
        var lastItem = listView.ItemContainerGenerator.ContainerFromIndex(listView.Items.Count - 1) as ListViewItem;
        if (lastItem != null)
        {
          var lastItemPos = lastItem.TranslatePoint(new Point(), listView);
          var mousePos = e.GetPosition(listView);

          // If the mouse is below the last item's bottom edge, drop at the end
          if (mousePos.Y > lastItemPos.Y + lastItem.ActualHeight)
          {
            targetIndex = viewModel.Pages.Count - 1;
          }
          else
          {
            // Otherwise, drop before the last item's position
            targetIndex = viewModel.Pages.Count - 2;
          }
        }
        else
        {
          targetIndex = viewModel.Pages.Count;
        }
      }

      // Move the page
      viewModel.MovePage(droppedPage, targetIndex);
    }

    private static T? FindAncestor<T>(DependencyObject? current) where T : DependencyObject
    {
      while (current != null)
      {
        if (current is T ancestor)
        {
          return ancestor;
        }

        // For non-visual elements, walk up the logical tree first
        if (!(current is Visual))
        {
          current = LogicalTreeHelper.GetParent(current);
          continue;
        }

        // For visual elements, use the visual tree
        current = VisualTreeHelper.GetParent(current);
      }
      return null;
    }
  }

  // Adorner for showing insertion point
  public class InsertionAdorner : Adorner
  {
    private readonly double _yPos;

    public InsertionAdorner(UIElement adornedElement, double yPos) : base(adornedElement)
    {
      _yPos = yPos;
      IsHitTestVisible = false;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
      var adornedElement = AdornedElement as FrameworkElement;
      if (adornedElement == null) return;

      var pen = new Pen(Brushes.Blue, 2);
      pen.Freeze();

      var startPoint = new Point(0, _yPos);
      var endPoint = new Point(adornedElement.ActualWidth, _yPos);
      drawingContext.DrawLine(pen, startPoint, endPoint);

      // Draw little triangles at the ends
      const double triangleSize = 6;
      var leftTriangle = new StreamGeometry();
      using (var context = leftTriangle.Open())
      {
        context.BeginFigure(new Point(0, _yPos), true, true);
        context.LineTo(new Point(triangleSize, _yPos - triangleSize), true, false);
        context.LineTo(new Point(triangleSize, _yPos + triangleSize), true, false);
      }
      leftTriangle.Freeze();

      var rightTriangle = new StreamGeometry();
      using (var context = rightTriangle.Open())
      {
        context.BeginFigure(new Point(adornedElement.ActualWidth, _yPos), true, true);
        context.LineTo(new Point(adornedElement.ActualWidth - triangleSize, _yPos - triangleSize), true, false);
        context.LineTo(new Point(adornedElement.ActualWidth - triangleSize, _yPos + triangleSize), true, false);
      }
      rightTriangle.Freeze();

      var brush = Brushes.Blue;
      brush.Freeze();

      drawingContext.DrawGeometry(brush, null, leftTriangle);
      drawingContext.DrawGeometry(brush, null, rightTriangle);
    }
  }
}