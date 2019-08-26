using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
//using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace DirectX11TutorialLevelEditor
{
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
        Movement
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

        private ETileMode m_TileMode = ETileMode.Design;
        private string m_LevelName = KDefaultLevelName;

        private SPosition m_MouseDownPos;

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

            KAssetDir = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "/asset/";

            SurfaceTile = new MGSurfaceTile(KAssetDir)
            {
                Parent = SplitViews.Panel1,
                Dock = DockStyle.Fill,
                FixedMovementTileSize = KDefaultTileSize
            };

            SurfaceLevel = new MGSurfaceLevel(KAssetDir)
            {
                Parent = SplitViews.Panel2,
                Dock = DockStyle.Fill,
                FixedMovementTileSize = KDefaultTileSize
            };

            SurfaceTile.MouseMove += SurfaceTile_MouseMove;
            SurfaceTile.MouseDown += SurfaceTile_MouseDown;

            SurfaceLevel.MouseMove += SurfaceLevel_MouseMove;
            SurfaceLevel.MouseDown += SurfaceLevel_MouseDown;
            SurfaceLevel.MouseWheel += SurfaceLevel_MouseWheel;

            this.KeyPreview = true;
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            SurfaceTile.SetTileSheetTextures(ref m_DesignTileInfo, ref m_MovementTileInfo);

            SurfaceLevel.CreateLevel(KDefaultLevelSize.Width, KDefaultLevelSize.Height, m_DesignTileInfo, m_MovementTileInfo);

            UpdateMainFrmTitle();

            UpdateViews();
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
            }
            else if (e.Button == MouseButtons.Right)
            {
                SurfaceLevel.SetLevelTile(e.X, e.Y, true);
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


            if (SurfaceTile == null) return;

            int tile_x = (SurfaceTile.GetCurrentTileSheetWidth() - TileViewHScrollBar.Width) / KDefaultTileSize.Width;
            TileViewHScrollBar.Maximum = TileViewHScrollBar.LargeChange + tile_x;

            int tile_y = (SurfaceTile.GetCurrentTileSheetHeight() - TileViewVScrollBar.Height) / KDefaultTileSize.Height;
            TileViewVScrollBar.Maximum = TileViewVScrollBar.LargeChange + tile_y;

            SurfaceTile.Invalidate();


            if (SurfaceLevel == null) return;

            SSize level_size = SurfaceLevel.GetLevelSize();

            int level_x = (level_size.Width * KDefaultTileSize.Width - LevelViewHScrollBar.Width) / KDefaultTileSize.Width;
            LevelViewHScrollBar.Maximum = Math.Max(LevelViewHScrollBar.LargeChange + level_x, LevelViewHScrollBar.LargeChange - 1);

            int level_y = (level_size.Height * KDefaultTileSize.Height - LevelViewVScrollBar.Height) / KDefaultTileSize.Height + 1;
            LevelViewVScrollBar.Maximum = Math.Max(LevelViewVScrollBar.LargeChange + level_y, LevelViewVScrollBar.LargeChange - 1);

            SurfaceLevel.Invalidate();

            LabelScale.Text = "배율: " + (int)(SurfaceLevel.GetScaleFactor() * 100) + " %";

            LabelLevelName.Text = "레벨 이름: " + m_LevelName;

            LabelLevelSize.Left = SplitViews.Panel2.Width - LabelLevelSize.Width;
            LabelLevelSize.Text = "레벨 크기: " + SurfaceLevel.GetLevelSize().Width + " x " + SurfaceLevel.GetLevelSize().Height;
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
            if (TabTileView.SelectedIndex == 0)
            {
                m_TileMode = ETileMode.Design;
            }
            else
            {
                m_TileMode = ETileMode.Movement;
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
            dlgSaveFile.Filter = "Level|*.xml";
            dlgSaveFile.InitialDirectory = KAssetDir;
            dlgSaveFile.Title = "저장하기";
            dlgSaveFile.DefaultExt = "xml";

            DialogResult result = dlgSaveFile.ShowDialog();

            if (result == DialogResult.OK)
            {
                SerializeLevel(dlgSaveFile.FileName);
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

            doc.AppendChild(root);

            doc.Save(file_name);

        }

        private void 불러오기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Filter = "Level file (.xml)|*.xml";
            dlgOpenFile.InitialDirectory = KAssetDir;
            dlgOpenFile.Title = "불러오기";
            dlgOpenFile.DefaultExt = "xml";

            DialogResult result = dlgOpenFile.ShowDialog();

            if (result == DialogResult.OK)
            {
                DeserializeLevel(dlgOpenFile.FileName);
            }
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

        private void 불러오기ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            dlgOpenFile.Filter = "Tileset texture (.png)|*.png";
            dlgOpenFile.InitialDirectory = KAssetDir;
            dlgOpenFile.Title = "불러오기";
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

        private void 크기설정ToolStripMenuItem_Click(object sender, EventArgs e)
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
        }
    }
}
