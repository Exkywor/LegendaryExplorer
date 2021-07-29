﻿//using System;
//using System.IO;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows.Forms;
//using ME3Explorer.Scene3D;
//using ME3ExplorerCore.Helpers;
//using ME3ExplorerCore.Packages;
//using ME3ExplorerCore.Unreal.BinaryConverters;
//using StaticMesh = ME3ExplorerCore.Unreal.BinaryConverters.StaticMesh;

////until it gets switched over
//using SkeletalMesh = ME3ExplorerCore.Unreal.Classes.SkeletalMesh;

//namespace ME3Explorer.Meshplorer
//{
//    public partial class Meshplorer : WinFormsBase
//    {
//        public List<int> Objects = new List<int>();
//        public List<int> Materials = new List<int>();
//        public List<int> ChosenMaterials = new List<int>(); // materials included in the skeletal mesh.
//        public int MeshplorerMode = 0; //0=PCC,1=PSK
//        public StaticMesh stm;
//        public SkeletalMesh skm;
//        public float PreviewRotation = 0;
//        public List<string> RFiles;
//        public static readonly string MeshplorerDataFolder = Path.Combine(App.AppDataFolder, @"Meshplorer\");
//        private readonly string RECENTFILES_FILE = "RECENTFILES";
//        private string pendingFileToLoad = null;

//        public Meshplorer()
//        {
//            MemoryAnalyzer.AddTrackedMemoryItem(new MemoryAnalyzerObjectExtended("Meshplorer", new WeakReference(this));
//            InitializeComponent();
//            LoadRecentList();
//            RefreshRecent(false);
//        }

//        public Meshplorer(string filepath)
//        {
//            pendingFileToLoad = filepath;
//            InitializeComponent();
//            LoadRecentList();
//            RefreshRecent(false);
//        }

//        private void Meshplorer_Load(object sender, EventArgs e)
//        {
//            view.LoadDirect3D();
//            rotatingToolStripMenuItem.Checked = Properties.Settings.Default.MeshplorerViewRotating;
//            wireframeToolStripMenuItem.Checked = Properties.Settings.Default.MeshplorerViewWireframeEnabled;
//            solidToolStripMenuItem.Checked = Properties.Settings.Default.MeshplorerViewSolidEnabled;
//            firstPersonToolStripMenuItem.Checked = Properties.Settings.Default.MeshplorerViewFirstPerson;
//            firstPersonToolStripMenuItem_Click(null, null); // Force first/third person setting to take effect

//            if (pendingFileToLoad != null)
//            {
//                LoadFile(pendingFileToLoad);
//                pendingFileToLoad = null;
//            }
//        }

//        private void timer1_Tick(object sender, EventArgs e)
//        {
//            view.Context.UpdateScene(0.1f); // TODO: Measure actual elapsed time
//            view.Invalidate();
//        }

//        private void loadPCCToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            OpenFileDialog d = new OpenFileDialog();
//            d.Filter = "*.pcc;*.sfm;*.upk|*.pcc;*.sfm;*.upk";
//            if (d.ShowDialog() == DialogResult.OK)
//                LoadFile(d.FileName);
//        }

//        public void LoadFile(string path)
//        {
//            try
//            {
//                //LoadME3Package(path);
//                LoadMEPackage(path);
//                MeshplorerMode = 0;
//                RefreshMaterialList();
//                RefreshMeshList();
//                lblStatus.Text = Path.GetFileName(path);

//                AddRecent(path, false);
//                SaveRecentList();
//                RefreshRecent(true, RFiles);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error:\n" + ex.Message);
//            }
//        }

//        public void RefreshMeshList()
//        {
//            view.Context.TextureCache.Dispose(); // Clear out the loaded textures from the previous pcc
//            listBox1.Items.Clear();
//            Objects.Clear();
//            foreach (var export in Pcc.Exports)
//            {
//                if (export.ClassName == "StaticMesh")
//                {
//                    listBox1.Items.Add($"StM#{export.UIndex} : {export.ObjectName.Instanced}");
//                    Objects.Add(export.UIndex);
//                }
//                else if (export.ClassName == "SkeletalMesh")
//                {
//                    listBox1.Items.Add($"SkM#{export.UIndex} : {export.ObjectName.Instanced}");
//                    Objects.Add(export.UIndex);
//                }
//            }
//        }

