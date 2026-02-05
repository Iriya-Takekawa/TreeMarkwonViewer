using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TomboViewer.Core;

namespace TomboViewer.Engine
{
    // Persistent preferences management
    public class PrefsManager
    {
        private readonly string _prefsFile;
        private PrefsData _cached;
        
        public PrefsManager()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDir = Path.Combine(appData, "TomboViewer");
            Directory.CreateDirectory(appDir);
            _prefsFile = Path.Combine(appDir, "prefs.json");
            _cached = new PrefsData();
        }
        
        public async Task<PrefsData> LoadAsync()
        {
            if (!File.Exists(_prefsFile))
            {
                _cached = new PrefsData();
                await SaveAsync(_cached);
                return _cached;
            }
            
            try
            {
                var json = await File.ReadAllTextAsync(_prefsFile);
                _cached = JsonSerializer.Deserialize<PrefsData>(json) ?? new PrefsData();
            }
            catch
            {
                _cached = new PrefsData();
            }
            
            return _cached;
        }
        
        public async Task SaveAsync(PrefsData prefs)
        {
            _cached = prefs;
            var json = JsonSerializer.Serialize(prefs, new JsonSerializerOptions 
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(_prefsFile, json);
        }
        
        public PrefsData GetCached() => _cached;
        
        public async Task InvokeEditorAsync(string filePath)
        {
            if (_cached.EditMode == 1)
            {
                await Microsoft.Maui.Controls.Shell.Current.GoToAsync(
                    $"editor?filepath={Uri.EscapeDataString(filePath)}");
                return;
            }
            
            if (string.IsNullOrWhiteSpace(_cached.ExtEditorBinary))
                return;
            
            var args = _cached.ExtEditorFlags.Replace("%file%", filePath);
            
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _cached.ExtEditorBinary,
                    Arguments = args,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Shell.Current.DisplayAlert(
                    "Error", $"Editor failed: {ex.Message}", "OK");
            }
        }
        
        public async Task InvokeFolderOpenerAsync(string dirPath)
        {
            if (_cached.OpenMode == 1)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dirPath,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
                catch (Exception ex)
                {
                    await Microsoft.Maui.Controls.Shell.Current.DisplayAlert(
                        "Error", $"Open failed: {ex.Message}", "OK");
                }
                return;
            }
            
            if (string.IsNullOrWhiteSpace(_cached.ExtOpenerBinary))
                return;
            
            var args = _cached.ExtOpenerFlags.Replace("%path%", dirPath);
            
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = _cached.ExtOpenerBinary,
                    Arguments = args,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Shell.Current.DisplayAlert(
                    "Error", $"Opener failed: {ex.Message}", "OK");
            }
        }
    }
}
