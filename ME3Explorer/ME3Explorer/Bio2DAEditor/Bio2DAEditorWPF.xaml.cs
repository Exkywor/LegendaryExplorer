﻿using ME3Explorer.SharedUI;
using ME3Explorer.TlkManagerNS;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ME3ExplorerCore.Misc;
using ME3ExplorerCore.Packages;
using static ME3Explorer.PackageEditorWPF;

namespace ME3Explorer
{
    /// <summary>
    /// Interaction logic for Bio2DAEditorWPF.xaml
    /// </summary>
    public partial class Bio2DAEditorWPF : ExportLoaderControl
    {
        public ObservableCollectionExtended<IndexedName> ParentNameList { get; private set; }

        private Bio2DA _table2da;
        public Bio2DA Table2DA
        {
            get => _table2da;
            private set => SetProperty(ref _table2da, value);
        }

        public ICommand CommitCommand { get; set; }

        public Bio2DAEditorWPF() : base("Bio2DA Editor")
        {
            DataContext = this;
            LoadCommands();
            InitializeComponent();
        }

        public override bool CanParse(ExportEntry exportEntry)
        {
            return !exportEntry.IsDefaultObject && exportEntry.ObjectName != "Default2DA" && (exportEntry.ClassName == "Bio2DA" || exportEntry.ClassName == "Bio2DANumberedRows");
        }

        public override void LoadExport(ExportEntry exportEntry)
        {
            CurrentLoadedExport = exportEntry;
            Table2DA = new Bio2DA(CurrentLoadedExport);
        }

        public override void UnloadExport()
        {
            Table2DA = null;
            CurrentLoadedExport = null;
        }

        public override void PopOut()
        {
            if (CurrentLoadedExport != null)
            {
                ExportLoaderHostedWindow elhw = new ExportLoaderHostedWindow(new Bio2DAEditorWPF(), CurrentLoadedExport)
                {
                    Title = $"Bio2DA Editor - {CurrentLoadedExport.UIndex} {CurrentLoadedExport.InstancedFullPath} - {CurrentLoadedExport.FileRef.FilePath}"
                };
                elhw.Show();
            }
        }
        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            for (int counter = 0; counter < Bio2DA_DataGrid.SelectedCells.Count; counter++)
            {
                int columnIndex = Bio2DA_DataGrid.SelectedCells[0].Column.DisplayIndex;
                int rowIndex = Bio2DA_DataGrid.Items.IndexOf(Bio2DA_DataGrid.SelectedCells[0].Item);
                var item = Table2DA[rowIndex, columnIndex];
                Bio2DAInfo_CellCoordinates_TextBlock.Text = $"Selected cell coordinates: {rowIndex + 1},{columnIndex + 1}";
                if (item != null)
                {
                    Bio2DAInfo_CellDataType_TextBlock.Text = $"Selected cell data type: {item.Type}";
                    Bio2DAInfo_CellData_TextBlock.Text = $"Selected cell data: {item.DisplayableValue}";
                    Bio2DAInfo_CellDataOffset_TextBlock.Text = $"Selected cell data offset: 0x{item.Offset:X6}";
                    if (item.Type == Bio2DACell.Bio2DADataType.TYPE_INT)
                    {
                        Bio2DAInfo_CellDataAsStrRef_TextBlock.Text = TLKManagerWPF.GlobalFindStrRefbyID(item.IntValue, CurrentLoadedExport.FileRef.Game, CurrentLoadedExport.FileRef);
                    }
                    else
                    {
                        Bio2DAInfo_CellDataAsStrRef_TextBlock.Text = "Select cell of TYPE_INT to see as TLK Str";
                    }
                }
                else
                {
                    Bio2DAInfo_CellDataType_TextBlock.Text = "Selected cell data type: NULL";
                    Bio2DAInfo_CellData_TextBlock.Text = "Selected cell data:";
                    Bio2DAInfo_CellDataOffset_TextBlock.Text = "Selected cell data offset: N/A";
                    Bio2DAInfo_CellDataAsStrRef_TextBlock.Text = "Select a cell to preview TLK value";
                }
            }
        }

        /// <summary>
        /// Removes the access key where you can do _ for quick key press to go to a column. will make headers and stuff look proper
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            string header = e.Column.Header.ToString();

            // Replace all underscores with two underscores, to prevent AccessKey handling
            e.Column.Header = header.Replace("_", "__");
        }

        private void LoadCommands()
        {
            CommitCommand = new GenericCommand(Commit2DA, CanCommit2DA);
        }

        private void Commit2DA()
        {
            Table2DA.Write2DAToExport();
            Table2DA.MarkAsUnmodified();
        }

        private bool CanCommit2DA() => Table2DA?.IsModified ?? false;

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog
            {
                Filter = "Excel spreadsheet|*.xlsx",
                FileName = CurrentLoadedExport.ObjectName
            };
            if (d.ShowDialog() == true)
            {
                Table2DA.Write2DAToExcel(d.FileName);
                MessageBox.Show("Done");
            }
        }

        internal void SetParentNameList(ObservableCollectionExtended<IndexedName> namesList)
        {
            ParentNameList = namesList;
        }

        public override void Dispose()
        {
            //Nothing to dispose in this control
        }

        private void ImportToExcel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Excel sheet must be formatted so: \r\nFIRST ROW must have the same column headings as current sheet. \r\nFIRST COLUMN has row numbers. \r\nIf using a multisheet excel file, the sheet tab must be named 'Import'.", "IMPORTANT INFORMATION:");
            OpenFileDialog oDlg = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "Import Excel table"
            };

            if (oDlg.ShowDialog() == true)
            {
                if (MessageBox.Show("This will overwrite the existing 2DA table.", "WARNING", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Bio2DA resulting2DA = Bio2DA.ReadExcelTo2DA(CurrentLoadedExport, oDlg.FileName);
                    if (resulting2DA != null)
                    {
                        if (resulting2DA.IsIndexed != Table2DA.IsIndexed)
                        {
                            MessageBox.Show(resulting2DA.IsIndexed
                                                ? "Warning: Imported sheet contains blank cells. Underlying sheet does not."
                                                : "Warning: Underlying sheet contains blank cells. Imported sheet does not.");
                        }
                        resulting2DA.Write2DAToExport();
                    }
                }
            }
        }
    }
}
