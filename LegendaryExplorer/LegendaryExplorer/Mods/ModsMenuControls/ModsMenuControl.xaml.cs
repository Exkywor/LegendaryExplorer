using LegendaryExplorer.Misc.ExperimentsTools;
using LegendaryExplorer.Tools.PackageEditor;
using LegendaryExplorer.Tools.Sequence_Editor;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Unreal.BinaryConverters;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private void LoadCommands()
        {
        }

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

        private void FemShepvBroshep_CleanFiles_Click(object sender, RoutedEventArgs e)
        {
            List<string> paths = new();
            if (Window.GetWindow(this) is PackageEditorWindow pew)
            {
                paths = SharedMethods.CopyCleanFiles(FemShepvBroShep.Files,
                    "G:\\My Drive\\Modding\\Mass Effect\\mods\\Counter Clone\\delivery\\FemShep v BroShep Duel of the Shepards LE\\DLC_MOD_FSvBSLE\\CookedPCConsole\\",
                    MEGame.LE3, true);
                SharedMethods.CopyCleanFiles(FemShepvBroShep.Files_Clean,
                    "G:\\My Drive\\Modding\\Mass Effect\\mods\\Counter Clone\\delivery\\FemShep v BroShep Duel of the Shepards LE\\DLC_MOD_FSvBSLE\\CookedPCConsole\\Clean\\",
                    MEGame.LE3, true);
            }

            MessageBox.Show(string.Join("\n ", paths));
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
        private void FemShepvBroshep_V_CleanFiles_Click(object sender, RoutedEventArgs e)
        {
            List<string> paths = new();
            if (Window.GetWindow(this) is PackageEditorWindow pew)
            {
                paths = SharedMethods.CopyCleanFiles(FemShepvBroShep_V.Files,
                    "G:\\My Drive\\Modding\\Mass Effect\\mods\\Counter Clone\\delivery\\FemShep v BroShep Duel of the Shepards LE - Vanilla VS\\DLC_MOD_FSvBSLE_V\\CookedPCConsole\\",
                    MEGame.LE3, true);
                SharedMethods.CopyCleanFiles(FemShepvBroShep_V.Files_Clean,
                    "G:\\My Drive\\Modding\\Mass Effect\\mods\\Counter Clone\\delivery\\FemShep v BroShep Duel of the Shepards LE - Vanilla VS\\DLC_MOD_FSvBSLE_V\\CookedPCConsole\\Clean\\",
                    MEGame.LE3, true);
            }

            MessageBox.Show(string.Join("\n ", paths));
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
