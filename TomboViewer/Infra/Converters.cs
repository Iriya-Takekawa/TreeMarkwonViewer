using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace TomboViewer.Infra
{
    // Converts tree depth to indent width
    public class DepthToPixelsConverter : IValueConverter
    {
        public object Convert(object val, Type targetType, object param, CultureInfo culture)
        {
            if (val is int depth)
                return depth * 20;
            return 0;
        }
        
        public object ConvertBack(object val, Type targetType, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    // Converts folder flag to font weight
    public class FolderToWeightConverter : IValueConverter
    {
        public object Convert(object val, Type targetType, object param, CultureInfo culture)
        {
            if (val is bool isFolder && isFolder)
                return FontAttributes.Bold;
            return FontAttributes.None;
        }
        
        public object ConvertBack(object val, Type targetType, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    // Converts folder flag to icon
    public class FolderToIconConverter : IValueConverter
    {
        public object Convert(object val, Type targetType, object param, CultureInfo culture)
        {
            if (val is bool isFolder)
                return isFolder ? "📁" : "📄";
            return "📄";
        }
        
        public object ConvertBack(object val, Type targetType, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    // Converts expanded state to glyph
    public class ExpandedToSymbolConverter : IValueConverter
    {
        public object Convert(object val, Type targetType, object param, CultureInfo culture)
        {
            if (val is bool expanded)
                return expanded ? "▼" : "▶";
            return "▶";
        }
        
        public object ConvertBack(object val, Type targetType, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    // Converts selection state to background
    public class SelectedToBgConverter : IValueConverter
    {
        public object Convert(object val, Type targetType, object param, CultureInfo culture)
        {
            if (val is bool selected && selected)
                return Colors.LightBlue;
            return Colors.Transparent;
        }
        
        public object ConvertBack(object val, Type targetType, object param, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
