# Implementation Notes

## Unique Architectural Decisions

This TomboViewer implementation employs several non-standard patterns:

### 1. Fixed Array Storage Pattern
**Decision**: Use `FsNode[]` instead of `List<FsNode>` for child storage

**Rationale**:
- Tree structures rarely modify after initial construction
- Arrays provide O(1) access without list abstraction overhead
- Reduces memory allocations during tree traversal
- Immutable structure prevents accidental modifications

**Implementation**:
```csharp
private FsNode[] _descendants;

public void AttachDescendant(FsNode child)
{
    var temp = new FsNode[_descendants.Length + 1];
    Array.Copy(_descendants, temp, _descendants.Length);
    temp[_descendants.Length] = child;
    _descendants = temp;
}
```

### 2. Recursive Flattening vs Iterative Stack
**Decision**: Use recursive array concatenation for tree flattening

**Alternatives Rejected**:
- Iterative with explicit Stack<FsNode>
- LINQ SelectMany with recursion
- Visitor pattern with callback

**Benefits**:
- Natural representation of tree traversal
- Easier to understand and maintain
- Leverages call stack instead of heap-allocated stack
- Direct array manipulation without intermediate collections

### 3. Action Delegates for Change Notification
**Decision**: Custom `Action NotifyChange` instead of INotifyPropertyChanged

**Rationale**:
- Tree nodes don't need full property change infrastructure
- Single action per node is sufficient
- Avoids string-based property names
- Reduces boilerplate code in FsNode

### 4. Timestamp-Based File Naming
**Decision**: Format `yyyyMMdd_HHmmss_title.md` for new files

**Benefits**:
- Natural chronological sorting
- Unique filenames without collision checking
- Easy to parse creation time from filename
- Human-readable format

### 5. Direct Process Invocation
**Decision**: Use Process.Start directly for external tools

**Alternatives Rejected**:
- Plugin architecture
- MEF or dependency injection
- Custom tool abstraction layer

**Benefits**:
- Simple and straightforward
- Works across Windows and Android
- No additional abstractions needed
- Users control tool configuration

## Code Statistics

- **Total Lines**: ~990 lines of original C# code
- **Classes**: 11 core classes
- **Patterns**: MVVM, Repository, Factory
- **Dependencies**: Minimal (only .NET MAUI framework)

## Testing Strategy

### Unit Testing (To Be Added)
- FsNode flattening algorithm
- TreeConstructor directory scanning
- FileOps sanitization logic
- Converter transformations

### Integration Testing (To Be Added)
- PrefsManager JSON persistence
- External tool invocation
- File creation workflow

### Manual Testing Checklist
- [ ] Tree expansion/collapse on Windows
- [ ] Tree expansion/collapse on Android
- [ ] File selection and preview
- [ ] New document creation
- [ ] External editor launch
- [ ] Folder opener launch
- [ ] Settings persistence
- [ ] Path validation
- [ ] Unicode filename support

## Performance Characteristics

### Memory Usage
- **Per Node**: ~80 bytes + string allocations
- **Tree with 1000 nodes**: ~100KB
- **Flattened display list**: Temporary array allocation only

### Time Complexity
- **Tree Construction**: O(n) where n = total files/folders
- **Flattening**: O(m) where m = visible nodes
- **Node Expansion**: O(m) for display list rebuild
- **File Selection**: O(1) with direct reference

## Future Enhancements

### Planned Features
1. Internal markdown editor with syntax highlighting
2. Search functionality across all documents
3. Tag-based organization
4. Export to different formats
5. Sync with cloud storage

### Architecture Improvements
1. Async tree construction with progress reporting
2. Virtual scrolling for large trees
3. Incremental tree updates instead of full rebuild
4. Cached file content with invalidation

## Platform-Specific Notes

### Windows
- Full filesystem access
- Native file picker dialogs
- Process.Start works for all .exe files

### Android
- Scoped storage limitations
- Special handling for external storage
- May need permissions for external tools

## Lessons Learned

1. **Array-based storage** works well for read-heavy tree structures
2. **Action delegates** provide simpler notification for specific use cases
3. **Recursive flattening** is clearer than iterative for tree traversal
4. **Timestamp prefixes** eliminate filename collision edge cases
5. **Direct tool invocation** is simpler than abstraction layers

## Contributing

When extending this codebase:
- Maintain the array-based storage pattern for consistency
- Use Action delegates for simple notifications
- Follow the existing recursive patterns
- Keep external dependencies minimal
- Write unit tests for new algorithms
