﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using Microsoft.Win32;
using FontAwesome5.WPF;
using ME3Explorer.Unreal;
using ME3ExplorerCore.GameFilesystem;
using ME3ExplorerCore.Helpers;
using ME3ExplorerCore.Packages;
using Microsoft.AppCenter.Analytics;
using System.Diagnostics;

namespace ME3Explorer
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool CICOpen = true;
        private bool SearchOpen;
        private bool AdvancedOpen;
        private bool UtilitiesOpen;
        private bool CreateModsOpen;
        private bool TaskPaneOpen;
        private bool ToolInfoPanelOpen;
        private bool PathsPanelOpen;
        private bool TaskPaneInfoPanelOpen;

        readonly Brush HighlightBrush = Application.Current.FindResource("HighlightColor") as Brush;
        readonly Brush LabelTextBrush = Application.Current.FindResource("LabelTextBrush") as Brush;
        public static double dpiScaleX = 1;
        public static double dpiScaleY = 1;

        private static FieldInfo _menuDropAlignmentField;

        /// <summary>
        /// Displayed version in the UI. About page will be more detailed.
        /// </summary>
        public string DisplayedVersion
        {
            get
            {
                Version assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
                string version = $"{assemblyVersion.Major}.{assemblyVersion.Minor}";
                if (assemblyVersion.Build != 0)
                {
                    version += "." + assemblyVersion.Build;
                }

#if DEBUG
                version += " DEBUG";
#elif NIGHTLY
                //This is what will be placed in release. Comment this out when building for a stable!
                version += " NIGHTLY"; //ENSURE THIS IS CHANGED FOR MAJOR RELEASES AND RELEASE CANDIDATES
#elif RELEASE
                // UPDATE THIS FOR RELEASE
                //version += " RC";
#endif
                return version;
            }
        }

        public MainWindow()
        {
            //exception occurs in InitializeComponent() without try block, but doesn't if present. wtf
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                SystemCommands.CloseWindow(this);
            }
            var buildDate = SharedUI.BuildInfo.GetBuildDateTime(Assembly.GetExecutingAssembly().Location);
            if (DateTime.Compare(buildDate, default) != 0)
            {
                VersionBar_BuildDate_Label.Content = buildDate.Date;
            }
            else
            {
                VersionBar_BuildDate_Label.Content = "Unknown build date";
            }

            //make menu's appear right side, which is busted on WPF with touchscreens
            _menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            System.Diagnostics.Debug.Assert(_menuDropAlignmentField != null);

            EnsureStandardPopupAlignment();
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;



            installModspanel.setToolList(Tools.Items.Where(x => x.tags.Contains("user")));
            Tools.FavoritesChanged += Tools_FavoritesChanged;
            Tools_FavoritesChanged(null, null);
            utilitiesPanel.setToolList(Tools.Items.Where(x => x.tags.Contains("utility")));
            createModsPanel.setToolList(Tools.Items.Where(x => x.tags.Contains("developer")));

            Topmost = Properties.Settings.Default.AlwaysOnTop;
            //Check that at least one game path is set. If none are, show the initial dialog.
            if (ME1Directory.DefaultGamePath == null && ME2Directory.DefaultGamePath == null && ME3Directory.DefaultGamePath == null)
            {
                (new InitialSetup()).ShowDialog();
            }
            UpdateGamePathWarningIconStatus();
            StartLoadingTLKs();
            //File.WriteAllText(Path.Combine(App.ExecFolder, "LoadedFiles.json"), JsonConvert.SerializeObject(ME3LoadedFiles.GetFilesLoadedInGame(), Formatting.Indented));
        }

        private void StartLoadingTLKs()
        {
            Task.Run(() =>
            {
                //Fetch core count from WMI - this can take like 1-2 seconds and is not typically necessary until a tool is opened
                try
                {
                    App.CoreCount = 2;
                    foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
                    {
                        App.CoreCount = int.Parse(item["NumberOfCores"].ToString());
                    }
                }
                catch
                {
                    //???
                }

                // load TLK strings
                // todo: this loads them all. Just do one call i guess.

                try
                {
                    TLKLoader.LoadSavedTlkList();
                    TlkManagerNS.TLKManagerWPF.ME1LastReloaded = $"{DateTime.Now:HH:mm:ss tt}";
                    TlkManagerNS.TLKManagerWPF.ME2LastReloaded = $"{DateTime.Now:HH:mm:ss tt}";
                    TlkManagerNS.TLKManagerWPF.ME3LastReloaded = $"{DateTime.Now:HH:mm:ss tt}";
                }
                catch
                {
                    //?
                }

                App.TlkFirstLoadDone = true;
            }).ContinueWithOnUIThread(prevTask =>
            {
                //StartingUpPanel.Visibility = Visibility.Invisible;
                DoubleAnimation fadeout = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    EasingFunction = new ExponentialEase(),
                    Duration = new Duration(TimeSpan.FromSeconds(1))
                };
                fadeout.Completed += delegate
                {
                    LoadingPanel.Visibility = Visibility.Collapsed;
                    ME3TweaksLogoButton.Visibility = Visibility.Visible;
                    LoadingPanel.Children.Remove(LoadingSpinnerImage); //This will remove the storyboard
                    DoubleAnimation fadein = new DoubleAnimation
                    {
                        From = 0,
                        To = 1,
                        EasingFunction = new ExponentialEase(),
                        Duration = new Duration(TimeSpan.FromSeconds(1))
                    };
                    ME3TweaksLogoButton.BeginAnimation(OpacityProperty, fadein);
                };
                //da.RepeatBehavior=new RepeatBehavior(3);
                LoadingPanel.BeginAnimation(OpacityProperty, fadeout);
            });
        }

        private static void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            EnsureStandardPopupAlignment();
        }

        private static void EnsureStandardPopupAlignment()
        {
            if (SystemParameters.MenuDropAlignment && _menuDropAlignmentField != null)
            {
                _menuDropAlignmentField.SetValue(null, false);
            }
        }

        private void Tools_FavoritesChanged(object sender, EventArgs e)
        {
            IEnumerable<Tool> favs = Tools.Items.Where(x => x.IsFavorited);
            favoritesPanel.setToolList(favs);
            favoritesWatermark.Visibility = favs.Any() ? Visibility.Hidden : Visibility.Visible;
        }

        private void Command_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void MaximizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void MinimizeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (SearchBox.IsFocused)
                {
                    SearchBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
                this.DragMove();
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            (new About()).Show();
        }

        private void Logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CICOpen)
            {
                closeCIC();

            }
            else
            {
                Analytics.TrackEvent("CIC Window Size Changed", new Dictionary<string, string>()
                {
                    {"New State", "Maximized"}
                });
                if (CreateModsOpen)
                {
                    closeCreateMods(100);
                }
                if (UtilitiesOpen)
                {
                    closeUtilities(100);
                }
                if (TaskPaneOpen)
                {
                    //closeTaskPane(100);
                }
                CICOpen = true;
                if (SearchBox.Text.Trim() != string.Empty)
                {
                    SearchOpen = true;
                    searchPanel.BeginDoubleAnimation(WidthProperty, 300, 200);
                }
                CICPanel.BeginDoubleAnimation(WidthProperty, 650, 300);
            }
        }

        private void closeCIC(int duration = 300, bool triggerAnalytics = true)
        {
            CICOpen = false;
            if (SearchOpen)
            {
                closeSearch(duration / 3);
            }
            if (AdvancedOpen)
            {
                closeAdvancedSettings(duration / 3);
            }
            if (ToolInfoPanelOpen)
            {
                closeToolInfo();
            }
            if (PathsPanelOpen)
            {
                closeGamePaths();
            }

            if (triggerAnalytics)
            {
                Analytics.TrackEvent("CIC Window Size Changed", new Dictionary<string, string>()
                {
                    {"New State", "Minimized"}
                });
            }

            CICPanel.BeginDoubleAnimation(WidthProperty, 0, duration);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!SearchOpen)
            {
                if (AdvancedOpen)
                {
                    closeAdvancedSettings();
                }
                if (ToolInfoPanelOpen)
                {
                    closeToolInfo();
                }
                if (PathsPanelOpen)
                {
                    closeGamePaths();
                }
                SearchOpen = true;
                searchPanel.BeginDoubleAnimation(WidthProperty, 300, 200);
            }

            var results = new List<Tool>();
            string[] words = SearchBox.Text.ToLower().Split(' ');
            foreach (Tool tool in Tools.Items)
            {
                if (tool.open != null)
                {
                    //for each word we've typed in
                    if (words.Any(word => tool.tags.FuzzyMatch(word) || tool.name.ToLower().Contains(word) || tool.name.ToLower().Split(' ').FuzzyMatch(word)))
                    {
                        results.Add(tool);
                    }
                }
            }

            searchPanel.setToolList(results);
        }

        private void SearchBox_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (SearchOpen && SearchBox.Text.Trim() == string.Empty)
            {
                SearchOpen = false;
                searchPanel.BeginDoubleAnimation(WidthProperty, 0, 200);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var tools = new List<WPFBase>();
            foreach (var package in MEPackageHandler.packagesInTools)
            {
                tools.AddRange(package.Users.OfType<WPFBase>());
            }
            foreach (var tool in tools)
            {
                try
                {
                    tool.Close();
                }
                catch (Exception)
                {
                    //This exception can occur if you are doing "Close All Windows" while a file has unsaved changes
                    //All windows will be signaled to close, which will throw dialog twice
                    //exception occuring here will cause this window to close but not the one with the dialog.
                }
            }
            if (MEPackageHandler.packagesInTools.Count > 0)
            {
                e.Cancel = true;
            }
            Properties.Settings.Default.AlwaysOnTop = alwaysOnTopCheckBox.IsChecked ?? false;
        }

        private void advancedSettings_Click(object sender, RoutedEventArgs e)
        {
            if (AdvancedOpen)
            {
                closeAdvancedSettings(200);
            }
            else
            {
                if (SearchOpen)
                {
                    closeSearch();
                }
                if (ToolInfoPanelOpen)
                {
                    closeToolInfo();
                }
                if (PathsPanelOpen)
                {
                    closeGamePaths();
                }
                AdvancedOpen = true;
                advancedPanel.BeginDoubleAnimation(WidthProperty, 300, 200);
            }
        }

        private void closeSearch(int duration = 100)
        {
            SearchOpen = false;
            searchPanel.BeginDoubleAnimation(WidthProperty, 0, duration);
        }

        private void SearchBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!SearchOpen && SearchBox.Text.Trim() != string.Empty)
            {
                SearchOpen = true;
                searchPanel.BeginDoubleAnimation(WidthProperty, 300, 200);
                if (AdvancedOpen)
                {
                    closeAdvancedSettings();
                }
                if (ToolInfoPanelOpen)
                {
                    closeToolInfo();
                }
                if (PathsPanelOpen)
                {
                    closeGamePaths();
                }
            }
        }

        private void closeAdvancedSettings(int duration = 100)
        {
            AdvancedOpen = false;
            advancedPanel.BeginDoubleAnimation(WidthProperty, 0, duration);
        }

        private void UtilitiesButton_Click(object sender, RoutedEventArgs e)
        {
            if (UtilitiesOpen)
            {
                closeUtilities();
            }
            else
            {
                Analytics.TrackEvent("Main Window Tab Changed", new Dictionary<string, string>()
                {
                    {"Tab Name Opened", "Utilities"}
                });
                UtilitiesOpen = true;
                if (CICOpen)
                {
                    closeCIC(100, false);
                }
                if (CreateModsOpen)
                {
                    closeCreateMods(100);
                }
                if (TaskPaneOpen)
                {
                    //closeTaskPane(100);
                }
                utilitiesButton.OpacityMask = HighlightBrush;
                utilitiesPanel.BeginDoubleAnimation(WidthProperty, 650, 300);
            }
        }

        private void closeUtilities(int duration = 300)
        {
            if (ToolInfoPanelOpen)
            {
                closeToolInfo();
            }
            Analytics.TrackEvent("Main Window Tab Changed", new Dictionary<string, string>()
            {
                {"Tab Name Closed", "Utilities"}
            });
            UtilitiesOpen = false;
            utilitiesPanel.BeginDoubleAnimation(WidthProperty, 0, duration);
            utilitiesButton.OpacityMask = LabelTextBrush;
        }

        private void CreateModsButton_Click(object sender, RoutedEventArgs e)
        {
            if (CreateModsOpen)
            {
                closeCreateMods();
            }
            else
            {
                Analytics.TrackEvent("Main Window Tab Changed", new Dictionary<string, string>()
                {
                    {"Tab Name Opened", "Create Mods"}
                });
                CreateModsOpen = true;
                if (CICOpen)
                {
                    closeCIC(100, false);
                }
                if (UtilitiesOpen)
                {
                    closeUtilities(100);
                }
                if (TaskPaneOpen)
                {
                    //closeTaskPane(100);
                }
                createModsButton.OpacityMask = HighlightBrush;
                createModsPanel.BeginDoubleAnimation(WidthProperty, 650, 300);
            }
        }

        private void closeCreateMods(int duration = 300)
        {
            if (ToolInfoPanelOpen)
            {
                closeToolInfo();
            }
            Analytics.TrackEvent("Main Window Tab Changed", new Dictionary<string, string>()
            {
                {"Tab Name Closed", "Create Mods"}
            });
            CreateModsOpen = false;
            createModsPanel.BeginDoubleAnimation(WidthProperty, 0, duration);
            createModsButton.OpacityMask = LabelTextBrush;
        }

        private void closeToolInfo(int duration = 50)
        {
            ToolInfoPanelOpen = false;
            toolInfoPanel.BeginDoubleAnimation(WidthProperty, 0, duration);
        }

        private void ToolMouseOver(object sender, Tool t)
        {
            if (SearchBox.IsFocused)
            {
                SearchBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            openToolInfo(t);
        }

        private void openToolInfo(Tool e)
        {
            toolInfoPanel.setTool(e);
            if (!ToolInfoPanelOpen)
            {
                if (SearchOpen)
                {
                    closeSearch();
                }
                if (AdvancedOpen)
                {
                    closeAdvancedSettings();
                }
                if (PathsPanelOpen)
                {
                    closeGamePaths();
                }
                ToolInfoPanelOpen = true;
                toolInfoPanel.BeginDoubleAnimation(WidthProperty, 300, 50);
            }
        }

        private void closeGamePaths(int duration = 50)
        {
            PathsPanelOpen = false;
            pathsPanel.BeginDoubleAnimation(WidthProperty, 0, duration);

            string me1Path = null;
            string me2Path = null;
            string me3Path = null;
            if (File.Exists(Path.Combine(me1PathBox.Text, "Binaries", "MassEffect.exe")))
            {
                me1Path = me1PathBox.Text;
            }
            if (File.Exists(Path.Combine(me2PathBox.Text, "Binaries", "MassEffect2.exe")))
            {
                me2Path = me2PathBox.Text;
            }
            if (File.Exists(Path.Combine(me3PathBox.Text, "Binaries", "Win32", "MassEffect3.exe")))
            {
                me3Path = me3PathBox.Text;
            }
            var directories = new List<string> { me1Path, me2Path, me3Path };
            MEDirectories.SaveSettings(directories);

            gamePathsWarningIcon.Visibility = directories.All(item => item == null || !Directory.Exists(item)) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void pathBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            List<string> allowedVersions = new List<string>();
            string game;
            if (b == me1PathBrowseButton)
            {
                game = "Mass Effect";
                allowedVersions.Add("1.2.20608.0");
            }
            else if (b == me2PathBrowseButton)
            {
                game = "Mass Effect 2";
                allowedVersions.Add("1.2.1604.0"); // Steam
                allowedVersions.Add("01604.00"); // Origin
            }
            else if (b == me3PathBrowseButton)
            {
                game = "Mass Effect 3";
                allowedVersions.Add("05427.124");
            }
            else
            {
                return;
            }

            if (game != "")
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = $"Select {game} executable"
                };
                game = game.Replace(" ", "");
                ofd.Filter = $"{game}.exe|{game}.exe";

                if (ofd.ShowDialog() == true)
                {
                    var fvi = FileVersionInfo.GetVersionInfo(ofd.FileName);
                    if (!allowedVersions.Contains(fvi.FileVersion))
                    {
                        MessageBox.Show($"Cannot use this executable: The only supported file versions are:\n{string.Join("\n", allowedVersions)}\n\nThe selected one has version: {fvi.FileVersion}\n\nNote: ME3Explorer does not support Mass Effect Legendary Edition games.",
                            "Invalid game", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string result = Path.GetDirectoryName(Path.GetDirectoryName(ofd.FileName));

                    switch (game)
                    {
                        case "MassEffect":
                            me1PathBox.Text = ME1Directory.DefaultGamePath = result;
                            me1PathBox.Visibility = Visibility.Visible;
                            break;
                        case "MassEffect2":
                            me2PathBox.Text = ME2Directory.DefaultGamePath = result;
                            me2PathBox.Visibility = Visibility.Visible;
                            break;
                        case "MassEffect3":
                            me3PathBox.Text = ME3Directory.DefaultGamePath = Path.GetDirectoryName(result);
                            me3PathBox.Visibility = Visibility.Visible;
                            break;
                    }
                    UpdateGamePathWarningIconStatus();
                }
            }
        }

        private void UpdateGamePathWarningIconStatus()
        {
            var warningIcons = new List<ImageAwesome> { me1GamePathWarningIcon, me2GamePathWarningIcon, me3GamePathWarningIcon };
            var directories = new List<string> { ME1Directory.DefaultGamePath, ME2Directory.DefaultGamePath, ME3Directory.DefaultGamePath };
            gamePathsWarningIcon.Visibility = directories.Any(item => item != null && (!Directory.Exists(item) || !Directory.Exists(Path.Combine(item, "BIOGame")) || !Directory.Exists(Path.Combine(item, "Binaries")))) ? Visibility.Visible : Visibility.Collapsed;
            for (int i = 0; i < warningIcons.Count; i++)
            {
                ImageAwesome icon = warningIcons[i];
                string directory = directories[i];
                icon.Visibility = directory != null && (!Directory.Exists(directory) || !Directory.Exists(Path.Combine(directory, "BIOGame")) || !Directory.Exists(Path.Combine(directory, "Binaries"))) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void gamePaths_Click(object sender, RoutedEventArgs e)
        {
            if (PathsPanelOpen)
            {
                closeGamePaths();
            }
            else
            {
                if (ME1Directory.DefaultGamePath != null)
                {
                    me1PathBox.Text = ME1Directory.DefaultGamePath;
                }
                else
                {
                    me1PathBox.Visibility = Visibility.Collapsed;
                }
                if (ME2Directory.DefaultGamePath != null)
                {
                    me2PathBox.Text = ME2Directory.DefaultGamePath;
                }
                else
                {
                    me2PathBox.Visibility = Visibility.Collapsed;
                }
                if (ME3Directory.DefaultGamePath != null)
                {
                    me3PathBox.Text = ME3Directory.DefaultGamePath;
                }
                else
                {
                    me3PathBox.Visibility = Visibility.Collapsed;
                }
                if (SearchOpen)
                {
                    closeSearch();
                }
                if (AdvancedOpen)
                {
                    closeAdvancedSettings();
                }
                if (ToolInfoPanelOpen)
                {
                    closeToolInfo();
                }
                PathsPanelOpen = true;
                pathsPanel.BeginDoubleAnimation(WidthProperty, 300, 50);
            }
        }

        private void taskPaneButton_Click(object sender, RoutedEventArgs e)
        {
            //if (TaskPaneOpen)
            //{
            //    closeTaskPane();
            //}
            //else
            //{
            //    Analytics.TrackEvent("Main Window Tab Changed", new Dictionary<string, string>()
            //    {
            //        {"Tab Name Opened", "TaskPane"}
            //    });
            //    TaskPaneOpen = true;
            //    if (CICOpen)
            //    {
            //        closeCIC(100, false);
            //    }
            //    if (UtilitiesOpen)
            //    {
            //        closeUtilities(100);
            //    }
            //    if (CreateModsOpen)
            //    {
            //        closeCreateMods(100);
            //    }
            //    taskPaneButton.OpacityMask = HighlightBrush;
            //    taskPanePanel.BeginDoubleAnimation(WidthProperty, 650, 300);
            //}
        }

        //private void closeTaskPane(int duration = 300)
        //{
        //    if (TaskPaneInfoPanelOpen)
        //    {
        //        Analytics.TrackEvent("Main Window Tab Changed", new Dictionary<string, string>()
        //        {
        //            {"Tab Name Closed", "TaskPane"}
        //        });
        //        closeTaskPaneInfoPanel();
        //    }
        //    TaskPaneOpen = false;
        //    taskPanePanel.BeginDoubleAnimation(WidthProperty, 0, duration);
        //    taskPaneButton.OpacityMask = LabelTextBrush;
        //}

        //private void closeTaskPaneInfoPanel(int duration = 100)
        //{
        //    TaskPaneInfoPanelOpen = false;
        //    taskPaneInfoPanel.BeginDoubleAnimation(WidthProperty, 0, duration);
        //}

        //private void taskPanePanel_ToolMouseOver(object sender, WPFBaseViewModel e)
        //{
        //    taskPaneInfoPanel.setTool(e.wpf);
        //    if (!TaskPaneInfoPanelOpen)
        //    {
        //        TaskPaneInfoPanelOpen = true;
        //        taskPaneInfoPanel.BeginDoubleAnimation(WidthProperty, 300, 100);
        //    }
        //}

        //private void taskPaneInfoPanel_Close(object sender, EventArgs e)
        //{
        //    closeTaskPaneInfoPanel();
        //}

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PresentationSource source = PresentationSource.FromVisual(this);

            if (source != null)
            {
                dpiScaleX = source.CompositionTarget.TransformToDevice.M11;
                dpiScaleY = source.CompositionTarget.TransformToDevice.M22;
            }
        }

        private void ME3TweaksDiscord_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(App.DiscordInviteURL);
            }
            catch (Exception)
            {
                try
                {
                    Clipboard.SetText(App.DiscordInviteURL);
                }
                catch
                {
                    //RIP
                }
            }
        }
    }
}
