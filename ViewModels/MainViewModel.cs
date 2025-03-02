using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using PdfMerger.Helpers;
using PdfMerger.Models;
using PdfMerger.Services;

namespace PdfMerger.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly PdfService _pdfService;
        private bool _isMerging;
        private int _mergeProgress;
        private bool _autoOpenAfterMerge = true;

        public ObservableCollection<PdfFile> PdfFiles { get; } = new ObservableCollection<PdfFile>();
        
        public ICommand AddFilesCommand { get; }
        public ICommand RemoveFileCommand { get; }
        public ICommand ClearFilesCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }
        public ICommand MergePdfsCommand { get; }

        public bool IsMerging
        {
            get => _isMerging;
            set => SetProperty(ref _isMerging, value);
        }

        public int MergeProgress
        {
            get => _mergeProgress;
            set => SetProperty(ref _mergeProgress, value);
        }

        public bool AutoOpenAfterMerge
        {
            get => _autoOpenAfterMerge;
            set => SetProperty(ref _autoOpenAfterMerge, value);
        }

        public MainViewModel()
        {
            _pdfService = new PdfService();
            
            AddFilesCommand = new RelayCommand(_ => AddFiles());
            RemoveFileCommand = new RelayCommand(RemoveFile, CanRemoveFile);
            ClearFilesCommand = new RelayCommand(_ => ClearFiles(), _ => PdfFiles.Any());
            MoveUpCommand = new RelayCommand(MoveFileUp, CanMoveFileUp);
            MoveDownCommand = new RelayCommand(MoveFileDown, CanMoveFileDown);
            MergePdfsCommand = new RelayCommand(_ => MergePdfs(), _ => CanMergePdfs());
            
            Logger.Log("Application started");
        }

        private void AddFiles()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    Multiselect = true,
                    Title = "Select PDF Files"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        if (FileHelper.IsPdfFile(filePath))
                        {
                            PdfFiles.Add(new PdfFile(filePath));
                            Logger.Log($"Added file: {filePath}");
                        }
                        else
                        {
                            Logger.Log(LogLevel.Warning, $"Attempted to add non-PDF file: {filePath}");
                            MessageBox.Show($"The file {Path.GetFileName(filePath)} is not a valid PDF file.", 
                                "Invalid File", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error adding files", ex);
                MessageBox.Show($"Error adding files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanRemoveFile(object parameter)
        {
            return parameter is PdfFile && PdfFiles.Contains(parameter as PdfFile);
        }

        private void RemoveFile(object parameter)
        {
            if (parameter is PdfFile file)
            {
                PdfFiles.Remove(file);
                Logger.Log($"Removed file: {file.FilePath}");
            }
        }

        private void ClearFiles()
        {
            PdfFiles.Clear();
            Logger.Log("Cleared all files");
        }

        private bool CanMoveFileUp(object parameter)
        {
            if (parameter is PdfFile file)
            {
                int index = PdfFiles.IndexOf(file);
                return index > 0;
            }
            return false;
        }

        private void MoveFileUp(object parameter)
        {
            if (parameter is PdfFile file)
            {
                int index = PdfFiles.IndexOf(file);
                if (index > 0)
                {
                    PdfFiles.Move(index, index - 1);
                    Logger.Log($"Moved file up: {file.FilePath}");
                }
            }
        }

        private bool CanMoveFileDown(object parameter)
        {
            if (parameter is PdfFile file)
            {
                int index = PdfFiles.IndexOf(file);
                return index >= 0 && index < PdfFiles.Count - 1;
            }
            return false;
        }

        private void MoveFileDown(object parameter)
        {
            if (parameter is PdfFile file)
            {
                int index = PdfFiles.IndexOf(file);
                if (index >= 0 && index < PdfFiles.Count - 1)
                {
                    PdfFiles.Move(index, index + 1);
                    Logger.Log($"Moved file down: {file.FilePath}");
                }
            }
        }

        private bool CanMergePdfs()
        {
            return PdfFiles.Count >= 2 && !IsMerging;
        }

        private async void MergePdfs()
        {
            if (PdfFiles.Count < 2)
            {
                MessageBox.Show("Please select at least two PDF files to merge.", 
                    "Not Enough Files", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                IsMerging = true;
                MergeProgress = 0;
                
                Logger.Log($"Starting merge of {PdfFiles.Count} files");
                
                var progress = new Progress<int>(value => MergeProgress = value);
                string outputPath = await _pdfService.MergePdfsAsync(PdfFiles.ToList(), progress);
                
                Logger.Log($"Merge completed successfully. Output file: {outputPath}");
                
                MessageBox.Show($"PDFs merged successfully!\nSaved to: {outputPath}", 
                    "Merge Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                
                if (AutoOpenAfterMerge)
                {
                    FileHelper.OpenFile(outputPath);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error merging PDFs", ex);
                MessageBox.Show($"Error merging PDFs: {ex.Message}", 
                    "Merge Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsMerging = false;
                MergeProgress = 0;
            }
        }
    }
} 