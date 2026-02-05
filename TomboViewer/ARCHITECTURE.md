# TomboViewer Architecture

## Component Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                        UI Layer                              │
│  ┌────────────────┐              ┌────────────────┐         │
│  │ MainScreenVm   │              │ SettingsScreen │         │
│  │                │              │      Vm        │         │
│  │ - TreeItems    │              │ - BaseDir      │         │
│  │ - ContentDsp   │              │ - EditChoice   │         │
│  │ - Commands     │              │ - Commands     │         │
│  └────────┬───────┘              └────────┬───────┘         │
└───────────┼──────────────────────────────┼─────────────────┘
            │                              │
            │ Uses                   Uses │
            ▼                              ▼
┌───────────────────────────────────────────────────────────┐
│                    Engine Layer                            │
│  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐     │
│  │TreeConstructr│  │   FileOps    │  │PrefsManager │     │
│  │              │  │              │  │             │     │
│  │ - BuildAsync │  │ - LoadText   │  │ - LoadAsync │     │
│  │ - ToFlatArr  │  │ - Generate   │  │ - SaveAsync │     │
│  └──────┬───────┘  └──────┬───────┘  └──────┬──────┘     │
└─────────┼──────────────────┼─────────────────┼────────────┘
          │                  │                  │
          │ Produces         │ Reads/Writes     │ Persists
          ▼                  ▼                  ▼
┌───────────────────────────────────────────────────────────┐
│                     Core Layer                             │
│  ┌────────────┐                      ┌─────────────┐      │
│  │  FsNode    │                      │ PrefsData   │      │
│  │            │                      │             │      │
│  │ - FullUri  │                      │ - BaseDir   │      │
│  │ - IsFolder │                      │ - EditMode  │      │
│  │ - Descndts │                      │ - OpenMode  │      │
│  │ - Flatten  │                      │ - ExtPaths  │      │
│  └────────────┘                      └─────────────┘      │
└───────────────────────────────────────────────────────────┘
          ▲                                      ▲
          │ Rendered By                          │ Bound To
          │                                      │
┌─────────┴──────────────────────────────────────┴──────────┐
│                Infrastructure Layer                        │
│  ┌────────────────┐              ┌────────────────┐       │
│  │TreeLineCanvas  │              │   Converters   │       │
│  │                │              │                │       │
│  │ - Node prop    │              │ - DepthToPixel │       │
│  │ - IDrawable    │              │ - FolderToWgt  │       │
│  │ - TreeDrawing  │              │ - FolderToIcon │       │
│  └────────────────┘              │ - ExpandToSymb │       │
│                                  │ - SelectedToBg │       │
│                                  └────────────────┘       │
└───────────────────────────────────────────────────────────┘
```

## Data Flow

### Application Startup
```
1. PrefsManager.LoadAsync()
   └─> Read prefs.json from AppData
       └─> Return PrefsData or defaults

2. TreeConstructor.BuildAsync(baseDir)
   └─> Scan filesystem recursively
       └─> Create FsNode tree structure

3. FsNode.FlattenToArray()
   └─> Convert tree to flat array
       └─> Populate ObservableCollection<FsNode>

4. UI renders TreeItems with TreeLineCanvas
```

### User Interaction Flow

#### File Selection
```
User taps item
  └─> MainScreenVm.CmdSelectItem
      └─> Clear all MarkedForView flags
          └─> Set selected item.MarkedForView = true
              └─> FileOps.LoadTextAsync(path)
                  └─> Update ContentDisplay property
                      └─> UI shows file content
```

#### Tree Expansion
```
User taps expand glyph
  └─> MainScreenVm.CmdToggleExpand
      └─> FsNode.FlipExpansion()
          └─> Toggle ViewExpanded property
              └─> Trigger NotifyChange action
                  └─> MainScreenVm.RebuildDisplay()
                      └─> FlattenToArray() again
                          └─> Update ObservableCollection
```

#### New Document
```
User taps New button
  └─> Prompt for title
      └─> FileOps.GenerateNewFileAsync()
          └─> Create yyyyMMdd_HHmmss_title.md
              └─> Write template content
                  └─> Refresh tree
                      └─> Show success message
```

#### Settings Save
```
User edits settings → taps Save
  └─> SettingsScreenVm.CmdSavePrefs
      └─> Copy UI values to PrefsData
          └─> PrefsManager.SaveAsync()
              └─> Serialize to JSON
                  └─> Write to prefs.json
                      └─> Navigate back
```

## Rendering Pipeline

### Tree Line Drawing
```
For each FsNode in TreeItems:
  1. TreeLineCanvas bound to Node property
  2. OnNodeChanged triggers Invalidate()
  3. TreeDrawing.Draw() called with ICanvas
  4. Algorithm:
     - Calculate indent: depth * 20px
     - Draw ancestor vertical lines
     - Check HasNextSibling for each ancestor
     - Draw horizontal connector
     - Draw node vertical segments
```

### Value Conversion
```
XAML Binding → Converter → Display Value

Examples:
- TreeDepth (int) → DepthToPixelsConverter → Width (int)
- IsFolder (bool) → FolderToWeightConverter → FontAttributes
- IsFolder (bool) → FolderToIconConverter → String ("📁" or "📄")
- ViewExpanded (bool) → ExpandedToSymbolConverter → String ("▼" or "▶")
- MarkedForView (bool) → SelectedToBgConverter → Color
```

## Memory Model

### Object Graph
```
MainScreenVm
 └─> ObservableCollection<FsNode>
     └─> FsNode (root)
         ├─> FsNode[] _descendants
         │   ├─> FsNode (child 1)
         │   │   └─> FsNode[] ...
         │   └─> FsNode (child 2)
         │       └─> FsNode[] ...
         └─> FsNode Ancestor (back reference)
```

### Lifecycle
- FsNode tree built once per refresh
- Flattened array recreated on expand/collapse
- ObservableCollection updated causes UI redraw
- No node caching - recreated on each refresh

## Extension Points

### Adding New File Types
Edit `TreeConstructor._acceptedExts`:
```csharp
private readonly string[] _acceptedExts = { 
    ".md", ".txt", ".markdown", ".rst", ".adoc" 
};
```

### Custom File Templates
Modify `FileOps.GenerateNewFileAsync()`:
```csharp
var content = new StringBuilder();
content.AppendLine($"# {title}");
content.AppendLine($"Author: {Environment.UserName}");
// Add custom template logic
```

### Additional Converters
Create new IValueConverter in `Converters.cs`:
```csharp
public class CustomConverter : IValueConverter
{
    public object Convert(object val, ...) { ... }
    public object ConvertBack(object val, ...) { ... }
}
```

## Threading Model

- **UI Thread**: MainScreenVm, SettingsScreenVm, UI updates
- **Background Thread**: Tree construction, file I/O
- **Synchronization**: `MainThread.BeginInvokeOnMainThread()` for collection updates
