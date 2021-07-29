﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Gammtek.Conduit.MassEffect3.SFXGame.QuestMap;
using LegendaryExplorer.Misc;
using LegendaryExplorer.Tools.PlotEditor;
using LegendaryExplorerCore.Gammtek;
using LegendaryExplorerCore.Packages;
using LegendaryExplorer.Tools.PlotEditor.Dialogs;
using static LegendaryExplorer.Tools.TlkManagerNS.TLKManagerWPF;

namespace LegendaryExplorer.Tools.PlotEditor
{
	/// <summary>
	///   Interaction logic for QuestMapView.xaml
	/// </summary>
	public partial class QuestMapView : NotifyPropertyChangedControlBase
    {
		public QuestMapView()
		{
			InitializeComponent();
            SetFromQuestMap(new BioQuestMap());
        }

        private ObservableCollection<KeyValuePair<int, BioQuest>> _quests;
        private KeyValuePair<int, BioQuest> _selectedQuest;
        private BioQuestGoal _selectedQuestGoal;
        private BioQuestPlotItem _selectedQuestPlotItem;
        private BioQuestTask _selectedQuestTask;

        public bool CanAddQuestGoal
        {
            get
            {
                if (Quests == null || !Quests.Any())
                {
                    return false;
                }

                return SelectedQuest.Value != null;
            }
        }

        public bool CanAddQuestPlotItem
        {
            get
            {
                if (Quests == null || !Quests.Any())
                {
                    return false;
                }

                return SelectedQuest.Value != null;
            }
        }

        public bool CanAddQuestTask
        {
            get
            {
                if (Quests == null || !Quests.Any())
                {
                    return false;
                }

                return SelectedQuest.Value != null;
            }
        }

        public bool CanRemoveQuest
        {
            get
            {
                if (Quests == null || !Quests.Any())
                {
                    return false;
                }

                return SelectedQuest.Value != null;
            }
        }

        public bool CanRemoveQuestGoal
        {
            get
            {
                if (SelectedQuest.Value?.Goals == null || !SelectedQuest.Value.Goals.Any())
                {
                    return false;
                }

                return SelectedQuestGoal != null;
            }
        }

        public bool CanRemoveQuestPlotItem
        {
            get
            {
                if (SelectedQuest.Value?.PlotItems == null || !SelectedQuest.Value.PlotItems.Any())
                {
                    return false;
                }

                return SelectedQuestPlotItem != null;
            }
        }

        public bool CanRemoveQuestTask
        {
            get
            {
                if (SelectedQuest.Value?.Tasks == null || !SelectedQuest.Value.Tasks.Any())
                {
                    return false;
                }

                return SelectedQuestTask != null;
            }
        }

        public ObservableCollection<KeyValuePair<int, BioQuest>> Quests
        {
            get => _quests;
            set
            {
                SetProperty(ref _quests, value);
                OnPropertyChanged(nameof(CanAddQuestGoal));
                OnPropertyChanged(nameof(CanAddQuestPlotItem));
                OnPropertyChanged(nameof(CanAddQuestTask));
                OnPropertyChanged(nameof(CanRemoveQuest));
            }
        }

        public KeyValuePair<int, BioQuest> SelectedQuest
        {
            get => _selectedQuest;
            set
            {
                SetProperty(ref _selectedQuest, value);
                OnPropertyChanged(nameof(CanAddQuestGoal));
                OnPropertyChanged(nameof(CanAddQuestPlotItem));
                OnPropertyChanged(nameof(CanAddQuestTask));
                OnPropertyChanged(nameof(CanRemoveQuest));
                OnPropertyChanged(nameof(CanRemoveQuestGoal));
                OnPropertyChanged(nameof(CanRemoveQuestPlotItem));
                OnPropertyChanged(nameof(CanRemoveQuestTask));
            }
        }

        public BioQuestGoal SelectedQuestGoal
        {
            get => _selectedQuestGoal;
            set
            {
                SetProperty(ref _selectedQuestGoal, value);
                OnPropertyChanged(nameof(CanRemoveQuestGoal));
            }
        }

        public BioQuestPlotItem SelectedQuestPlotItem
        {
            get => _selectedQuestPlotItem;
            set
            {
                SetProperty(ref _selectedQuestPlotItem, value);
                OnPropertyChanged(nameof(CanRemoveQuestPlotItem));
            }
        }

        public BioQuestTask SelectedQuestTask
        {
            get => _selectedQuestTask;
            set
            {
                SetProperty(ref _selectedQuestTask, value);
                OnPropertyChanged(nameof(SelectedQuestTask));
                OnPropertyChanged(nameof(CanRemoveQuestTask));
            }
        }

