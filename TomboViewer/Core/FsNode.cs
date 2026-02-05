using System;
using System.Linq;

namespace TomboViewer.Core
{
    // Filesystem node using linked structure with bidirectional navigation
    public class FsNode
    {
        private FsNode[] _descendants;
        private bool _viewExpanded;
        
        public FsNode(string fullUri, bool isFolder)
        {
            FullUri = fullUri;
            IsFolder = isFolder;
            _descendants = Array.Empty<FsNode>();
            Ancestor = null;
            TreeDepth = 0;
            MarkedForView = false;
        }
        
        public string FullUri { get; }
        public bool IsFolder { get; }
        public bool MarkedForView { get; set; }
        public int TreeDepth { get; set; }
        public FsNode Ancestor { get; set; }
        
        public string LeafName
        {
            get
            {
                var idx = Math.Max(FullUri.LastIndexOf('/'), FullUri.LastIndexOf('\\'));
                return idx >= 0 ? FullUri.Substring(idx + 1) : FullUri;
            }
        }
        
        public bool ViewExpanded
        {
            get => _viewExpanded;
            set
            {
                _viewExpanded = value;
                NotifyChange?.Invoke();
            }
        }
        
        public Action NotifyChange { get; set; }
        
        public FsNode[] Descendants => _descendants;
        
        public void AttachDescendant(FsNode child)
        {
            child.Ancestor = this;
            child.TreeDepth = TreeDepth + 1;
            
            var temp = new FsNode[_descendants.Length + 1];
            Array.Copy(_descendants, temp, _descendants.Length);
            temp[_descendants.Length] = child;
            _descendants = temp;
        }
        
        public void FlipExpansion() => ViewExpanded = !ViewExpanded;
        
        public FsNode[] FlattenToArray()
        {
            var collector = new System.Collections.Generic.List<FsNode> { this };
            
            if (ViewExpanded && _descendants.Length > 0)
            {
                foreach (var child in _descendants)
                {
                    collector.AddRange(child.FlattenToArray());
                }
            }
            
            return collector.ToArray();
        }
    }
}