//        public void RefreshMaterialList()
//        {
//            Materials.Clear();
//            MaterialBox.Items.Clear();
//            var listItems = Pcc.Exports.Where(x => x.ClassName == "Material" || x.ClassName == "MaterialInstanceConstant");
//            foreach (var item in listItems)
//            {
//                Materials.Add(item.UIndex);
//                MaterialBox.Items.Add($"#{item.UIndex} : {item.ObjectName.Instanced}");
//            }
//            //for (int i = 0; i < Exports.Count(); i++)
//            //{
//            //    exportEntry = Exports[i];
//            //    if (exportEntry.ClassName == "Material" || exportEntry.ClassName == "MaterialInstanceConstant")
//            //    {

//            //    }
//            //}
//        }

//        public void RefreshChosenMaterialsList()
//        {
//            ChosenMaterials.Clear();
//            MaterialIndexBox.Items.Clear();
//            if (skm != null)
//            {
//                for (int i = 0; i < skm.Materials.Count; i++)
//                {
//                    ChosenMaterials.Add(skm.Materials[i]);
//                    string desc = "";
//                    if (skm.Materials[i] > 0)
//                    { // Material is export
//                        ExportEntry export = Pcc.GetUExport(skm.Materials[i]);
//                        desc = " Export #" + skm.Materials[i] + " : " + export.ObjectName.Instanced;
//                    }
//                    else if (skm.Materials[i] < 0)
//                    { // Material is import???
//                        desc = "Import #" + -skm.Materials[i];
//                    }
//                    MaterialIndexBox.Items.Add(i + " - " + desc);
//                }
//            }
//        }

//        public void LoadStaticMesh(int index)
//        {
//            try
//            {
//                stm = ObjectBinary.From<StaticMesh>(Pcc.GetUExport(index));
//                //var mesh = ObjectBinary.From <StaticMesh>(Pcc.getUExport(index));
//                // Load meshes for the LODs
//                preview?.Dispose();
//                preview = new ModelPreview(view.Context.Device, stm, 0, view.Context.TextureCache);
//                RefreshChosenMaterialsList();
//                CenterView();

//                // Update treeview
//                treeView1.BeginUpdate();
//                treeView1.Nodes.Clear();
//                //treeView1.Nodes.Add(stm.ToTree());
//                //treeView1.Nodes[0].Expand();
//                treeView1.EndUpdate();
//                MaterialBox.Visible = false;
//                MaterialIndexBox.Visible = false;
//            }
//            catch (Exception e)
//            {
//                MessageBox.Show(e.FlattenException());
//            }
//        }

//        public void LoadSkeletalMesh(int uindex)
//        {
//            DisableLODs();
//            UnCheckLODs();
//            try
//            {
//                skm = new SkeletalMesh(Pcc.GetUExport(uindex));

//                // Load preview model
//                preview?.Dispose();
//                preview = new ModelPreview(view.Context.Device, skm, view.Context.TextureCache);
//                RefreshChosenMaterialsList();
//                CenterView();

//                // Update treeview
//                treeView1.BeginUpdate();
//                treeView1.Nodes.Clear();
//                treeView1.Nodes.Add(skm.ToTree());
//                treeView1.Nodes[0].Expand();
//                treeView1.EndUpdate();
//                lODToolStripMenuItem.Visible = true;
//                lOD0ToolStripMenuItem.Enabled = true;
//                lOD0ToolStripMenuItem.Checked = true;
//                if (skm.LODModels.Count > 1)
//                    lOD1ToolStripMenuItem.Enabled = true;
//                if (skm.LODModels.Count > 2)
//                    lOD2ToolStripMenuItem.Enabled = true;
//                if (skm.LODModels.Count > 3)
//                    lOD3ToolStripMenuItem.Enabled = true;
//                MaterialBox.Visible = false;
//                MaterialIndexBox.Visible = false;
//            }
//            catch (Exception e)
//            {
//                MessageBox.Show(e.FlattenException());
//            }
//        }

//        public float dir;

//        private void Meshplorer_KeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.KeyCode == Keys.Add)
//                dir = 1;
//            if (e.KeyCode == Keys.Subtract)
//                dir = -1;
//            if (e.KeyCode == Keys.F)
//            {
//                CenterView();
//            }
//        }

