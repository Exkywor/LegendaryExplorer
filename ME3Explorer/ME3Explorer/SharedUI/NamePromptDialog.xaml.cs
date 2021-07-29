﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ME3ExplorerCore.Packages;
using ME3ExplorerCore.Unreal;
using static ME3Explorer.PackageEditorWPF;

namespace ME3Explorer.SharedUI
{
    /// <summary>
    /// Interaction logic for NamePromptDialogPromptDialog.xaml
    /// </summary>
    public partial class NamePromptDialog : TrackingNotifyPropertyChangedWindowBase
    {
        private List<IndexedName> _nameList;
        public List<IndexedName> NameList
        {
            get => _nameList;
            set
            {
                _nameList = value;
                OnPropertyChanged();
            }
        }

        private int _number;

        public int Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        public NamePromptDialog(string question, string title, List<IndexedName> NameList, int defaultValue = 0) : base("Name Prompt Dialog", false)
        {
            this.NameList = NameList;
            DataContext = this;
            InitializeComponent();
            txtQuestion.Text = question;
            Title = title;
            answerChoicesCombobox.SelectedIndex = defaultValue;
        }

        public static IndexedName Prompt(Window owner, string question, string title, List<IndexedName> NameList, int defaultValue = 0)
        {
            NamePromptDialog inst = new NamePromptDialog(question, title, NameList, defaultValue);
            inst.Owner = owner;
            inst.numberColumn.Width = new GridLength(0);
            inst.ShowDialog();
            if (inst.DialogResult == true)
            {
                return (IndexedName)inst.answerChoicesCombobox.SelectedItem;
            }

            return null;
        }

        public static bool Prompt(Window owner, string question, string title, IMEPackage pcc, out NameReference result, int defaultValue = 0)
        {
            NamePromptDialog inst = new NamePromptDialog(question, title, pcc.Names.Select((nr, i) => new IndexedName(i, nr)).ToList(), defaultValue);
            inst.Owner = owner;
            inst.ShowDialog();
            if (inst.DialogResult == true)
            {
                IndexedName name = (IndexedName)inst.answerChoicesCombobox.SelectedItem;
                result = new NameReference(name.Name.Name, inst.Number);
                return true;
            }

            result = default;
            return false;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
