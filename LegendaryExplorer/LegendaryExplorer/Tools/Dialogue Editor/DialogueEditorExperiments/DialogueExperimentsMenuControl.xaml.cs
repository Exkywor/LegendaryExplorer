using LegendaryExplorer.DialogueEditor;
using LegendaryExplorer.DialogueEditor.DialogueEditorExperiments;
using System.Windows;
using System.Windows.Controls;

namespace LegendaryExplorer.Tools.Dialogue_Editor.DialogueEditorExperiments
{
    /// <summary>
    /// Class that holds toolset development experiments. Actual experiment code should be in the Experiments classes
    /// </summary>
    public partial class DialogueExperimentsMenuControl : MenuItem
    {
        public DialogueExperimentsMenuControl()
        {
            LoadCommands();
            InitializeComponent();
        }

        private void LoadCommands() { }

        public DialogueEditorWindow GetDEWindow()
        {
            if (Window.GetWindow(this) is DialogueEditorWindow dew)
            {
                return dew;
            }

            return null;
        }

        // EXPERIMENTS: EXKYWOR------------------------------------------------------------
        #region Exkywor's experiments
        private void UpdateNativeNodeStringRef_Click(object sender, RoutedEventArgs e)
        {
            DialogueEditorExperimentsE.UpdateNativeNodeStringRef(GetDEWindow());
        }

        private void CloneNodeAndSequence_Click(object sender, RoutedEventArgs e)
        {
            DialogueEditorExperimentsE.CloneNodeAndSequence(GetDEWindow());
        }

        private void LinkNodesFree_Click(object sender, RoutedEventArgs e)
        {
            DialogueEditorExperimentsE.LinkNodesFree(GetDEWindow());
        }

        private void LinkNodesStrRef_Click(object sender, RoutedEventArgs e)
        {
            DialogueEditorExperimentsE.LinkNodesStrRef(GetDEWindow());
        }

        private void CreateNodesSequence_Click(object sender, RoutedEventArgs e)
        {
            DialogueEditorExperimentsE.CreateNodesSequenceExperiment(GetDEWindow());
        }

        private void CreateSelectedNodeSequence_Click(object sender, RoutedEventArgs e)
        {
            DialogueEditorExperimentsE.CreateSelectedNodeSequence(GetDEWindow());
        }

        #endregion

    }
}
