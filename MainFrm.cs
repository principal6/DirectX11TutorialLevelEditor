using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
//using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace DirectX11TutorialLevelEditor
{
    using ObjectSetManager = DirectX11TutorialObjectEditor.ObjectSetManager;
    using ObjectSetElementData = DirectX11TutorialObjectEditor.ObjectSetElementData;

    public struct SSize
    {
        public int Width;
        public int Height;

        public SSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public static bool operator ==(SSize a, SSize b)
        {
            if ((a.Width == b.Width) && (a.Height == b.Height))
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(SSize a, SSize b)
        {
            if (a == b)
            {
                return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public struct SPosition
    {
        public int X;
        public int Y;

        public SPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator ==(SPosition a, SPosition b)
        {
            if ((a.X == b.X) && (a.Y == b.Y))
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(SPosition a, SPosition b)
        {
            if (a == b)
            {
                return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public struct STileModeInfo
    {
        public SSize TileSize;
        public SSize TileSheetSizeInTileCount;
        public string TileSheetFileName;

        public SPosition SelectionOrigin;
        public SPosition TileSelectionOffset;
        public SSize SelectionSizeInTileCount;
    }

    public enum ETileMode
    {
        Design,
        Movement,
        Object
    }

    public partial class MainFrm : Form
    {
        static private readonly string KMainFormTitle = "DirectX11Tutorial Level Editor";
        static private readonly string KDefaultLevelName = "level_new";
        static private readonly SSize KDefaultTileSize = new SSize(64, 64);
        static private readonly SSize KDefaultLevelSize = new SSize(5, 3);

        private readonly string KAssetDir;
        private readonly MGSurfaceTile SurfaceTile;
        private readonly MGSurfaceLevel SurfaceLevel;
        private readonly MGSurfaceObjectSet SurfaceObjectSet;

        private ETileMode m_TileMode = ETileMode.Design;
        private string m_LevelName = KDefaultLevelName;

        private SPosition m_MouseDownPos;

        private readonly ObjectSetManager m_ObjectSet = new ObjectSetManager();

        private STileModeInfo m_DesignTileInfo = new STileModeInfo
        {
            TileSheetFileName = "grass_64x64.png",
            TileSize = KDefaultTileSize
        };
        private STileModeInfo m_MovementTileInfo = new STileModeInfo
        {
            TileSheetFileName = "movement_platformer_64x64.png",
            TileSize = KDefaultTileSize
        };

        public MainFrm()
        {
            InitializeComponent();

            KAssetDir = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\asset\\";
            
            SurfaceTile = new MGSurfaceTile()
            {
                Parent = SplitViews.Panel1,
                Dock = DockStyle.Fill,
                FixedMovementTileSize = KDefaultTileSize
            };
            
            SurfaceLevel = new MGSurfaceLevel()
            {
                Parent = SplitViews.Panel2,
                Dock = DockStyle.Fill,
                FixedMovementTileSize = KDefaultTileSize
            };

            SurfaceObjectSet = new MGSurfaceObjectSet()
            {
                Parent = SplitObjectSet.Panel1,
                Dock = DockStyle.Fill,
                BackgroundColor = new Color(0, 0.6f, 1.0f)
            };

            SurfaceTile.SetAssetDir(KAssetDir);
            SurfaceLevel.SetAssetDir(KAssetDir);
            SurfaceObjectSet.SetAssetDir(KAssetDir);

            SurfaceTile.MouseMove += SurfaceTile_MouseMove;
            SurfaceTile.MouseDown += SurfaceTile_MouseDown;

            SurfaceLevel.MouseMove += SurfaceLevel_MouseMove;
            SurfaceLevel.MouseDown += SurfaceLevel_MouseDown;
            SurfaceLevel.MouseWheel += SurfaceLevel_MouseWheel;

            this.KeyPreview = true;
        }

        ~MainFrm()
        {
            SurfaceLevel.Dispose();
            SurfaceTile.Dispose();
            SurfaceObjectSet.Dispose();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            SurfaceTile.SetTileSheetTextures(ref m_DesignTileInfo, ref m_MovementTileInfo);

            SurfaceLevel.CreateLevel(KDefaultLevelSize.Width, KDefaultLevelSize.Height, m_DesignTileInfo, m_MovementTileInfo);

            UpdateMainFrmTitle();

            UpdateViews();

            SplitObjectView.Visible = false;
        }

        private void MainFrm_Resize(object sender, EventArgs e)
        {
            UpdateViews();
        }

        private void MainFrm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D1)
            {
                if (e.Control == true)
                {
                    TabTileView.SelectedIndex = 0;
                }
            }
            else if (e.KeyCode == Keys.D2)
            {
                if (e.Control == true)
                {
                    TabTileView.SelectedIndex = 1;
                }
            }

            if (e.KeyCode == Keys.Z)
            {
                if ((e.Control == true) && (e.Shift == true))
                {
                    SurfaceLevel.RedoHistory(ref TabTileView);
                }
                else if (e.Control == true)
                {
                    SurfaceLevel.UndoHistory(ref TabTileView);
                }
            }
        }

        private void SurfaceTile_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_MouseDownPos.X = e.X;
                m_MouseDownPos.Y = e.Y;

                SurfaceTile.UpdateSelection(m_MouseDownPos, SurfaceTile.GetTileSize());

                SurfaceLevel.UpdateSelectedTile(SurfaceTile.GetCurrentSelectionOrigin(), SurfaceTile.GetCurrentSelectionSizeInTileCount());
            }
        }

        private void SurfaceTile_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SSize tile_size = SurfaceTile.GetTileSize();

                SSize selection_rest;
                selection_rest.Width = m_MouseDownPos.X % tile_size.Width;
                selection_rest.Height = m_MouseDownPos.Y % tile_size.Height;

                SPosition selection_origin = m_MouseDownPos;
                SSize selection_size = tile_size;

                selection_size.Width += Math.Max(e.X - m_MouseDownPos.X, 0) + selection_rest.Width;
                selection_size.Height += Math.Max(e.Y - m_MouseDownPos.Y, 0) + selection_rest.Height;

                SurfaceTile.UpdateSelection(selection_origin, selection_size);

                SurfaceLevel.UpdateSelectedTile(SurfaceTile.GetCurrentSelectionOrigin(), SurfaceTile.GetCurrentSelectionSizeInTileCount());
            }
        }

        private void SurfaceLevel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                SurfaceLevel.SetLevelTile(e.X, e.Y);

                if (m_TileMode == ETileMode.Object)
                {
                    if (lbObjectSet.SelectedIndex == -1) return;

                    ObjectSetElementData element = m_ObjectSet.ObjectSet.Elements[lbObjectSet.SelectedIndex];

                    SPosition base_pos = SurfaceLevel.GetDesignTileScaledLevelBasePos();
                    float scalar = SurfaceLevel.GetScaleFactor();

                    CInsertedObject new_object = new CInsertedObject()
                    {
                        ObjectID = lbObjectSet.SelectedIndex,
                        Position = new SPosition(
                            (int)(e.X / scalar) - element.Size.Width / 2 + base_pos.X,
                            (int)(e.Y / scalar) - element.Size.Height / 2 + base_pos.Y)
                    };

                    SurfaceLevel.InsertedObjects.Add(new_object);

                    lbInsertedObjests.Items.Add("obj" + lbInsertedObjests.Items.Count.ToString() + 
                        "_" + element.ElementName);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                SurfaceLevel.SetLevelTile(e.X, e.Y, true);

                if (m_TileMode == ETileMode.Object)
                {
                    lbObjectSet.ClearSelected();
                }
            }
        }

        private void SurfaceLevel_MouseMove(object sender, MouseEventArgs e)
        {
            SurfaceLevel.UpdateMouseHoverPos(new SPosition(e.X, e.Y));

            if (e.Button == MouseButtons.Left)
            {
                SurfaceLevel.SetLevelTile(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                SurfaceLevel.SetLevelTile(e.X, e.Y, true);
            }
        }

        private void UpdateViews()
        {
            SplitViews.Height = this.ClientSize.Height - MainMenu.Height - SplitTab.Height;

            if (SurfaceTile != null)
            {
                int tile_x = (SurfaceTile.GetCurrentTileSheetWidth() - TileViewHScrollBar.Width) / KDefaultTileSize.Width;
                TileViewHScrollBar.Maximum = TileViewHScrollBar.LargeChange + tile_x;

                int tile_y = (SurfaceTile.GetCurrentTileSheetHeight() - TileViewVScrollBar.Height) / KDefaultTileSize.Height;
                TileViewVScrollBar.Maximum = TileViewVScrollBar.LargeChange + tile_y;

                SurfaceTile.Invalidate();
            }

            if (SurfaceLevel != null)
            {
                SSize level_size = SurfaceLevel.GetLevelSize();

                int level_x = 
                    ((int)(level_size.Width * SurfaceLevel.GetTileSize().Width * SurfaceLevel.GetScaleFactor())
                    - LevelViewHScrollBar.Width) /
                    (int)(SurfaceLevel.GetTileSize().Width * SurfaceLevel.GetScaleFactor());

                LevelViewHScrollBar.Maximum =
                    Math.Max(LevelViewHScrollBar.LargeChange + level_x, LevelViewHScrollBar.LargeChange - 1);

                int level_y = 
                    ((int)(level_size.Height * SurfaceLevel.GetTileSize().Height * SurfaceLevel.GetScaleFactor())
                    - LevelViewVScrollBar.Height) /
                    (int)(SurfaceLevel.GetTileSize().Height * SurfaceLevel.GetScaleFactor()) + 1;

                LevelViewVScrollBar.Maximum = 
                    Math.Max(LevelViewVScrollBar.LargeChange + level_y, LevelViewVScrollBar.LargeChange - 1);

                SurfaceLevel.Invalidate();

                LabelScale.Text = "배율: " + (int)(SurfaceLevel.GetScaleFactor() * 100) + " %";

                LabelLevelName.Text = "레벨 이름: " + m_LevelName;

                LabelLevelSize.Left = SplitViews.Panel2.Width - LabelLevelSize.Width;
                LabelLevelSize.Text = "레벨 크기: " + SurfaceLevel.GetLevelSize().Width + " x " + SurfaceLevel.GetLevelSize().Height;
            }
        }

        private void TileViewHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            SurfaceTile.UpdateTileBasePosX(e.NewValue);
        }

        private void TileViewVScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            SurfaceTile.UpdateTileBasePosY(e.NewValue);
        }

        private void LevelViewHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            SurfaceLevel.UpdateLevelBasePosX(e.NewValue);
        }

        private void LevelViewVScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            SurfaceLevel.UpdateLevelBasePosY(e.NewValue);
        }

        private void SplitViews_Panel1_Resize(object sender, EventArgs e)
        {
            UpdateViews();
        }

        private void SplitViews_Panel2_Resize(object sender, EventArgs e)
        {
            UpdateViews();
        }

        private void SurfaceLevel_MouseWheel(object sender, MouseEventArgs e)
        {
            int scroll = e.Delta / SystemInformation.MouseWheelScrollDelta;

            float factor = SurfaceLevel.GetScaleFactor();

            if (ModifierKeys == Keys.Control)
            {
                if (scroll > 0)
                {
                    factor += 0.1f;
                    factor = Math.Min(factor, 3.0f); // 최댓값 300%
                }
                else
                {
                    factor -= 0.1f;
                    factor = Math.Max(factor, 0.2f); // 최솟값 20%
                }
            }

            SurfaceLevel.SetScaleFactor(factor);

            UpdateViews();
        }

        private void TabTileView_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (TabTileView.SelectedIndex)
            {
                case 0:
                    m_TileMode = ETileMode.Design;

                    SurfaceTile.Visible = true;
                    SurfaceObjectSet.Visible = false;
                    SurfaceLevel.ShouldDrawTileOverlay = true;

                    SplitObjectView.Visible = false;
                    TileViewHScrollBar.Visible = true;
                    TileViewVScrollBar.Visible = true;
                    break;

                case 1:
                    m_TileMode = ETileMode.Movement;

                    SurfaceTile.Visible = true;
                    SurfaceObjectSet.Visible = false;
                    SurfaceLevel.ShouldDrawTileOverlay = true;

                    SplitObjectView.Visible = false;
                    TileViewHScrollBar.Visible = true;
                    TileViewVScrollBar.Visible = true;
                    break;

                case 2:
                    m_TileMode = ETileMode.Object;

                    SurfaceTile.Visible = false;
                    SurfaceObjectSet.Visible = true;
                    SurfaceLevel.ShouldDrawTileOverlay = false;

                    SplitObjectView.Visible = true;
                    TileViewHScrollBar.Visible = false;
                    TileViewVScrollBar.Visible = false;

                    UpdateObjectSet();

                    break;
                default:
                    break;
            }

            SurfaceTile.UpdateTileMode(m_TileMode);
            SurfaceLevel.UpdateTileMode(m_TileMode);

            SurfaceTile.UpdateSelection();
            SurfaceLevel.UpdateSelectedTile(SurfaceTile.GetCurrentSelectionOrigin(), SurfaceTile.GetCurrentSelectionSizeInTileCount());
        }

        private void 새로만들기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewLevel new_level = new NewLevel();

            if (new_level.ShowDialog() == DialogResult.OK)
            {
                m_LevelName = new_level.Controls["tbName"].Text;

                int level_size_x = Convert.ToInt32(new_level.Controls["numSizeX"].Text);
                int level_size_y = Convert.ToInt32(new_level.Controls["numSizeY"].Text);

                m_TileMode = ETileMode.Design;

                SurfaceTile.SetTileSheetTextures(ref m_DesignTileInfo, ref m_MovementTileInfo);

                SurfaceLevel.CreateLevel(level_size_x, level_size_y, m_DesignTileInfo, m_MovementTileInfo);

                UpdateMainFrmTitle();

                UpdateViews();
            }

            new_level.Dispose();
        }

        private void 저장하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgSaveFile.Filter = "Level file (.xml)|*.xml";
            dlgSaveFile.InitialDirectory = KAssetDir;
            dlgSaveFile.Title = "레벨 저장하기";
            dlgSaveFile.DefaultExt = "xml";

            DialogResult result = dlgSaveFile.ShowDialog();

            if (result == DialogResult.OK)
            {
                SerializeLevel(dlgSaveFile.FileName);
            }
        }

        private void 불러오기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Filter = "Level file (.xml)|*.xml";
            dlgOpenFile.InitialDirectory = KAssetDir;
            dlgOpenFile.Title = "레벨 불러오기";
            dlgOpenFile.DefaultExt = "xml";

            DialogResult result = dlgOpenFile.ShowDialog();

            if (result == DialogResult.OK)
            {
                DeserializeLevel(dlgOpenFile.FileName);

                lbInsertedObjests.ClearSelected();
                lbObjectSet.ClearSelected();

                SurfaceLevel.SelectedInsertedObjectIndex = lbInsertedObjests.SelectedIndex;
                SurfaceObjectSet.DrawingRectangle = new Rectangle(0, 0, 0, 0);
            }
        }

        private void SerializeLevel(string file_name)
        {
            ref int[,] ref_design_tiles = ref SurfaceLevel.GetDesignTilesRef();
            ref int[,] ref_movement_tiles = ref SurfaceLevel.GetMovementTilesRef();

            XmlDocument doc = new XmlDocument();

            XmlElement root = doc.CreateElement("DirectX11TutorialLevel");

            {
                XmlElement level_info = doc.CreateElement("level_info");

                level_info.SetAttribute("name", m_LevelName);
                level_info.SetAttribute("tile_size_width", SurfaceLevel.GetTileSize().Width.ToString());
                level_info.SetAttribute("tile_size_height", SurfaceLevel.GetTileSize().Height.ToString());
                level_info.SetAttribute("level_width", SurfaceLevel.GetLevelSize().Width.ToString());
                level_info.SetAttribute("level_height", SurfaceLevel.GetLevelSize().Height.ToString());
                level_info.SetAttribute("design_tileset", SurfaceLevel.GetTileModeInfoRef(ETileMode.Design).TileSheetFileName);
                level_info.SetAttribute("movement_tileset", SurfaceLevel.GetTileModeInfoRef(ETileMode.Movement).TileSheetFileName);
                level_info.SetAttribute("objectset", m_ObjectSet.ObjectSet.ObjectSetName);

                root.AppendChild(level_info);
            }

            {
                XmlElement desgin_tile_data = doc.CreateElement("design_tiles");

                int width = SurfaceLevel.GetLevelSize().Width;
                int height = SurfaceLevel.GetLevelSize().Height;

                for (int y = 0; y < height; ++y)
                {
                    XmlElement row = doc.CreateElement("row");
                    row.SetAttribute("id", y.ToString());

                    for (int x = 0; x < width; ++x)
                    {
                        row.SetAttribute("t" + x.ToString(), ref_design_tiles[x, y].ToString());
                    }

                    desgin_tile_data.AppendChild(row);
                }

                root.AppendChild(desgin_tile_data);
            }

            {
                XmlElement movement_tile_data = doc.CreateElement("movement_tiles");

                int width = SurfaceLevel.GetLevelSize().Width;
                int height = SurfaceLevel.GetLevelSize().Height;

                for (int y = 0; y < height; ++y)
                {
                    XmlElement row = doc.CreateElement("row");
                    row.SetAttribute("id", y.ToString());

                    for (int x = 0; x < width; ++x)
                    {
                        row.SetAttribute("t" + x.ToString(), ref_movement_tiles[x, y].ToString());
                    }

                    movement_tile_data.AppendChild(row);
                }

                root.AppendChild(movement_tile_data);
            }

            {
                XmlElement objects = doc.CreateElement("objects");
                objects.SetAttribute("object_count", SurfaceLevel.InsertedObjects.Count.ToString());

                foreach (CInsertedObject inserted in SurfaceLevel.InsertedObjects)
                {
                    XmlElement @object = doc.CreateElement("object");
                    @object.SetAttribute("id", inserted.ObjectID.ToString());
                    @object.SetAttribute("x", inserted.Position.X.ToString());
                    @object.SetAttribute("y", inserted.Position.Y.ToString());

                    objects.AppendChild(@object);
                }

                root.AppendChild(objects);
            }

            doc.AppendChild(root);

            doc.Save(file_name);

        }

        private void DeserializeLevel(string file_name)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(file_name);

            XmlElement root = doc.DocumentElement;

            int level_size_x;
            int level_size_y;
            {
                XmlElement level_info = (XmlElement)root.ChildNodes[0];

                XmlAttribute name = level_info.Attributes[0];
                m_LevelName = name.Value;

                XmlAttribute tile_size_width = level_info.Attributes[1];
                m_DesignTileInfo.TileSize.Width = m_MovementTileInfo.TileSize.Width =
                    Convert.ToInt32(tile_size_width.Value);

                XmlAttribute tile_size_height = level_info.Attributes[2];
                m_DesignTileInfo.TileSize.Height = m_MovementTileInfo.TileSize.Height =
                    Convert.ToInt32(tile_size_height.Value);

                XmlAttribute level_size_width = level_info.Attributes[3];
                level_size_x = Convert.ToInt32(level_size_width.Value);

                XmlAttribute level_size_height = level_info.Attributes[4];
                level_size_y = Convert.ToInt32(level_size_height.Value);

                XmlAttribute design_tileset = level_info.Attributes[5];
                m_DesignTileInfo.TileSheetFileName = design_tileset.Value;

                XmlAttribute movement_tileset = level_info.Attributes[6];
                m_MovementTileInfo.TileSheetFileName = movement_tileset.Value;

                lbObjectSet.Items.Clear();
                if (level_info.Attributes.Count > 7)
                {
                    XmlAttribute object_set = level_info.Attributes[7];
                    m_ObjectSet.LoadFromFile(KAssetDir + object_set.Value + ".xml");

                    UpdateObjectSet();
                }

                SurfaceTile.SetTileSheetTextures(ref m_DesignTileInfo, ref m_MovementTileInfo);

                SurfaceLevel.CreateLevel(level_size_x, level_size_y, m_DesignTileInfo, m_MovementTileInfo);


                ref int[,] ref_design_tiles = ref SurfaceLevel.GetDesignTilesRef();
                {
                    XmlElement desgin_tile_data = (XmlElement)root.ChildNodes[1];

                    for (int y = 0; y < level_size_y; ++y)
                    {
                        XmlElement row = (XmlElement)desgin_tile_data.ChildNodes[y];

                        int row_id = Convert.ToInt32(row.Attributes["id"].Value);

                        for (int x = 0; x < level_size_x; ++x)
                        {
                            XmlAttribute tile = row.Attributes["t" + x.ToString()];

                            ref_design_tiles[x, row_id] = Convert.ToInt32(tile.Value);
                        }
                    }
                }


                ref int[,] ref_movement_tiles = ref SurfaceLevel.GetMovementTilesRef();
                {
                    XmlElement movement_tile_data = (XmlElement)root.ChildNodes[2];

                    for (int y = 0; y < level_size_y; ++y)
                    {
                        XmlElement row = (XmlElement)movement_tile_data.ChildNodes[y];

                        int row_id = Convert.ToInt32(row.Attributes["id"].Value);

                        for (int x = 0; x < level_size_x; ++x)
                        {
                            XmlAttribute tile = row.Attributes["t" + x.ToString()];

                            ref_movement_tiles[x, row_id] = Convert.ToInt32(tile.Value);
                        }
                    }
                }


                SurfaceLevel.InsertedObjects.Clear();
                lbInsertedObjests.Items.Clear();
                if (root.ChildNodes.Count > 3)
                {
                    XmlElement objects = (XmlElement)root.ChildNodes[3];
                    int object_count = Convert.ToInt32(objects.GetAttribute("object_count"));
                    for (int i = 0; i < object_count; ++i)
                    {
                        XmlElement @object = (XmlElement)objects.ChildNodes[i];

                        CInsertedObject inserted = new CInsertedObject();
                        inserted.ObjectID = Convert.ToInt32(@object.GetAttribute("id"));
                        inserted.Position.X = Convert.ToInt32(@object.GetAttribute("x"));
                        inserted.Position.Y = Convert.ToInt32(@object.GetAttribute("y"));

                        SurfaceLevel.InsertedObjects.Add(inserted);

                        ObjectSetElementData element = m_ObjectSet.ObjectSet.Elements[inserted.ObjectID];

                        lbInsertedObjests.Items.Add("obj" + lbInsertedObjests.Items.Count.ToString() +
                        "_" + element.ElementName);
                    }
                }
            }

            UpdateMainFrmTitle();

            UpdateViews();
        }

        private void UpdateMainFrmTitle()
        {
            if (m_LevelName == null)
            {
                this.Text = KMainFormTitle;
                return;
            }

            this.Text = KMainFormTitle + " : " + m_LevelName;
        }

        private void 타일불러오기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Filter = "Tileset texture (.png)|*.png";
            dlgOpenFile.InitialDirectory = KAssetDir;
            dlgOpenFile.Title = "타일셋 불러오기";
            dlgOpenFile.DefaultExt = "png";

            DialogResult result = dlgOpenFile.ShowDialog();

            if (result == DialogResult.OK)
            {
                string[] file_names = dlgOpenFile.FileName.Split('\\');

                m_DesignTileInfo.TileSheetFileName = file_names[file_names.Length - 1];

                SurfaceTile.SetTileSheetTextures(ref m_DesignTileInfo, ref m_MovementTileInfo);

                SurfaceLevel.CreateLevel(KDefaultLevelSize.Width, KDefaultLevelSize.Height, m_DesignTileInfo, m_MovementTileInfo);

                UpdateMainFrmTitle();

                UpdateViews();
            }
        }

        private void 타일크기설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewTilesetSize new_tileset_size = new NewTilesetSize();

            if (new_tileset_size.ShowDialog() == DialogResult.OK)
            {
                m_DesignTileInfo.TileSize.Width = m_MovementTileInfo.TileSize.Width = Convert.ToInt32(new_tileset_size.Controls["numSizeX"].Text);
                m_DesignTileInfo.TileSize.Height = m_MovementTileInfo.TileSize.Height = Convert.ToInt32(new_tileset_size.Controls["numSizeY"].Text);

                SurfaceTile.SetTileSheetTextures(ref m_DesignTileInfo, ref m_MovementTileInfo);

                SurfaceLevel.CreateLevel(KDefaultLevelSize.Width, KDefaultLevelSize.Height, m_DesignTileInfo, m_MovementTileInfo);

                UpdateMainFrmTitle();

                UpdateViews();
            }

            new_tileset_size.Dispose();
        }

        private void 오브젝트셋지정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Filter = "Objectset file (.xml)|*.xml";
            dlgOpenFile.InitialDirectory = KAssetDir;
            dlgOpenFile.Title = "오브젝트셋 불러오기";
            dlgOpenFile.DefaultExt = "xml";

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                m_ObjectSet.LoadFromFile(dlgOpenFile.FileName);

                UpdateObjectSet();
            }
        }

        private void UpdateObjectSet()
        {
            lbObjectSet.Items.Clear();

            if (m_ObjectSet.ObjectSet != null)
            {
                foreach (ObjectSetElementData element in m_ObjectSet.ObjectSet.Elements)
                {
                    lbObjectSet.Items.Add(element.ElementName);
                }

                SurfaceObjectSet.ClearTextures();

                SurfaceObjectSet.AddTexture(m_ObjectSet.ObjectSet.TextureFileName);

                SurfaceObjectSet.Invalidate();

                SurfaceLevel.SetObjectSet(m_ObjectSet.ObjectSet);
            }
        }

        private void LbObjectSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbObjectSet.Items.Count > 0)
            {
                if (lbObjectSet.SelectedIndex == -1)
                {
                    SurfaceObjectSet.DrawingRectangle = new Rectangle(0, 0, 0, 0);
                }
                else
                {
                    ObjectSetElementData element = m_ObjectSet.ObjectSet.Elements[lbObjectSet.SelectedIndex];

                    SurfaceObjectSet.DrawingRectangle = new Rectangle(
                        element.OffsetU, element.OffsetV, element.Size.Width, element.Size.Height);
                }
               

                SurfaceLevel.ObjectOverlayRectangle = SurfaceObjectSet.DrawingRectangle;

                SurfaceObjectSet.Invalidate();

                SurfaceLevel.Invalidate();
            }
        }

        private void LbInsertedObjests_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbInsertedObjests.Items.Count > 0)
            {
                SurfaceLevel.SelectedInsertedObjectIndex = lbInsertedObjests.SelectedIndex;

                SurfaceLevel.Invalidate();
            }
        }

        private void LbInsertedObjests_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (lbInsertedObjests.SelectedIndex != -1)
                {
                    SurfaceLevel.InsertedObjects.RemoveAt(lbInsertedObjests.SelectedIndex);

                    lbInsertedObjests.Items.RemoveAt(lbInsertedObjests.SelectedIndex);

                    SurfaceLevel.SelectedInsertedObjectIndex = lbInsertedObjests.SelectedIndex;

                    SurfaceLevel.Invalidate();
                }
            }
        }
    }
}