        public void AddQuest()
        {
            if (Quests == null)
            {
                return;
            }

            var dlg = new NewObjectDialog
            {
                ContentText = "New codex section",
                ObjectId = GetMaxQuestId() + 1
            };

            if (dlg.ShowDialog() == false || dlg.ObjectId < 0)
            {
                return;
            }

            AddQuest(dlg.ObjectId);
        }

        public void AddQuest(int id, BioQuest quest = null)
        {
            if (Quests == null)
            {
                Quests = InitCollection<KeyValuePair<int, BioQuest>>();
            }

            if (Quests.Any(pair => pair.Key == id))
            {
                return;
            }

            if (quest == null)
            {
                quest = new BioQuest();
            }

            quest.Goals = InitCollection(quest.Goals);
            quest.PlotItems = InitCollection(quest.PlotItems);
            quest.Tasks = InitCollection(quest.Tasks);

            var questPair = new KeyValuePair<int, BioQuest>(id, quest);

            Quests.Add(questPair);

            SelectedQuest = questPair;
        }

        public void AddQuestGoal()
        {
            AddQuestGoal(null);
        }

        public void AddQuestGoal(BioQuestGoal questGoal)
        {
            if (Quests == null || SelectedQuest.Value == null)
            {
                return;
            }

            if (SelectedQuest.Value.Goals == null)
            {
                SelectedQuest.Value.Goals = InitCollection<BioQuestGoal>();
            }

            if (questGoal == null)
            {
                questGoal = new BioQuestGoal();
            }

            SelectedQuest.Value.Goals.Add(questGoal);

            SelectedQuestGoal = questGoal;
        }

        public void AddQuestPlotItem()
        {
            AddQuestPlotItem(null);
        }

        public void AddQuestPlotItem(BioQuestPlotItem questPlotItem)
        {
            if (Quests == null || SelectedQuest.Value == null)
            {
                return;
            }

            if (SelectedQuest.Value.PlotItems == null)
            {
                SelectedQuest.Value.PlotItems = InitCollection<BioQuestPlotItem>();
            }

            if (questPlotItem == null)
            {
                questPlotItem = new BioQuestPlotItem();
            }

            SelectedQuest.Value.PlotItems.Add(questPlotItem);

            SelectedQuestPlotItem = questPlotItem;
        }

        public void AddQuestTask()
        {
            AddQuestTask(null);
        }

        public void AddQuestTask(BioQuestTask questTask)
        {
            if (Quests == null || SelectedQuest.Value == null)
            {
                return;
            }

            if (SelectedQuest.Value.Tasks == null)
            {
                SelectedQuest.Value.Tasks = InitCollection<BioQuestTask>();
            }

            if (questTask == null)
            {
                questTask = new BioQuestTask();
            }

            questTask.PlotItemIndices = questTask.PlotItemIndices != null
                ? InitCollection(questTask.PlotItemIndices)
                : InitCollection<int>();

            SelectedQuest.Value.Tasks.Add(questTask);

            SelectedQuestTask = questTask;
        }

        public void AddQuestTaskPlotItemIndex()
        {
            if (Quests == null || SelectedQuest.Value == null || SelectedQuestTask == null)
            {
                return;
            }

            if (SelectedQuestTask.PlotItemIndices == null)
            {
                SelectedQuestTask.PlotItemIndices = InitCollection<int>();
            }

            var dlg = new NewObjectDialog
            {
                ContentText = "New quest task plot item index",
                ObjectId = SelectedQuestTask.PlotItemIndices.Any() ? SelectedQuestTask.PlotItemIndices.Max(i => i) + 1 : 0
            };

            if (dlg.ShowDialog() == false || dlg.ObjectId < 0)
            {
                return;
            }

            SelectedQuestTask.PlotItemIndices.Add(dlg.ObjectId);
        }

        public void CopyQuest()
        {
            if (Quests == null || SelectedQuest.Value == null)
            {
                return;
            }

            var dlg = new CopyObjectDialog
            {
                ContentText = $"Copy quest #{SelectedQuest.Key}",
                ObjectId = GetMaxQuestId() + 1
            };

            if (dlg.ShowDialog() == false || dlg.ObjectId < 0 || SelectedQuest.Key == dlg.ObjectId)
            {
                return;
            }

            AddQuest(dlg.ObjectId, new BioQuest(SelectedQuest.Value));
        }

        public void CopyQuestGoal()
        {
            if (Quests == null || SelectedQuest.Value?.Goals == null || SelectedQuestGoal == null)
            {
                return;
            }

            AddQuestGoal(new BioQuestGoal(SelectedQuestGoal));
        }

