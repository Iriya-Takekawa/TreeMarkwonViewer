using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TomboViewer.Core;

namespace TomboViewer.Engine
{
    // Constructs filesystem tree structures  
    public class TreeConstructor
    {
        private readonly string[] _acceptedExts = { ".md", ".txt", ".markdown" };
        
        public async Task<FsNode> BuildAsync(string root)
        {
            return await Task.Run(() => BuildRecursive(root, 0));
        }
        
        private FsNode BuildRecursive(string path, int depth)
        {
            if (!Directory.Exists(path))
                return null;
            
            var node = new FsNode(path, true)
            {
                TreeDepth = depth,
                ViewExpanded = depth == 0
            };
            
            try
            {
                var subdirs = Directory.GetDirectories(path)
                    .OrderBy(d => Path.GetFileName(d), StringComparer.OrdinalIgnoreCase);
                
                foreach (var subdir in subdirs)
                {
                    var childNode = BuildRecursive(subdir, depth + 1);
                    if (childNode != null)
                        node.AttachDescendant(childNode);
                }
                
                var files = Directory.GetFiles(path)
                    .Where(f => _acceptedExts.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                    .OrderBy(f => Path.GetFileName(f), StringComparer.OrdinalIgnoreCase);
                
                foreach (var file in files)
                {
                    var fileNode = new FsNode(file, false)
                    {
                        TreeDepth = depth + 1
                    };
                    node.AttachDescendant(fileNode);
                }
            }
            catch
            {
                // Skip inaccessible directories
            }
            
            return node;
        }
        
        public FsNode[] ToFlatArray(FsNode root)
        {
            return root?.FlattenToArray() ?? Array.Empty<FsNode>();
        }
    }
}
