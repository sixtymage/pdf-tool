using System;
using System.IO;

namespace PdfMerger.Models
{
    public class PdfFile
    {
        public string FilePath { get; set; }
        public string FileName => Path.GetFileName(FilePath);
        public long FileSize { get; set; }
        public DateTime LastModified { get; set; }

        public PdfFile(string filePath)
        {
            FilePath = filePath;
            
            var fileInfo = new FileInfo(filePath);
            FileSize = fileInfo.Length;
            LastModified = fileInfo.LastWriteTime;
        }

        public override string ToString()
        {
            return FileName;
        }
    }
} 