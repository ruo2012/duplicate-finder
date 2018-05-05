using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;

namespace DuplicateFinder.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private string _selectedPath;

        private bool _isRunning;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            BrowseCommand = new RelayCommand(Browse);
            StartCommand = new RelayCommand(Start, CanStart);
            TotalFileCount = 1;
            ProcessedFileCount = 0;
            ProcessingFilePath = string.Empty;
        }

        /// <summary>
        /// Target path selected by user
        /// </summary>
        public string SelectedPath
        {
            get { return _selectedPath; }
            private set
            {
                Set(ref _selectedPath, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Browse command for path selection
        /// </summary>
        public RelayCommand BrowseCommand { get; private set; }

        /// <summary>
        /// Start command for duplicate search
        /// </summary>
        public RelayCommand StartCommand { get; private set; }

        public bool IsRunning
        {
            get { return _isRunning; }
            private set
            {
                Set(ref _isRunning, value);
                StartCommand.RaiseCanExecuteChanged();
            }
        }

        private int _totalFileCount;
        public int TotalFileCount
        {
            get { return _totalFileCount; }
            private set { Set(ref _totalFileCount, value); }
        }

        private int _processedFileCount;
        public int ProcessedFileCount
        {
            get { return _processedFileCount; }
            private set { Set(ref _processedFileCount, value); }
        }

        private string _processingFilePath;
        public string ProcessingFilePath
        {
            get { return _processingFilePath; }
            private set { Set(ref _processingFilePath, value); }
        }

        private Core Core { get; set; }

        private void Browse()
        {
            WinForms.FolderBrowserDialog folderBrowserDialog = new WinForms.FolderBrowserDialog();

            folderBrowserDialog.ShowNewFolderButton = false;

            WinForms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();
            
            if(dialogResult == WinForms.DialogResult.OK && !folderBrowserDialog.SelectedPath.IsNullOrEmpty())
            {
                SelectedPath = folderBrowserDialog.SelectedPath;
            }
        }

        private void Start()
        {
            Task.Factory.StartNew(() =>
            {
                IsRunning = true;

                Core = new Core();
                Core.RootDirectoryPath = SelectedPath;
                Core.OnProgressChanged += OnProgressChanged;

                Core.FindDuplicates();

                IEnumerable<DuplicateGroup> duplicates = Core.DuplicateGroups;

                foreach (DuplicateGroup duplicate in duplicates)
                {
                    Debug.WriteLine(duplicate);
                }

                TotalFileCount = 1;
                ProcessedFileCount = 0;
                ProcessingFilePath = string.Empty;

                IsRunning = false;
            });
        }

        private bool CanStart()
        {
            return !SelectedPath.IsNullOrEmpty() && !IsRunning;
        }

        private void OnProgressChanged()
        {
            TotalFileCount = Core.TotalFileCount;
            ProcessedFileCount = Core.ProcessedFileCount;
            ProcessingFilePath = Core.ProcessingFilePath;
        }
    }
}