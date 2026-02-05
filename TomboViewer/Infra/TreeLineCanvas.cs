using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TomboViewer.Core;

namespace TomboViewer.Infra
{
    // Custom graphics view for tree lines
    public class TreeLineCanvas : GraphicsView
    {
        public static readonly BindableProperty NodeProperty = BindableProperty.Create(
            nameof(Node),
            typeof(FsNode),
            typeof(TreeLineCanvas),
            null,
            propertyChanged: OnNodeChanged
        );
        
        public FsNode Node
        {
            get => (FsNode)GetValue(NodeProperty);
            set => SetValue(NodeProperty, value);
        }
        
        private static void OnNodeChanged(BindableObject obj, object oldVal, object newVal)
        {
            if (obj is TreeLineCanvas canvas)
                canvas.Invalidate();
        }
        
        public TreeLineCanvas()
        {
            Drawable = new TreeDrawing(this);
        }
        
        private class TreeDrawing : IDrawable
        {
            private readonly TreeLineCanvas _canvas;
            
            public TreeDrawing(TreeLineCanvas canvas)
            {
                _canvas = canvas;
            }
            
            public void Draw(ICanvas ctx, RectF area)
            {
                var node = _canvas.Node;
                if (node == null || node.TreeDepth == 0)
                    return;
                
                ctx.StrokeColor = Colors.Gray;
                ctx.StrokeSize = 1;
                
                var indentSize = 20f;
                var midY = area.Height / 2;
                
                // Render ancestor vertical lines
                var current = node.Ancestor;
                var level = node.TreeDepth - 1;
                
                while (current != null && level > 0)
                {
                    var xPos = level * indentSize;
                    
                    if (HasNextSibling(current))
                        ctx.DrawLine(xPos, 0, xPos, area.Height);
                    
                    current = current.Ancestor;
                    level--;
                }
                
                // Draw horizontal connector
                var nodeX = node.TreeDepth * indentSize;
                ctx.DrawLine(nodeX, midY, nodeX + indentSize / 2, midY);
                
                // Draw vertical segments
                if (HasNextSibling(node))
                    ctx.DrawLine(nodeX, midY, nodeX, area.Height);
                ctx.DrawLine(nodeX, 0, nodeX, midY);
            }
            
            private bool HasNextSibling(FsNode node)
            {
                if (node == null || node.Ancestor == null)
                    return false;
                
                var siblings = node.Ancestor.Descendants;
                var idx = -1;
                
                for (int i = 0; i < siblings.Length; i++)
                {
                    if (siblings[i] == node)
                    {
                        idx = i;
                        break;
                    }
                }
                
                return idx >= 0 && idx < siblings.Length - 1;
            }
        }
    }
}
