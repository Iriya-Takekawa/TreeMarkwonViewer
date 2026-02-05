using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using TomboViewer.Core;
using TomboViewer.Engine;

namespace TomboViewer.UI
{
    public class SettingsScreenVm : INotifyPropertyChanged
    {
        private readonly PrefsManager _prefsMan;
        private PrefsData _working;
        
        private string _baseDir;
        private int _editChoice;
        private string _editorBin;
        private string _editorArgs;
        private int _openChoice;
        private string _openerBin;
        private string _openerArgs;
        
        public SettingsScreenVm()
        {
            _prefsMan = new PrefsManager();
            
            CmdBrowseEditor = new Command(async () => await PickEditorAsync());
            CmdBrowseOpener = new Command(async () => await PickOpenerAsync());
            CmdBrowseDataDir = new Command(async () => await PickDataDirAsync());
            CmdRestoreDefaults = new Command(SetDefaults);
            CmdSavePrefs = new Command(async () => await SaveAndExitAsync());
            CmdCancelEdit = new Command(async () => await DiscardAndExitAsync());
            
            Task.Run(async () => await LoadPrefsAsync());
        }
        
        public string BaseDir
        {
            get => _baseDir;
            set { _baseDir = value; RaisePropertyChanged(); }
        }
        
        public int EditChoice
        {
            get => _editChoice;
            set { _editChoice = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(IsExtEditorActive)); }
        }
        
        public bool IsExtEditorActive => EditChoice == 2;
        
        public string EditorBin
        {
            get => _editorBin;
            set { _editorBin = value; RaisePropertyChanged(); }
        }
        
        public string EditorArgs
        {
            get => _editorArgs;
            set { _editorArgs = value; RaisePropertyChanged(); }
        }
        
        public int OpenChoice
        {
            get => _openChoice;
            set { _openChoice = value; RaisePropertyChanged(); RaisePropertyChanged(nameof(IsExtOpenerActive)); }
        }
        
        public bool IsExtOpenerActive => OpenChoice == 2;
        
        public string OpenerBin
        {
            get => _openerBin;
            set { _openerBin = value; RaisePropertyChanged(); }
        }
        
        public string OpenerArgs
        {
            get => _openerArgs;
            set { _openerArgs = value; RaisePropertyChanged(); }
        }
        
        public ICommand CmdBrowseEditor { get; }
        public ICommand CmdBrowseOpener { get; }
        public ICommand CmdBrowseDataDir { get; }
        public ICommand CmdRestoreDefaults { get; }
        public ICommand CmdSavePrefs { get; }
        public ICommand CmdCancelEdit { get; }
        
        private async Task LoadPrefsAsync()
        {
            _working = await _prefsMan.LoadAsync();
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                BaseDir = _working.BaseDirectory;
                EditChoice = _working.EditMode;
                EditorBin = _working.ExtEditorBinary;
                EditorArgs = _working.ExtEditorFlags;
                OpenChoice = _working.OpenMode;
                OpenerBin = _working.ExtOpenerBinary;
                OpenerArgs = _working.ExtOpenerFlags;
            });
        }
        
        private async Task PickEditorAsync()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select External Editor"
                });
                
                if (result != null)
                    EditorBin = result.FullPath;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Pick failed: {ex.Message}", "OK");
            }
        }
        
        private async Task PickOpenerAsync()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Folder Opener"
                });
                
                if (result != null)
                    OpenerBin = result.FullPath;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Pick failed: {ex.Message}", "OK");
            }
        }
        
        private async Task PickDataDirAsync()
        {
            try
            {
                var result = await FolderPicker.PickAsync(default);
                if (result.IsSuccessful)
                    BaseDir = result.Folder.Path;
            }
            catch
            {
                var input = await Shell.Current.DisplayPromptAsync(
                    "Data Folder",
                    "Enter path:",
                    "OK",
                    "Cancel",
                    BaseDir
                );
                
                if (!string.IsNullOrWhiteSpace(input))
                    BaseDir = input;
            }
        }
        
        private void SetDefaults()
        {
            var defaults = new PrefsData();
            BaseDir = defaults.BaseDirectory;
            EditChoice = defaults.EditMode;
            EditorBin = defaults.ExtEditorBinary;
            EditorArgs = defaults.ExtEditorFlags;
            OpenChoice = defaults.OpenMode;
            OpenerBin = defaults.ExtOpenerBinary;
            OpenerArgs = defaults.ExtOpenerFlags;
        }
        
        private async Task SaveAndExitAsync()
        {
            _working.BaseDirectory = BaseDir;
            _working.EditMode = EditChoice;
            _working.ExtEditorBinary = EditorBin ?? "";
            _working.ExtEditorFlags = EditorArgs ?? "";
            _working.OpenMode = OpenChoice;
            _working.ExtOpenerBinary = OpenerBin ?? "";
            _working.ExtOpenerFlags = OpenerArgs ?? "";
            
            await _prefsMan.SaveAsync(_working);
            await Shell.Current.GoToAsync("..");
        }
        
        private async Task DiscardAndExitAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