//        private void Meshplorer_KeyUp(object sender, KeyEventArgs e)
//        {
//            dir = 0;
//        }

//        private void Meshplorer_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            if (!e.Cancel)
//            {
//                preview?.Dispose();
//                preview = null;
//                view.DestroyDirect3D();
//                Properties.Settings.Default.MeshplorerViewRotating = rotatingToolStripMenuItem.Checked;
//                Properties.Settings.Default.MeshplorerViewWireframeEnabled = wireframeToolStripMenuItem.Checked;
//                Properties.Settings.Default.MeshplorerViewSolidEnabled = solidToolStripMenuItem.Checked;
//                Properties.Settings.Default.MeshplorerViewFirstPerson = firstPersonToolStripMenuItem.Checked;
//                Properties.Settings.Default.Save();
//            }
//        }

//        public int getLOD()
//        {
//            int res = 0;
//            if (lOD1ToolStripMenuItem.Checked) res = 1;
//            if (lOD2ToolStripMenuItem.Checked) res = 2;
//            if (lOD3ToolStripMenuItem.Checked) res = 3;
//            return res;
//        }

//        private void lOD0ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CurrentLOD = 0;
//            UnCheckLODs();
//            lOD0ToolStripMenuItem.Checked = true;
//        }

//        private void lOD1ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CurrentLOD = 1;
//            UnCheckLODs();
//            lOD1ToolStripMenuItem.Checked = true;
//        }

//        private void lOD2ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CurrentLOD = 2;
//            UnCheckLODs();
//            lOD2ToolStripMenuItem.Checked = true;
//        }

//        private void lOD3ToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            CurrentLOD = 3;
//            UnCheckLODs();
//            lOD3ToolStripMenuItem.Checked = true;
//        }

//        public void UnCheckLODs()
//        {
//            lOD0ToolStripMenuItem.Checked = false;
//            lOD1ToolStripMenuItem.Checked = false;
//            lOD2ToolStripMenuItem.Checked = false;
//            lOD3ToolStripMenuItem.Checked = false;
//        }

//        public void DisableLODs()
//        {
//            lOD0ToolStripMenuItem.Enabled = false;
//            lOD1ToolStripMenuItem.Enabled = false;
//            lOD2ToolStripMenuItem.Enabled = false;
//            lOD3ToolStripMenuItem.Enabled = false;
//        }

//        private void exportTreeToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            SaveFileDialog d = new SaveFileDialog();
//            d.Filter = "Textfiles(*.txt)|*.txt";
//            if (d.ShowDialog() == DialogResult.OK)
//            {
//                FileStream fs = new FileStream(d.FileName, FileMode.Create, FileAccess.Write);
//                PrintNodes(treeView1.Nodes, fs, 0);
//                fs.Close();
//                MessageBox.Show("Done.", "Meshplorer", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
//            }
//        }

//        public void PrintNodes(TreeNodeCollection t, FileStream fs, int depth)
//        {
//            string tab = "";
//            for (int i = 0; i < depth; i++)
//                tab += ' ';
//            foreach (TreeNode t1 in t)
//            {
//                string s = tab + t1.Text;
//                WriteString(fs, s);
//                fs.WriteByte(0xD);
//                fs.WriteByte(0xA);
//                if (t1.Nodes.Count != 0)
//                    PrintNodes(t1.Nodes, fs, depth + 4);
//            }
//        }

//        public void WriteString(FileStream fs, string s)
//        {
//            for (int i = 0; i < s.Length; i++)
//                fs.WriteByte((byte)s[i]);
//        }

//        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
//        {
//            int n = listBox1.SelectedIndex;
//            if (n == -1)
//            {
//                return;
//            }
//            n = Objects[n];
//            lODToolStripMenuItem.Visible = false;
//            UnCheckLODs();
//            stm = null;
//            skm = null;
//            preview?.Dispose();
//            preview = null;
//            MaterialBox.Visible = false;
//            MaterialIndexBox.Visible = false;
//            if (Pcc.GetUExport(n).ClassName == "StaticMesh")
//                LoadStaticMesh(n);
//            if (Pcc.GetUExport(n).ClassName == "SkeletalMesh")
//                LoadSkeletalMesh(n);
//        }

