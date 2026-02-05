namespace TomboViewer.Core
{
    // Application preferences storage
    public class PrefsData
    {
        public string BaseDirectory { get; set; }
        public int EditMode { get; set; }
        public string ExtEditorBinary { get; set; }
        public string ExtEditorFlags { get; set; }
        public int OpenMode { get; set; }
        public string ExtOpenerBinary { get; set; }
        public string ExtOpenerFlags { get; set; }
        
        public PrefsData()
        {
            BaseDirectory = ComputeDefaultBase();
            EditMode = 1;
            ExtEditorBinary = "";
            ExtEditorFlags = "";
            OpenMode = 1;
            ExtOpenerBinary = "";
            ExtOpenerFlags = "";
        }
        
        private static string ComputeDefaultBase()
        {
            var docs = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            return System.IO.Path.Combine(docs, "TomboData");
        }
        
        public PrefsData Duplicate()
        {
            return new PrefsData
            {
                BaseDirectory = this.BaseDirectory,
                EditMode = this.EditMode,
                ExtEditorBinary = this.ExtEditorBinary,
                ExtEditorFlags = this.ExtEditorFlags,
                OpenMode = this.OpenMode,
                ExtOpenerBinary = this.ExtOpenerBinary,
                ExtOpenerFlags = this.ExtOpenerFlags
            };
        }
        
        public void ResetToDefaults()
        {
            BaseDirectory = ComputeDefaultBase();
            EditMode = 1;
            ExtEditorBinary = "";
            ExtEditorFlags = "";
            OpenMode = 1;
            ExtOpenerBinary = "";
            ExtOpenerFlags = "";
        }
    }
}
