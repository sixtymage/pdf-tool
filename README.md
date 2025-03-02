# PDF Merger

A simple, user-friendly desktop application for merging multiple PDF files into a single document.

## Features

- **Simple Interface**: Clean and intuitive user interface for easy PDF merging.
- **Drag and Drop**: Drag and drop PDF files directly into the application.
- **File Management**: Add, remove, and reorder PDF files before merging.
- **Auto-Open**: Option to automatically open the merged PDF when complete.
- **Progress Tracking**: Visual progress indicator during the merge process.
- **Error Handling**: Robust error handling with user-friendly messages.
- **Logging**: Comprehensive logging for troubleshooting.

## Requirements

- Windows 10/11 (64-bit)
- .NET 9 Runtime

## Installation

1. Download the latest release from the [Releases](https://github.com/yourusername/pdf-merger/releases) page.
2. Extract the ZIP file to a location of your choice.
3. Run `PdfMerger.exe` to start the application.

## Usage

1. **Add PDF Files**: Click "Add Files" or drag and drop PDF files into the application.
2. **Arrange Files**: Use the up/down arrows to reorder the files as needed.
3. **Merge PDFs**: Click "Merge PDFs" to combine the files in the specified order.
4. **Access Result**: The merged PDF will be saved to your desktop with a sequential filename (e.g., `Merged_1.pdf`).

## Building from Source

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/pdf-merger.git
   ```

2. Open the solution in Visual Studio 2022 or later.

3. Build the solution:
   ```
   dotnet build
   ```

4. Run the application:
   ```
   dotnet run
   ```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [PDFsharp](http://www.pdfsharp.net/) - PDF library for .NET
