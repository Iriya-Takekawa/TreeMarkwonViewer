using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TomboViewer.Engine
{
    // File I/O operations handler
    public class FileOps
    {
        public async Task<string> LoadTextAsync(string path)
        {
            if (!File.Exists(path))
                return "[File not found]";
            
            try
            {
                using var reader = new StreamReader(path, Encoding.UTF8);
                return await reader.ReadToEndAsync();
            }
            catch (Exception e)
            {
                return $"[Error: {e.Message}]";
            }
        }
        
        public async Task<string> GenerateNewFileAsync(string baseDir, string title)
        {
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);
            
            var timestamp = DateTime.Now;
            var dateStr = timestamp.ToString("yyyyMMdd_HHmmss");
            var cleanTitle = CleanupTitle(title);
            var filename = $"{dateStr}_{cleanTitle}.md";
            var fullPath = Path.Combine(baseDir, filename);
            
            var content = new StringBuilder();
            content.AppendLine($"# {title}");
            content.AppendLine();
            content.AppendLine($"Created: {timestamp:yyyy-MM-dd HH:mm:ss}");
            content.AppendLine();
            content.AppendLine("---");
            content.AppendLine();
            
            using var writer = new StreamWriter(fullPath, false, Encoding.UTF8);
            await writer.WriteAsync(content.ToString());
            
            return fullPath;
        }
        
        private string CleanupTitle(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "untitled";
            
            var invalidChars = Path.GetInvalidFileNameChars();
            var result = new StringBuilder();
            
            foreach (var c in raw)
            {
                if (Array.IndexOf(invalidChars, c) == -1)
                    result.Append(c);
                else
                    result.Append('_');
            }
            
            var cleaned = result.ToString().Trim();
            return string.IsNullOrEmpty(cleaned) ? "untitled" : cleaned;
        }
        
        public bool CheckPathValid(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            
            try
            {
                Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public void EnsureDirExists(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}
