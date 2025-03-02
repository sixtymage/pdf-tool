## **Updated Step-by-Step Blueprint for .NET 9 and C#**

### **Phase 1: Project Setup**
1. **Create a .NET 9 WPF Project**
   - Initialize a new **.NET 9 WPF project** using `dotnet new wpf`.
   - Set up **MVVM architecture** (Model-View-ViewModel).
   - Install dependencies like `PdfSharp` or `iTextSharp` for PDF handling.

2. **Basic UI Skeleton**
   - Create a main window with placeholders for:
     - **File selection**
     - **Drag-and-drop area**
     - **List of selected PDFs**
     - **Merge button**

---

### **Phase 2: Core Features**
3. **File Selection**
   - Implement an **Open File Dialog** to select PDFs.
   - Show selected files in a **ListView**.
   - Enable removing files from the list.

4. **Drag-and-Drop Support**
   - Enable users to drag PDFs into the app window.
   - Validate file types and show errors if needed.

5. **Reordering & Removing Files**
   - Implement **drag-to-reorder**.
   - Add "Move Up" and "Move Down" buttons.
   - Add "Remove" and "Clear All" buttons.

---

### **Phase 3: PDF Merging and File Handling**
6. **Implement PDF Merging Logic**
   - Merge selected PDFs in order using `PdfSharp` or `iTextSharp`.
   - Show a **progress bar** during merging.

7. **Save Merged PDF**
   - Save to the desktop with an **auto-incremented filename**.
   - Handle **file overwrites and errors**.

8. **Auto-Open Merged File**
   - Launch the **default PDF viewer** after merging.
   - Add a **checkbox option** to enable/disable this.

---

### **Phase 4: Enhancements & Stability**
9. **Error Handling & User Feedback**
   - Show **error messages** for invalid files or merge failures.
   - Add **a confirmation popup** when overwriting files.

10. **Logging System**
    - Log **selected files, merge actions, and errors**.
    - Save logs in a local file.

11. **Auto-Update Feature**
    - Check for **updates on startup**.
    - **Download and install** updates when closing the app.

---

## **Breaking Into Small, Iterative Chunks**
I'll now refine this into **smaller development tasks**:

### **Iteration 1: Basic Application Setup**
- Create a **WPF .NET 9 project**.
- Set up a **MainWindow.xaml** with a **ListView** for PDFs.

### **Iteration 2: File Selection**
- Implement an **Open File Dialog**.
- Add selected PDFs to a **ListView**.
- Enable **removing files from the list**.

### **Iteration 3: Drag-and-Drop Support**
- Detect **dragged files** and add them to the list.
- **Validate PDFs** and show an error message if invalid.

### **Iteration 4: File List Management**
- Implement **drag-to-reorder**.
- Add **"Remove" and "Clear All"** buttons.

### **Iteration 5: PDF Merging**
- Implement merging with **PdfSharp** or **iTextSharp**.
- Add a **progress bar**.

### **Iteration 6: Saving & Auto-Opening**
- Save with an **auto-incremented filename**.
- **Open the merged file** after completion.

### **Iteration 7: Error Handling**
- Handle **file read errors** and **corrupted PDFs**.

### **Iteration 8: Logging System**
- Log **selected files, merge actions, and errors**.

### **Iteration 9: Auto-Update System**
- Implement **update checking and auto-download**.

---

## **Updated LLM Code-Generation Prompts for .NET 9**
Now, I'll generate **structured prompts** to guide AI in generating **C# and .NET 9 WPF code**.

### **Prompt 1: Create a Minimal WPF App**
```text
Create a minimal .NET 9 WPF application with:
- A main window titled "PDF Merger"
- A `ListView` to display selected PDFs
- A "Select PDFs" button
- A basic `ViewModel` for MVVM pattern
```

### **Prompt 2: Implement File Picker**
```text
Modify the WPF app to:
- Add an "Open File" button that opens a file dialog
- Allow **multiple PDF selection**
- Display selected files in a `ListView`
- Show a "Remove" button next to each file
```

### **Prompt 3: Drag-and-Drop Support**
```text
Enable drag-and-drop in the WPF app:
- Detect when PDF files are dragged into the window
- Validate that only PDFs are accepted
- Add valid files to the `ListView`
- Show an error message for invalid files
```

### **Prompt 4: Implement File Reordering**
```text
Modify the WPF app to allow reordering of selected PDFs:
- Implement **drag-to-reorder** within the `ListView`
- Add "Move Up" and "Move Down" buttons next to each file
```

### **Prompt 5: Implement PDF Merging**
```text
Implement PDF merging using `PdfSharp`:
- Add a "Merge PDFs" button
- Merge selected PDFs in order
- Save the merged file as "Merged_1.pdf" on the desktop
- If "Merged_1.pdf" exists, increment (Merged_2.pdf, etc.)
- Display a **progress bar** while merging
- Show a confirmation message when done
```

### **Prompt 6: Auto-Open Merged PDF**
```text
Modify the WPF app to automatically open the merged PDF:
- Launch the **default PDF viewer**
- Provide a **checkbox setting** to enable/disable this
```

### **Prompt 7: Error Handling**
```text
Improve error handling in the WPF app:
- Show an **error message** if merging fails
- Handle **locked or corrupted PDFs** gracefully
- Ensure invalid files are rejected before merging
```

### **Prompt 8: Logging System**
```text
Implement a **logging system** in the WPF app:
- Log each **selected file**
- Log **successful merges and errors**
- Save logs as a **text file** in the application folder
```

### **Prompt 9: Auto-Update System**
```text
Implement an **auto-update feature** in the WPF app:
- On startup, check a **remote server** for the latest version
- If an update is available, **download it in the background**
- Install updates when the application exits
- Ensure the update process does not **interrupt ongoing tasks**
```