//        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
//        {
//            TreeNode t = e.Node;
//            MaterialBox.Visible = false;
//            MaterialIndexBox.Visible = false;
//            if (skm != null)
//            {
//                if (t.Parent != null && t.Parent.Text == "Materials")
//                {
//                    MaterialBox.Visible = true;
//                    try
//                    {
//                        string s = t.Text.Split(' ')[0].Trim('#');
//                        int idx = Convert.ToInt32(s);
//                        for (int i = 0; i < Materials.Count; i++)
//                            if (Materials[i] == idx)
//                                MaterialBox.SelectedIndex = i;
//                    }
//                    catch
//                    {

//                    }
//                }
//                if (t.Parent != null && t.Parent.Text == "Sections")
//                {
//                    MaterialIndexBox.Visible = true;
//                    try
//                    {
//                        int m = skm.LODModels[t.Parent.Parent.Index].Sections[t.Index].MaterialIndex;
//                        MaterialIndexBox.SelectedIndex = m;
//                    }
//                    catch
//                    {

//                    }
//                }
//            }
//            else if (stm != null)
//            {
//                if (t.Parent != null && t.Parent.Text == "Sections")
//                {
//                    MaterialBox.Visible = true;
//                    // HACK: assume that all static meshes have only 1 LOD. This has been true in my experience.
//                    int section = t.Index;
//                    //DISABLED TEMP

//                    //int mat = stm.Mesh.Mat.Lods[0].Sections[section].Name - 1;
//                    //for (int i = 0; i < Materials.Count; i++)
//                    //    if (Materials[i] == mat)
//                    //        MaterialBox.SelectedIndex = i;
//                }
//            }
//        }

//        public override void handleUpdate(List<PackageUpdate> updates)
//        {
//            IEnumerable<PackageUpdate> relevantUpdates = updates.Where(x => x.Change.Has(PackageChange.Export));
//            List<int> updatedExports = relevantUpdates.Select(x => x.Index).ToList();
//            if (skm != null && updatedExports.Contains(skm.Export.UIndex)) 
//            {
//                //loaded SkeletalMesh is no longer a SkeletalMesh
//                if (skm.Export.ClassName != "SkeletalMesh")
//                {
//                    skm = null;
//                    preview?.Dispose();
//                    preview = null;
//                    treeView1.Nodes.Clear();
//                    RefreshMeshList();
//                }
//                else
//                {
//                    LoadSkeletalMesh(skm.Export.UIndex); //this will be refactored someday
//                }
//                updatedExports.Remove(skm.Export.UIndex);
//            }
//            else if (stm != null && updatedExports.Contains(stm.Export.UIndex))
//            {
//                int index = stm.Export.UIndex;
//                //loaded SkeletalMesh is no longer a SkeletalMesh
//                if (Pcc.GetUExport(index).ClassName != "StaticMesh")
//                {
//                    stm = null;
//                    preview?.Dispose();
//                    preview = null;
//                    treeView1.Nodes.Clear();
//                    RefreshMeshList();
//                }
//                else
//                {
//                    LoadStaticMesh(index);
//                }
//                updatedExports.Remove(index);
//            }
//            if (updatedExports.Intersect(Materials).Any())
//            {
//                RefreshMaterialList();
//            }
//            else
//            {
//                foreach (var i in updatedExports)
//                {
//                    string className = Pcc.GetUExport(i).ClassName;
//                    if (className == "MaterialInstanceConstant" || className == "Material")
//                    {
//                        RefreshMaterialList();
//                        break;
//                    }
//                }
//            }
//            if (updatedExports.Intersect(Objects).Any())
//            {
//                RefreshMeshList();
//            }
//            else
//            {
//                foreach (var i in updatedExports)
//                {
//                    string className = Pcc.GetUExport(i).ClassName;
//                    if (className == "SkeletalMesh" || className == "StaticMesh")
//                    {
//                        RefreshMeshList();
//                        break;
//                    }
//                }
//            }
//        }

//        private void savePCCToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (Pcc == null)
//                return;
//            Pcc.Save();
//            MessageBox.Show("Done.", "Meshplorer", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
//        }

