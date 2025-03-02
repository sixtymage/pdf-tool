using System;
using System.IO;
using System.Linq;
using System.Windows;
using PdfMerger.Helpers;
using PdfMerger.Models;
using PdfMerger.ViewModels;

namespace PdfMerger;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        
        _viewModel = new MainViewModel();
        DataContext = _viewModel;
        
        Logger.Log("MainWindow initialized");
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            foreach (string file in files)
            {
                try
                {
                    if (FileHelper.IsPdfFile(file))
                    {
                        _viewModel.PdfFiles.Add(new PdfFile(file));
                        Logger.Log($"File dropped: {file}");
                    }
                    else
                    {
                        Logger.Log(LogLevel.Warning, $"Invalid file dropped: {file}");
                        MessageBox.Show($"The file {Path.GetFileName(file)} is not a valid PDF file.", 
                            "Invalid File", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Error processing dropped file: {file}", ex);
                    MessageBox.Show($"Error processing file: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            // Check if all files are PDFs
            bool allPdfs = files.All(FileHelper.IsPdfFile);
            
            e.Effects = allPdfs ? DragDropEffects.Copy : DragDropEffects.None;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        
        e.Handled = true;
    }
}