        public void CopyQuestPlotItem()
        {
            if (Quests == null || SelectedQuest.Value?.PlotItems == null || SelectedQuestPlotItem == null)
            {
                return;
            }

            AddQuestPlotItem(new BioQuestPlotItem(SelectedQuestPlotItem));
        }

        public void CopyQuestTask()
        {
            if (Quests == null || SelectedQuest.Value?.Tasks == null || SelectedQuestTask == null)
            {
                return;
            }

            AddQuestTask(new BioQuestTask(SelectedQuestTask));
        }

        public void GoToQuest(KeyValuePair<int, BioQuest> quest)
        {
            SelectedQuest = quest;
            QuestsListBox.ScrollIntoView(SelectedQuest);
            QuestsListBox.Focus();
        }

        public bool TryFindQuestMap(IMEPackage pcc, out ExportEntry export, out int dataOffset)
        {
            export = null;
            dataOffset = -1;

            try
            {
                export = pcc.Exports.First(exp => exp.ClassName == "BioQuestMap");
            }
            catch
            {
                return false;
            }

            dataOffset = export.propsEnd();

            return true;
        }

        public void Open(IMEPackage pcc)
        {
            if (!TryFindQuestMap(pcc, out ExportEntry export, out int dataOffset))
            {
                return;
            }

            using (var stream = new MemoryStream(export.Data))
            {
                stream.Seek(dataOffset, SeekOrigin.Begin);

                var questMap = BinaryBioQuestMap.Load(stream);

                SetFromQuestMap(questMap);
            }
        }

        
        public BioQuestMap ToQuestMap()
        {
            var questMap = new BioQuestMap
            {
                BoolTaskEvals = BoolStateTaskListsControl.StateTaskLists.ToDictionary(pair => pair.Key, pair => pair.Value),
                FloatTaskEvals = FloatStateTaskListsControl.StateTaskLists.ToDictionary(pair => pair.Key, pair => pair.Value),
                IntTaskEvals = IntStateTaskListsControl.StateTaskLists.ToDictionary(pair => pair.Key, pair => pair.Value),
                Quests = Quests.ToDictionary(pair => pair.Key, pair => pair.Value)
            };

            return questMap;
        }

        public void ChangeQuestId()
        {
            if (SelectedQuest.Value == null)
            {
                return;
            }

            var dlg = new ChangeObjectIdDialog
            {
                ContentText = $"Change id of codex page #{SelectedQuest.Key}",
                ObjectId = SelectedQuest.Key
            };

            if (dlg.ShowDialog() == false || dlg.ObjectId < 0 || dlg.ObjectId == SelectedQuest.Key)
            {
                return;
            }

            var quest = SelectedQuest.Value;

            Quests.Remove(SelectedQuest);

            AddQuest(dlg.ObjectId, quest);
        }

        public void RemoveQuest()
        {
            if (Quests == null || SelectedQuest.Value == null)
            {
                return;
            }

            var index = Quests.IndexOf(SelectedQuest);

            if (!Quests.Remove(SelectedQuest))
            {
                return;
            }

            if (Quests.Any())
            {
                SelectedQuest = index - 1 >= 0
                    ? Quests[index - 1]
                    : Quests.First();
            }
        }

        public void RemoveQuestGoal()
        {
            if (Quests == null || SelectedQuest.Value?.PlotItems == null || SelectedQuestGoal == null)
            {
                return;
            }

            var index = SelectedQuest.Value.Goals.IndexOf(SelectedQuestGoal);

            if (!SelectedQuest.Value.Goals.Remove(SelectedQuestGoal))
            {
                return;
            }

            if (SelectedQuest.Value.Goals.Any())
            {
                SelectedQuestGoal = index - 1 >= 0
                    ? SelectedQuest.Value.Goals[index - 1]
                    : SelectedQuest.Value.Goals.First();
            }
        }

        public void RemoveQuestPlotItem()
        {
            if (Quests == null || SelectedQuest.Value?.PlotItems == null || SelectedQuestPlotItem == null)
            {
                return;
            }

            var index = SelectedQuest.Value.PlotItems.IndexOf(SelectedQuestPlotItem);

            if (!SelectedQuest.Value.PlotItems.Remove(SelectedQuestPlotItem))
            {
                return;
            }

            if (SelectedQuest.Value.PlotItems.Any())
            {
                SelectedQuestPlotItem = index - 1 >= 0
                    ? SelectedQuest.Value.PlotItems[index - 1]
                    : SelectedQuest.Value.PlotItems.First();
            }
        }

