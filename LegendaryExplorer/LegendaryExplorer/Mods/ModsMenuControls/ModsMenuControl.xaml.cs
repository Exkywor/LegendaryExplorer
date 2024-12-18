using LegendaryExplorer.Mods;
using LegendaryExplorer.Tools.PackageEditor;
using LegendaryExplorer.Tools.Sequence_Editor;
using System.Windows;
using System.Windows.Controls;

namespace LegendaryExplorer.Mods.ModsMenuControls
{
    /// <summary>
    /// Class that holds toolset development experiments. Actual experiment code should be in the Experiments classes
    /// </summary>
    public partial class ModsMenuControl : MenuItem
    {
        public ModsMenuControl()
        {
            LoadCommands();
            InitializeComponent();
        }

        private void LoadCommands() { }

        public PackageEditorWindow GetPEWindow()
        {
            if (Window.GetWindow(this) is PackageEditorWindow pew)
            {
                return pew;
            }

            return null;
        }

        // EXPERIMENTS: EXKYWOR------------------------------------------------------------
        #region Exkywor's experiments
        private void EmilyReturns_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is PackageEditorWindow pew)
            {
                EmilyReturns.Patch(pew.Pcc);
            }
        }
        private void FemShepvBroshep_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is PackageEditorWindow pew)
            {
                FemShepvBroShep.Patch(pew.Pcc);
            }

            MessageBox.Show($"File successfully patched.");
        }
        private void BatchFemShepvBroshep_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is PackageEditorWindow)
            {
                FemShepvBroShep.BatchPatch();
            }

            MessageBox.Show($"Files successfully patched.");
        }

        private void FemShepvBroshep_V_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is PackageEditorWindow pew)
            {
                FemShepvBroShep_V.Patch(pew.Pcc);
            }

            MessageBox.Show($"File successfully patched.");
        }
        private void BatchFemShepvBroshep_V_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is PackageEditorWindow)
            {
                FemShepvBroShep_V.BatchPatch();
            }

            MessageBox.Show($"Files successfully patched.");
        }
        #endregion

        // EXPERIMENTS: OTHERS------------------------------------------------------------
        #region Other's experiments
        private void LE1Doors_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is SequenceEditorWPF sew)
            {
                Audemus.LE1Doors(sew);
            }
        }
        #endregion
    }
}
