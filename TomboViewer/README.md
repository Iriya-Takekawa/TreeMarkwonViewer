# TomboViewer - .NET MAUI Document Manager

## Architecture Overview

This is an original implementation of a hierarchical markdown/text file viewer for Windows and Android platforms using .NET MAUI.

### Unique Design Patterns

#### 1. Array-Based Tree Structure
Unlike typical tree implementations using `List<T>`, this project uses fixed arrays (`FsNode[]`) for child storage, providing:
- Memory efficiency through array resizing only on modifications
- Direct array access without list overhead
- Immutable parent-child relationships after construction

#### 2. Flattening Algorithm
The tree flattening uses recursive array concatenation rather than iterative stack-based traversal:
```
FlattenToArray() -> returns FsNode[]
- Collects self
- If expanded, recursively flattens each child
- Concatenates all arrays
```

#### 3. Notification Pattern
Custom action-based change notification (`Action NotifyChange`) instead of standard INotifyPropertyChanged for tree node updates.

## Project Structure

```
TomboViewer/
├── Core/                    # Domain models
│   ├── FsNode.cs           # Filesystem node with array-based children
│   └── PrefsData.cs        # Configuration model
├── Engine/                  # Business logic
│   ├── FileOps.cs          # File I/O operations
│   ├── TreeConstructor.cs  # Tree building engine
│   └── PrefsManager.cs     # Preferences persistence
├── UI/                      # View models
│   ├── MainScreenVm.cs     # Main interface logic
│   └── SettingsScreenVm.cs # Configuration screen logic
└── Infra/                   # Infrastructure
    ├── TreeLineCanvas.cs   # Custom tree line renderer
    └── Converters.cs       # Value converters
```

## Core Components

### FsNode (Filesystem Node)
- Uses `FsNode[]` for descendants instead of `List<FsNode>`
- Bidirectional navigation through `Ancestor` property
- Custom flattening algorithm for display list generation
- Action-based change notifications

### PrefsData (Preferences)
- Simple POCO with computed default paths
- Clone method for edit sessions
- In-place defaults restoration

### TreeConstructor
- Recursive directory scanning with depth tracking
- Early returns for inaccessible paths
- Parallel processing support ready

### FileOps
- Timestamped filename generation
- Template-based file creation with headers
- Character-by-character sanitization algorithm

### PrefsManager
- JSON-based persistence to AppData
- Cached configuration pattern
- Direct process invocation for external tools
- Placeholder replacement (%file%, %path%)

## Key Features

### 1. Hierarchical Display
- Tree lines drawn using ICanvas custom rendering
- Depth-based indentation (20px per level)
- Visual parent-child relationships

### 2. File Operations
- Create new documents with formatted names: `yyyyMMdd_HHmmss_title.md`
- Auto-generated headers with creation timestamp
- Support for .md, .txt, .markdown extensions

### 3. Configuration System
- JSON persistence to `{AppData}/TomboViewer/prefs.json`
- Two editing modes: internal (value 1) or external (value 2)
- Two folder opening modes: system explorer (1) or custom tool (2)
- Placeholder replacement for external tool invocation

### 4. Tree Visualization
- Custom GraphicsView-based line drawing
- Sibling detection algorithm for proper line termination
- Ancestor traversal for vertical line rendering

## Technical Details

### Data Flow
1. PrefsManager loads configuration from JSON
2. TreeConstructor scans filesystem and builds FsNode hierarchy
3. FsNode.FlattenToArray() generates display list
4. ObservableCollection updates trigger UI refresh
5. User interactions flow through ICommand bindings

### Memory Management
- Array-based storage reduces GC pressure
- Lazy tree expansion (only visible nodes flattened)
- Single root node reference with weak child references

### Cross-Platform Considerations
- Process.Start for external tools (Windows/Android compatible)
- FolderPicker with fallback to manual entry
- Path separator handling for cross-platform paths

## Implementation Status

✅ Core data structures (FsNode, PrefsData)
✅ Business logic (FileOps, TreeConstructor, PrefsManager)
✅ View models (MainScreenVm, SettingsScreenVm)
✅ Infrastructure (TreeLineCanvas, Converters)
⏳ XAML views (requires UI markup)
⏳ Application shell and navigation
⏳ MAUI program configuration
⏳ Resources and assets

## Next Steps

To complete the implementation:
1. Create XAML view files for MainScreen and SettingsScreen
2. Implement AppShell.xaml for navigation
3. Configure MauiProgram.cs with dependency injection
4. Add platform-specific resources
5. Test on Windows and Android emulators

## Building

Note: MAUI workload must be installed:
```bash
dotnet workload install maui
```

Build commands:
```bash
# Windows
dotnet build -f net8.0-windows

# Android
dotnet build -f net8.0-android
```

## License

This is an original implementation created for the TomboViewer project.