//        #region 3D Viewport
//        //private List<WorldMesh> CurrentMeshLODs = new List<WorldMesh>();
//        private int CurrentLOD = 0;
//        //private List<SharpDX.Direct3D11.Texture2D> CurrentTextures = new List<SharpDX.Direct3D11.Texture2D>();
//        //private List<SharpDX.Direct3D11.ShaderResourceView> CurrentTextureViews = new List<SharpDX.Direct3D11.ShaderResourceView>();
//        private ModelPreview preview = null;
//        private float globalscale = 1.0f;

//        private void CenterView()
//        {
//            if (preview != null && preview.LODs.Count > 0)
//            {
//                WorldMesh m = preview.LODs[CurrentLOD].Mesh;
//                globalscale = 0.5f / m.AABBHalfSize.Length();
//                view.Context.Camera.Position = m.AABBCenter * globalscale;
//                view.Context.Camera.FocusDepth = 1.0f;
//                if (view.Context.Camera.FirstPerson)
//                {
//                    view.Context.Camera.Position -= view.Context.Camera.CameraForward * view.Context.Camera.FocusDepth;
//                }
//            }
//            else
//            {
//                view.Context.Camera.Position = SharpDX.Vector3.Zero;
//                view.Context.Camera.Pitch = -(float)Math.PI / 5.0f;
//                view.Context.Camera.Yaw = (float)Math.PI / 4.0f;
//                globalscale = 1.0f;
//            }
//        }

//        private void view_Render(object sender, EventArgs e)
//        {
//            if (preview != null && preview.LODs.Count > 0) // For some reason, reading props calls DoEvents which means that this might be called *in the middle of* loading a preview
//            {
//                if (solidToolStripMenuItem.Checked && CurrentLOD < preview.LODs.Count)
//                {
//                    view.Context.Wireframe = false;
//                    preview.Render(view.Context, CurrentLOD, SharpDX.Matrix.Scaling(globalscale) * SharpDX.Matrix.RotationY(PreviewRotation));
//                }
//                if (wireframeToolStripMenuItem.Checked)
//                {
//                    view.Context.Wireframe = true;
//                    SceneRenderContext.WorldConstants ViewConstants = new SceneRenderContext.WorldConstants(SharpDX.Matrix.Transpose(view.Context.Camera.ProjectionMatrix), SharpDX.Matrix.Transpose(view.Context.Camera.ViewMatrix), SharpDX.Matrix.Transpose(SharpDX.Matrix.Scaling(globalscale) * SharpDX.Matrix.RotationY(PreviewRotation)));
//                    view.Context.DefaultEffect.PrepDraw(view.Context.ImmediateContext);
//                    view.Context.DefaultEffect.RenderObject(view.Context.ImmediateContext, ViewConstants, preview.LODs[CurrentLOD].Mesh, new SharpDX.Direct3D11.ShaderResourceView[] { null });
//                }
//            }
//        }

//        private void view_Update(object sender, float e)
//        {
//            if (rotatingToolStripMenuItem.Checked) PreviewRotation += e * 0.05f;
//            //view.Context.Camera.Pitch = (float)Math.Sin(view.Context.Time);
//        }

//        private void firstPersonToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            bool old = view.Context.Camera.FirstPerson;
//            view.Context.Camera.FirstPerson = firstPersonToolStripMenuItem.Checked;
//            // Adjust view position so the camera doesn't teleport
//            if (!old && view.Context.Camera.FirstPerson)
//            {
//                view.Context.Camera.Position += -view.Context.Camera.CameraForward * view.Context.Camera.FocusDepth;
//            }
//            else if (old && !view.Context.Camera.FirstPerson)
//            {
//                view.Context.Camera.Position += view.Context.Camera.CameraForward * view.Context.Camera.FocusDepth;
//            }
//        }
//        #endregion

//        private void meshplorer_DragDrop(object sender, DragEventArgs e)
//        {
//            List<string> DroppedFiles = ((string[])e.Data.GetData(DataFormats.FileDrop)).ToList();
//            if (DroppedFiles.Count > 0)
//            {
//                LoadFile(DroppedFiles[0]);
//            }
//        }

