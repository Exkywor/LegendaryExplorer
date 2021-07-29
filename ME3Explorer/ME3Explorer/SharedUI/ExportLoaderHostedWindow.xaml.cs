﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using ME3ExplorerCore.Helpers;
using ME3ExplorerCore.Misc;
using ME3ExplorerCore.Packages;
using Microsoft.Win32;
using static ME3Explorer.PackageEditorWPF;

namespace ME3Explorer.SharedUI
{
    /// <summary>
    /// Interaction logic for ExportLoaderHostedWindow.xaml
    /// </summary>
    public partial class ExportLoaderHostedWindow : WPFBase
    {
        public ExportEntry LoadedExport { get; private set; }
        public readonly ExportLoaderControl HostedControl;
        public ObservableCollectionExtended<IndexedName> NamesList { get; } = new ObservableCollectionExtended<IndexedName>();
        public bool SupportsRecents => HostedControl is FileExportLoaderControl;

        private bool _fileHasPendingChanges;
        public bool FileHasPendingChanges
        {
            get => _fileHasPendingChanges;
            set
            {
                SetProperty(ref _fileHasPendingChanges, value);
                OnPropertyChanged(nameof(IsModifiedProxy));
            }
        }
        public bool IsModifiedProxy
        {
            get
            {
                if (LoadedExport != null)
                {
                    return LoadedExport.EntryHasPendingChanges;
                }
                if (HostedControl is FileExportLoaderControl felc)
                {
                    return felc.FileModified;
                }
                return true;
            }
        }

        /// <summary>
        /// Opens ELHW with an export and the specified tool.
        /// </summary>
        /// <param name="hostedControl"></param>
        /// <param name="exportToLoad"></param>
        public ExportLoaderHostedWindow(ExportLoaderControl hostedControl, ExportEntry exportToLoad) : base($"ELHW for {hostedControl.GetType()}")
        {
            DataContext = this;
            this.HostedControl = hostedControl;
            this.LoadedExport = exportToLoad;
            LoadedExport.EntryModifiedChanged += NotifyPendingChangesStatusChanged;
            NamesList.ReplaceAll(LoadedExport.FileRef.Names.Select((name, i) => new IndexedName(i, name))); //we replaceall so we don't add one by one and trigger tons of notifications
            LoadCommands();
            InitializeComponent();
            HostedControl.PoppedOut(Recents_MenuItem);
            RootPanel.Children.Add(hostedControl);
        }

        private void NotifyPendingChangesStatusChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsModifiedProxy));
        }

        /// <summary>
        /// Opens ELFH with a file loader and an optional file.
        /// </summary>
        /// <param name="hostedControl"></param>
        /// <param name="file"></param>
        public ExportLoaderHostedWindow(FileExportLoaderControl hostedControl, string file = null) : base($"ELHW for {hostedControl.GetType()}")
        {
            DataContext = this;
            this.HostedControl = hostedControl;
            hostedControl.ModifiedStatusChanging += NotifyPendingChangesStatusChanged;
            //NamesList.ReplaceAll(LoadedExport.FileRef.Names.Select((name, i) => new IndexedName(i, name))); //we replaceall so we don't add one by one and trigger tons of notifications
            LoadCommands();
            InitializeComponent();
            HostedControl.PoppedOut(Recents_MenuItem);
            RootPanel.Children.Add(hostedControl);
            if (file != null)
            {
                hostedControl.LoadFile(file);
            }
        }

        public ICommand SaveCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand LoadFileCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand ReloadCurrentExportCommand { get; set; }
        private void LoadCommands()
        {
            SaveCommand = new GenericCommand(SavePackage, CanSave);
            SaveAsCommand = new GenericCommand(SavePackageAs, CanSave);
            LoadFileCommand = new GenericCommand(LoadFile, CanLoadFile);
            OpenFileCommand = new GenericCommand(OpenFile, CanLoadFile);
            ReloadCurrentExportCommand = new GenericCommand(ReloadCurrentExport, IsExportLoaded);
        }

        private void ReloadCurrentExport()
        {
            var exp = HostedControl.CurrentLoadedExport;
            HostedControl.UnloadExport();
            HostedControl.LoadExport(exp);
        }

        private bool IsExportLoaded()
        {
            if (HostedControl is FileExportLoaderControl felc && felc.LoadedFile != null) return false;
            if (HostedControl.CurrentLoadedExport != null) return true;
            return false;
        }

        private bool CanSave()
        {
            if (HostedControl is FileExportLoaderControl felc)
            {
                return felc.CanSave();
            }
            else
            {
                return true;
            }
        }

        private void OpenFile()
        {
            if (HostedControl is FileExportLoaderControl felc)
            {
                FileHasPendingChanges = false;
                felc.OpenFile();
            }
        }

        private bool CanLoadFile()
        {
            return HostedControl is FileExportLoaderControl felc && felc.CanLoadFile();
        }

        private void LoadFile()
        {
            var felc = HostedControl as FileExportLoaderControl;
            felc.OpenFile();
        }

        private void SavePackageAs()
        {
            if (HostedControl is FileExportLoaderControl felc)
            {
                felc.SaveAs();
                FileHasPendingChanges = false;
            }
            else
            {
                string extension = Path.GetExtension(Pcc.FilePath);
                SaveFileDialog d = new SaveFileDialog { Filter = $"*{extension}|*{extension}" };
                if (d.ShowDialog() == true)
                {
                    Pcc.Save(d.FileName);
                    MessageBox.Show("Done");
                }
            }
        }

        private void SavePackage()
        {
            if (HostedControl is FileExportLoaderControl felc)
            {
                felc.Save();
                FileHasPendingChanges = false;
            }
            else
            {
                Pcc.Save();
            }
        }

        public string CurrentFile => Pcc != null ? Path.GetFileName(Pcc.FilePath) : "";
        public override void handleUpdate(List<PackageUpdate> updates)
        {
            if (updates.Any(x => x.Change.Has(PackageChange.Name)))
            {
                HostedControl.SignalNamelistAboutToUpdate();
                NamesList.ReplaceAll(Pcc.Names.Select((name, i) => new IndexedName(i, name))); //we replaceall so we don't add one by one and trigger tons of notifications
                HostedControl.SignalNamelistChanged();
            }

            //Put code to reload the export here
            foreach (var update in updates)
            {
                if ((update.Change.Has(PackageChange.Export))
                    && update.Index == LoadedExport.UIndex)
                {
                    HostedControl.LoadExport(LoadedExport); //reload export
                    return;
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                if (LoadedExport != null)
                {
                    // This will register the tool and assign a reference to it.
                    // Since this export is already in memory we will just reference the existing package instead.
                    RegisterPackage(LoadedExport.FileRef); 
                    HostedControl.LoadExport(LoadedExport);
                    OnPropertyChanged(nameof(CurrentFile));
                }
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                if (LoadedExport != null)
                {
                    LoadedExport.EntryModifiedChanged -= NotifyPendingChangesStatusChanged;
                    LoadedExport = null;
                }
                if (HostedControl is FileExportLoaderControl felc)
                {
                    felc.ModifiedStatusChanging -= NotifyPendingChangesStatusChanged;
                }

                HostedControl.Dispose();
            }
        }

        private void ExportLoaderHostedWindow_OnDrop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop) && HostedControl is FileExportLoaderControl felc)
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                if (felc.CanLoadFileExtension(Path.GetExtension(files[0])))
                {
                    felc.LoadFile(files[0]);
                }
            }
        }

        private void ExportLoaderHostedWindow_OnDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) && HostedControl is FileExportLoaderControl felc)
            {
                // Note that you can have more than one file.
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string ext = Path.GetExtension(files[0]).ToLower();
                if (!felc.CanLoadFileExtension(ext))
                {
                    e.Effects = DragDropEffects.None;
                    e.Handled = true;
                }
            }
        }
    }
}
