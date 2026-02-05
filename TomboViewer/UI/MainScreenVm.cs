using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using TomboViewer.Core;
using TomboViewer.Engine;

namespace TomboViewer.UI
{
    public class MainScreenVm : INotifyPropertyChanged
    {
        private readonly TreeConstructor _treeBuilder;
        private readonly FileOps _fileOps;
        private readonly PrefsManager _prefsMan;
        private FsNode _treeRoot;
        private string _contentDisplay;
        private string _selectedPath;
        
        public MainScreenVm()
        {
            _treeBuilder = new TreeConstructor();
            _fileOps = new FileOps();
            _prefsMan = new PrefsManager();
            
            TreeItems = new ObservableCollection<FsNode>();
            
            CmdRefresh = new Command(async () => await RefreshTreeAsync());
            CmdSelectItem = new Command<FsNode>(async (item) => await OnItemSelectedAsync(item));
            CmdToggleExpand = new Command<FsNode>(item => OnToggleExpand(item));
            CmdNewDoc = new Command(async () => await OnNewDocAsync());
            CmdOpenEditor = new Command(async () => await OnOpenEditorAsync(), () => !string.IsNullOrEmpty(_selectedPath));
            CmdOpenFolder = new Command(async () => await OnOpenFolderAsync());
            CmdGoSettings = new Command(async () => await GoToSettingsAsync());
            
            Task.Run(async () => await InitializeAsync());
        }
        
        public ObservableCollection<FsNode> TreeItems { get; }
        
        public string ContentDisplay
        {
            get => _contentDisplay;
            set { _contentDisplay = value; RaisePropertyChanged(); }
        }
        
        public ICommand CmdRefresh { get; }
        public ICommand CmdSelectItem { get; }
        public ICommand CmdToggleExpand { get; }
        public ICommand CmdNewDoc { get; }
        public ICommand CmdOpenEditor { get; }
        public ICommand CmdOpenFolder { get; }
        public ICommand CmdGoSettings { get; }
        
        private async Task InitializeAsync()
        {
            var prefs = await _prefsMan.LoadAsync();
            _fileOps.EnsureDirExists(prefs.BaseDirectory);
            await RefreshTreeAsync();
        }
        
        private async Task RefreshTreeAsync()
        {
            var prefs = _prefsMan.GetCached();
            _treeRoot = await _treeBuilder.BuildAsync(prefs.BaseDirectory);
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TreeItems.Clear();
                var flatArray = _treeBuilder.ToFlatArray(_treeRoot);
                foreach (var node in flatArray)
                {
                    node.NotifyChange = () => RebuildDisplay();
                    TreeItems.Add(node);
                }
            });
        }
        
        private void RebuildDisplay()
        {
            var flatArray = _treeBuilder.ToFlatArray(_treeRoot);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TreeItems.Clear();
                foreach (var node in flatArray)
                {
                    node.NotifyChange = () => RebuildDisplay();
                    TreeItems.Add(node);
                }
            });
        }
        
        private async Task OnItemSelectedAsync(FsNode item)
        {
            if (item == null) return;
            
            foreach (var node in TreeItems)
                node.MarkedForView = false;
            
            item.MarkedForView = true;
            
            if (!item.IsFolder)
            {
                _selectedPath = item.FullUri;
                ContentDisplay = await _fileOps.LoadTextAsync(item.FullUri);
                ((Command)CmdOpenEditor).ChangeCanExecute();
            }
            else
            {
                _selectedPath = null;
                ContentDisplay = string.Empty;
                ((Command)CmdOpenEditor).ChangeCanExecute();
            }
        }
        
        private void OnToggleExpand(FsNode item)
        {
            if (item != null && item.IsFolder)
                item.FlipExpansion();
        }
        
        private async Task OnNewDocAsync()
        {
            var title = await Shell.Current.DisplayPromptAsync(
                "New Document",
                "Enter title:",
                "Create",
                "Cancel",
                "My Note"
            );
            
            if (string.IsNullOrWhiteSpace(title))
                return;
            
            var prefs = _prefsMan.GetCached();
            var newPath = await _fileOps.GenerateNewFileAsync(prefs.BaseDirectory, title);
            
            await RefreshTreeAsync();
            await Shell.Current.DisplayAlert("Success", $"Created: {System.IO.Path.GetFileName(newPath)}", "OK");
        }
        
        private async Task OnOpenEditorAsync()
        {
            if (!string.IsNullOrEmpty(_selectedPath))
                await _prefsMan.InvokeEditorAsync(_selectedPath);
        }
        
        private async Task OnOpenFolderAsync()
        {
            var prefs = _prefsMan.GetCached();
            await _prefsMan.InvokeFolderOpenerAsync(prefs.BaseDirectory);
        }
        
        private async Task GoToSettingsAsync()
        {
            await Shell.Current.GoToAsync("settings");
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
