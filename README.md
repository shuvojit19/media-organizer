# Media Organizer

A WPF Windows desktop application for organizing photos and videos into year/month folders by creation date, and finding/managing duplicate files.

## Features

### 📁 Photo & Video Organization
- **Automatic Sorting**: Organizes media files into a Year/Month folder structure based on creation/capture date
- **EXIF Support**: Reads EXIF metadata from photos to extract the actual capture date
- **Flexible Media Detection**: Supports common photo formats (.jpg, .jpeg, .png, .gif, .bmp, .heic) and video formats (.mp4, .mov, .avi, .mkv, .wmv)
- **Copy or Move**: Choose between copying or moving files to the target location
- **Smart Naming**: Automatically handles filename conflicts by appending numbers

### 🔍 Duplicate File Detection
- **Hash-Based Detection**: Uses SHA256 hashing to identify duplicate files
- **Size Optimization**: First groups files by size to improve performance
- **Visual Management**: DataGrid interface to review and select duplicates
- **Safe Deletion**: Confirmation dialogs before deleting any files
- **Batch Selection**: Mark all duplicates in a group except the first one

## Project Structure

```
MediaOrganizer/
├── MediaOrganizer.csproj          # Project file
├── MainWindow.xaml                # UI layout
├── MainWindow.xaml.cs             # UI logic
├── DuplicateRow.cs                # Data model for duplicates
└── Core/
    ├── MediaHelper.cs             # Media type detection & date extraction
    ├── Organizer.cs               # File organization logic
    └── DuplicateFinder.cs         # Duplicate detection logic
```

## Getting Started

### Prerequisites
- Windows 10 or later
- .NET 7.0 or later
- Visual Studio 2022 (or any C# IDE)

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/shuvojit19/media-organizer.git
   cd media-organizer
   ```

2. **Open in Visual Studio**
   - Open `MediaOrganizer/MediaOrganizer.csproj` in Visual Studio 2022

3. **Restore NuGet Packages**
   - The project uses `MetadataExtractor` NuGet package for EXIF reading
   - Visual Studio will automatically restore packages on build

4. **Build and Run**
   - Press `F5` or click "Start" to run the application
   - Or build an executable via `Build → Build Solution`

### Usage

#### Organizing Files

1. **Select Source Folder**: Click "Browse..." next to "Source folder" and select the folder containing your photos/videos
2. **Select Target Folder**: Click "Browse..." next to "Target folder" and select where you want the organized files
3. **Choose Operation**:
   - **Copy instead of move**: Check this to copy files instead of moving them
4. **Start Organization**: Click "Organize Photos & Videos"

Files will be organized into this structure:
```
Target/
├── Photos/
│   ├── 2023/
│   │   ├── 01 - January/
│   │   ├── 02 - February/
│   │   └── ...
│   └── 2024/
│       └── ...
└── Videos/
    ├── 2023/
    └── ...
```

#### Finding and Deleting Duplicates

1. **Scan for Duplicates**: Click "Scan for Duplicates"
   - The application will scan the target folder for duplicate files
   - Duplicates are grouped and displayed in the grid below

2. **Review Duplicates**:
   - Each group shows duplicate files with the same hash
   - Sort and filter to find the duplicates you want to manage

3. **Select Duplicates**:
   - **Mark all except first in each group**: Automatically selects all duplicates except the first file in each group
   - Or manually check individual files you want to delete

4. **Delete**: Click "Delete selected duplicates" and confirm

## Technology Stack

- **Language**: C# (.NET 7.0)
- **UI Framework**: WPF (Windows Presentation Foundation)
- **Libraries**:
  - `MetadataExtractor` v2.8.1 - EXIF metadata reading for accurate photo dates
  - `System.Security.Cryptography` - SHA256 hashing for duplicate detection

## How It Works

### Photo/Video Organization
1. Scans source folder for all image and video files
2. For photos with EXIF data, extracts the `DateTimeOriginal` tag
3. Falls back to file creation/modification date if EXIF is unavailable
4. Creates folder structure: `Photos/YYYY/MM - MonthName/` or `Videos/YYYY/MM - MonthName/`
5. Moves or copies files, handling filename conflicts automatically

### Duplicate Detection
1. Reads all files from the target folder
2. Groups by file size (optimization step)
3. For each size group, computes SHA256 hash of file contents
4. Groups files by hash value
5. Any hash appearing more than once indicates duplicates

## Building an Executable

To create a standalone `.exe` file:

1. In Visual Studio, go to `File → Publish → Create new publish profile`
2. Choose "Folder" as the target
3. Set output to your desired folder
4. Click "Publish"
5. The `.exe` will be in the published folder (can be run independently)

## Notes

- **EXIF Limitations**: Some edited photos or screenshots may not have reliable EXIF dates; the app falls back to file system dates
- **Performance**: Duplicate detection with SHA256 hashing is thorough but may take time on very large folders
- **Backup**: Always backup important files before running the organize or delete operations
- **File Conflicts**: If files with the same name already exist in the target location, they are renamed with a suffix like `(1)`, `(2)`, etc.

## License

This project is open source. Feel free to use, modify, and distribute as needed.

## Contributing

Contributions are welcome! Feel free to submit issues or pull requests to improve the application.