//        private void meshplorer_DragEnter(object sender, DragEventArgs e)
//        {
//            if (e.Data.GetDataPresent(DataFormats.FileDrop))
//                e.Effect = DragDropEffects.All;
//            else
//                e.Effect = DragDropEffects.None;
//        }

//        public void AddRecent(string s, bool loadingList)
//        {
//            RFiles = RFiles.Where(x => !x.Equals(s, StringComparison.InvariantCultureIgnoreCase)).ToList();
//            if (loadingList)
//            {
//                RFiles.Add(s); //in order
//            }
//            else
//            {
//                RFiles.Insert(0, s); //put at front
//            }
//            if (RFiles.Count > 10)
//            {
//                RFiles.RemoveRange(10, RFiles.Count - 10);
//            }
//            recentToolStripMenuItem.Enabled = true;
//        }

//        private void SaveRecentList()
//        {
//            if (!Directory.Exists(MeshplorerDataFolder))
//            {
//                Directory.CreateDirectory(MeshplorerDataFolder);
//            }
//            string path = MeshplorerDataFolder + RECENTFILES_FILE;
//            if (File.Exists(path))
//                File.Delete(path);
//            File.WriteAllLines(path, RFiles);
//        }

//        private void RefreshRecent(bool propogate, List<string> recents = null)
//        {
//            if (propogate && recents != null)
//            {
//                //we are posting an update to other instances of packed
//                var forms = Application.OpenForms;
//                foreach (Form form in forms)
//                {
//                    if (form is Meshplorer && this != form)
//                    {
//                        ((Meshplorer)form).RefreshRecent(false, RFiles);
//                    }
//                }
//            }
//            else if (recents != null)
//            {
//                //we are receiving an update
//                RFiles = new List<string>(recents);
//            }
//            recentToolStripMenuItem.DropDownItems.Clear();
//            if (RFiles.Count <= 0)
//            {
//                recentToolStripMenuItem.Enabled = false;
//                return;
//            }
//            recentToolStripMenuItem.Enabled = true;

//            foreach (string filepath in RFiles)
//            {
//                ToolStripMenuItem fr = new ToolStripMenuItem(filepath, null, RecentFile_click);
//                recentToolStripMenuItem.DropDownItems.Add(fr);
//            }
//        }

//        private void RecentFile_click(object sender, EventArgs e)
//        {
//            string s = sender.ToString();
//            if (File.Exists(s))
//            {
//                LoadFile(s);
//            }
//            else
//            {
//                MessageBox.Show("File does not exist: " + s);
//            }
//        }

//        private void LoadRecentList()
//        {
//            RFiles = new List<string>();
//            RFiles.Clear();
//            string path = MeshplorerDataFolder + RECENTFILES_FILE;
//            recentToolStripMenuItem.Enabled = false;
//            if (File.Exists(path))
//            {
//                string[] recents = File.ReadAllLines(path);
//                foreach (string recent in recents)
//                {
//                    if (File.Exists(recent))
//                    {
//                        recentToolStripMenuItem.Enabled = true;
//                        AddRecent(recent, true);
//                    }
//                }
//            }
//        }

//        private void exportToOBJToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            SaveFileDialog dialog = new SaveFileDialog();
//            dialog.Filter = "Wavefront OBJ File (*.obj)|*.obj";

//            if (dialog.ShowDialog() == DialogResult.OK)
//            {
//                if (stm != null)
//                {
//                    //DISABLED TEMP
//                    //stm.ExportOBJ(dialog.FileName);
//                }
//                else if (skm != null)
//                {
//                    skm.ExportOBJ(dialog.FileName);
//                }
//            }
//        }

//        private void importFromOBJToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            if (stm == null)
//            {
//                MessageBox.Show("Only static meshes can be imported from OBJ files.");
//                return;
//            }

//            OpenFileDialog dialog = new OpenFileDialog();
//            dialog.Filter = "Wavefront OBJ File (*.obj)|*.obj";

//            if (dialog.ShowDialog() == DialogResult.OK)
//            {
//                timer1.Enabled = false;
//                //DISABLED TEMP
//                //stm.ImportOBJ(dialog.FileName);
//                //stm.Export.Data = stm.SerializeToBuffer();
//                MessageBox.Show("OBJ import complete.");
//                timer1.Enabled = true;
//            }
//        }
//    }
//}