        public void RemoveQuestTask()
        {
            if (Quests == null || SelectedQuest.Value?.Tasks == null || SelectedQuestTask == null)
            {
                return;
            }

            var index = SelectedQuest.Value.Tasks.IndexOf(SelectedQuestTask);

            if (!SelectedQuest.Value.Tasks.Remove(SelectedQuestTask))
            {
                return;
            }

            if (SelectedQuest.Value.Goals.Any())
            {
                SelectedQuestTask = index - 1 >= 0
                    ? SelectedQuest.Value.Tasks[index - 1]
                    : SelectedQuest.Value.Tasks.First();
            }
        }

        public void RemoveQuestTaskPlotItemIndex(int index)
        {
            if (Quests == null || SelectedQuest.Value == null || SelectedQuestTask == null)
            {
                return;
            }

            if (SelectedQuestTask.PlotItemIndices == null)
            {
                return;
            }

            if (index < 0 || index > SelectedQuestTask.PlotItemIndices.Count)
            {
                return;
            }

            SelectedQuestTask.PlotItemIndices.RemoveAt(index);
        }

        protected void SetFromQuestMap(BioQuestMap questMap)
        {
            if (questMap == null)
            {
                return;
            }

            BoolStateTaskListsControl.SetStateTaskLists(questMap.BoolTaskEvals.OrderBy(pair => pair.Key));
            FloatStateTaskListsControl.SetStateTaskLists(questMap.FloatTaskEvals.OrderBy(pair => pair.Key));
            IntStateTaskListsControl.SetStateTaskLists(questMap.IntTaskEvals.OrderBy(pair => pair.Key));
            Quests = InitCollection(questMap.Quests.OrderBy(quest => quest.Key));

            foreach (var quest in Quests)
            {
                quest.Value.Goals = InitCollection(quest.Value.Goals);
                quest.Value.PlotItems = InitCollection(quest.Value.PlotItems);
                quest.Value.Tasks = InitCollection(quest.Value.Tasks);

                foreach (var questTask in quest.Value.Tasks)
                {
                    questTask.PlotItemIndices = InitCollection(questTask.PlotItemIndices);
                }
            }
        }

        
        private static ObservableCollection<T> InitCollection<T>()
        {
            return new ObservableCollection<T>();
        }

        
        private static ObservableCollection<T> InitCollection<T>(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(collection));
            }

            return new ObservableCollection<T>(collection);
        }

        private int GetMaxQuestId()
        {
            return Quests.Any() ? Quests.Max(page => page.Key) : -1;
        }

        private void ChangeQuestId_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ChangeQuestId();
        }

        private void CopyQuest_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CopyQuest();
        }

        private void RemoveQuest_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveQuest();
        }

        private void AddQuestButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddQuest();
        }

        private void CopyQuestGoal_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CopyQuestGoal();
        }

        private void RemoveQuestGoal_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveQuestGoal();
        }

        private void AddQuestGoal_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddQuestGoal();
        }

        private void CopyQuestPlotItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CopyQuestPlotItem();
        }

        private void RemoveQuestPlotItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveQuestPlotItem();
        }

        private void AddQuestPlotItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddQuestPlotItem();
        }

        private void CopyQuestTask_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CopyQuestTask();
        }

        private void RemoveQuestTask_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveQuestTask();
        }

        private void AddQuestTask_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddQuestTask();
        }

        private void AddQuestTaskPlotItemIndex_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            AddQuestTaskPlotItemIndex();
        }

        private void RemoveQuestTaskPlotItemIndex_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveQuestTaskPlotItemIndex(QuestTaskPlotItemIndicesListBox.SelectedIndex);
        }

        private void txt_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (CodexMapView.package != null)
            {
                if (SelectedQuestGoal != null)
                {
                    txt_goalName.Text = GlobalFindStrRefbyID(SelectedQuestGoal.Name, CodexMapView.package);
                    txt_goalDesc.Text = GlobalFindStrRefbyID(SelectedQuestGoal.Description, CodexMapView.package);
                }
                else txt_goalName.Text = txt_goalDesc.Text = "";

                if (SelectedQuestTask != null)
                {
                    txt_taskName.Text = GlobalFindStrRefbyID(SelectedQuestTask.Name, CodexMapView.package);
                    txt_taskDesc.Text = GlobalFindStrRefbyID(SelectedQuestTask.Description, CodexMapView.package);
                }
                else txt_taskName.Text = txt_taskDesc.Text = "";

                if (SelectedQuestPlotItem != null)
                {
                    txt_PlotitmDesc.Text = GlobalFindStrRefbyID(SelectedQuestPlotItem.Name, CodexMapView.package);
                }
                else txt_PlotitmDesc.Text = "";

            }
        }
    }
}
