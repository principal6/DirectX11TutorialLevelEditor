using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using System.Drawing;
using System.Linq;
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

    public enum EHistoryAction
    {
        SetTile, // IntData = { X, Y, OldTileID, NewTileID }
        ChangeTileMode,
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
        static readonly string KMainFormTitle = "DirectX11Tutorial Level Editor";
        static readonly string KDefaultLevelName = "level_new";
        readonly string KAssetDir;
        readonly SSize KDefaultTileSize = new SSize(64, 64);
        readonly SSize KDefaultLevelSize = new SSize(5, 3);

        MGSurfaceTile SurfaceTile;
        MGSurfaceLevel SurfaceLevel;

        ETileMode m_TileMode = ETileMode.Design;
        string m_LevelName = KDefaultLevelName;

        SPosition m_MouseDownPos;

        public MainFrm()
        {
            InitializeComponent();

            KAssetDir = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "/asset/";

            SurfaceTile = new MGSurfaceTile(KAssetDir);
            SurfaceTile.Parent = SplitViews.Panel1;
            SurfaceTile.Dock = DockStyle.Fill;

            SurfaceLevel = new MGSurfaceLevel(KAssetDir);
            SurfaceLevel.Parent = SplitViews.Panel2;
            SurfaceLevel.Dock = DockStyle.Fill;

            SurfaceTile.MouseMove += SurfaceTile_MouseMove;
            SurfaceTile.MouseDown += SurfaceTile_MouseDown;

            SurfaceLevel.MouseMove += SurfaceLevel_MouseMove;
            SurfaceLevel.MouseDown += SurfaceLevel_MouseDown;

            this.KeyPreview = true;
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            STileModeInfo design_tile_info = new STileModeInfo();
            design_tile_info.TileSheetFileName = "grass_64x64.png";
            design_tile_info.TileSize = KDefaultTileSize;

            STileModeInfo movement_tile_info = new STileModeInfo();
            movement_tile_info.TileSheetFileName = "movement_platformer_64x64.png";
            movement_tile_info.TileSize = KDefaultTileSize;

            SurfaceTile.SetTileSheetTextures(ref design_tile_info, ref movement_tile_info);

            SurfaceLevel.CreateLevel(KDefaultLevelSize.Width, KDefaultLevelSize.Height, design_tile_info, movement_tile_info);

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

                STileModeInfo design_tile_info = new STileModeInfo();
                design_tile_info.TileSheetFileName = "grass_64x64.png";
                design_tile_info.TileSize = KDefaultTileSize;

                STileModeInfo movement_tile_info = new STileModeInfo();
                movement_tile_info.TileSheetFileName = "movement_platformer_64x64.png";
                movement_tile_info.TileSize = KDefaultTileSize;

                SurfaceTile.SetTileSheetTextures(ref design_tile_info, ref movement_tile_info);

                SurfaceLevel.CreateLevel(level_size_x, level_size_y, design_tile_info, movement_tile_info);

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
                STileModeInfo design_tile_info = new STileModeInfo();
                design_tile_info.TileSize = KDefaultTileSize;

                STileModeInfo movement_tile_info = new STileModeInfo();
                movement_tile_info.TileSize = KDefaultTileSize;

                XmlElement level_info = (XmlElement)root.ChildNodes[0];

                XmlAttribute name = level_info.Attributes[0];
                m_LevelName = name.Value;

                XmlAttribute tile_size_width = level_info.Attributes[1];
                design_tile_info.TileSize.Width = movement_tile_info.TileSize.Width =
                    Convert.ToInt32(tile_size_width.Value);

                XmlAttribute tile_size_height = level_info.Attributes[2];
                design_tile_info.TileSize.Height = movement_tile_info.TileSize.Height =
                    Convert.ToInt32(tile_size_height.Value);

                XmlAttribute level_size_width = level_info.Attributes[3];
                level_size_x = Convert.ToInt32(level_size_width.Value);

                XmlAttribute level_size_height = level_info.Attributes[4];
                level_size_y = Convert.ToInt32(level_size_height.Value);

                XmlAttribute design_tileset = level_info.Attributes[5];
                design_tile_info.TileSheetFileName = design_tileset.Value;

                XmlAttribute movement_tileset = level_info.Attributes[6];
                movement_tile_info.TileSheetFileName = movement_tileset.Value;



                SurfaceTile.SetTileSheetTextures(ref design_tile_info, ref movement_tile_info);

                SurfaceLevel.CreateLevel(level_size_x, level_size_y, design_tile_info, movement_tile_info);


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
    }



    public class MGTextureData
    {
        public Texture2D Texture;
        public Rectangle Rect;
        public Color BlendColor = Color.White;
    }

    public class MGSurface : MonoGame.Forms.Controls.InvalidationControl
    {
        protected string m_AssetDir;
        protected Color m_BGColor = Color.White;
        protected List<MGTextureData> m_Textures = new List<MGTextureData>();

        public MGSurface(string asset_dir)
        {
            m_AssetDir = asset_dir;
        }

        public Texture2D CreateBlankTexture2D(int width, int height)
        {
            Texture2D blank_texture = new Texture2D(Editor.graphics, width, height);

            Color[] data = new Color[width * height];

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.White;
            }

            blank_texture.SetData(data);

            return blank_texture;
        }

        public void AddTexture(string texture_file_name)
        {
            m_Textures.Add(new MGTextureData());

            Texture2D texture = Texture2D.FromStream(Editor.graphics, File.OpenRead(m_AssetDir + texture_file_name));

            m_Textures.ElementAt(m_Textures.Count - 1).Texture = texture;
            m_Textures.ElementAt(m_Textures.Count - 1).Rect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public void AddTexture(Texture2D texture)
        {
            m_Textures.Add(new MGTextureData());

            m_Textures.ElementAt(m_Textures.Count - 1).Texture = texture;
            m_Textures.ElementAt(m_Textures.Count - 1).Rect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Draw()
        {
            base.Draw();

            Editor.graphics.Clear(m_BGColor);
        }

        protected void BeginDrawing()
        {
            Editor.spriteBatch.Begin();
        }

        protected void DrawAllTextures()
        {
            foreach (MGTextureData i in m_Textures)
            {
                Editor.spriteBatch.Draw(i.Texture, i.Rect, i.BlendColor * ((float)i.BlendColor.A / 255.0f));
            }
        }

        protected void EndDrawing()
        {
            Editor.spriteBatch.End();
        }
    }

    public class MGSurfaceTile : MGSurface
    {
        ETileMode m_TileMode;
        STileModeInfo m_DesignTileInfo;
        STileModeInfo m_MovementTileInfo;

        public MGSurfaceTile(string asset_dir) : base(asset_dir) { }

        protected override void Initialize()
        {
            base.Initialize();

            m_BGColor = new Color(0.0f, 0.0f, 0.3f, 1.0f);
        }

        protected override void Draw()
        {
            base.Draw();

            BeginDrawing();

            int texture_index = GetCurrentTileModeTextureIndex();

            Editor.spriteBatch.Draw(m_Textures[texture_index].Texture, m_Textures[texture_index].Rect,
                m_Textures[texture_index].BlendColor * ((float)m_Textures[texture_index].BlendColor.A / 255.0f));

            ref STileModeInfo tile_mode = ref GetCurrentTileModeInfoRef();

            if ((tile_mode.SelectionSizeInTileCount.Width >= 1) || (tile_mode.SelectionSizeInTileCount.Height >= 1))
            {
                Rectangle rect = m_Textures[2].Rect;

                for (int x = 0; x < tile_mode.SelectionSizeInTileCount.Width; ++x)
                {
                    for (int y = 0; y < tile_mode.SelectionSizeInTileCount.Height; ++y)
                    {
                        rect.X = m_Textures[2].Rect.X + x * tile_mode.TileSize.Width;
                        rect.Y = m_Textures[2].Rect.Y + y * tile_mode.TileSize.Height;

                        Editor.spriteBatch.Draw(m_Textures[2].Texture, rect,
                            m_Textures[2].BlendColor * ((float)m_Textures[2].BlendColor.A / 255.0f));
                    }
                }
            }
            else
            {
                Editor.spriteBatch.Draw(m_Textures[2].Texture, m_Textures[2].Rect,
                   m_Textures[2].BlendColor * ((float)m_Textures[2].BlendColor.A / 255.0f));
            }

            EndDrawing();
        }

        public void UpdateTileMode(ETileMode mode)
        {
            m_TileMode = mode;

            Invalidate();
        }

        public void SetTileSheetTextures(ref STileModeInfo design, ref STileModeInfo movement)
        {
            Debug.Assert(design.TileSize == movement.TileSize);

            m_Textures.Clear();

            AddTexture(design.TileSheetFileName);
            AddTexture(movement.TileSheetFileName);
            AddTexture(CreateBlankTexture2D(64, 64));

            design.TileSheetSizeInTileCount.Width = m_Textures[0].Texture.Width / design.TileSize.Width;
            design.TileSheetSizeInTileCount.Height = m_Textures[0].Texture.Height / design.TileSize.Height;
            design.SelectionSizeInTileCount.Width = 1;
            design.SelectionSizeInTileCount.Height = 1;

            movement.TileSheetSizeInTileCount.Width = m_Textures[1].Texture.Width / movement.TileSize.Width;
            movement.TileSheetSizeInTileCount.Height = m_Textures[1].Texture.Height / movement.TileSize.Height;
            movement.SelectionSizeInTileCount.Width = 1;
            movement.SelectionSizeInTileCount.Height = 1;

            m_DesignTileInfo = design;
            m_MovementTileInfo = movement;

            m_Textures[2].BlendColor = new Color(0.0f, 0.3f, 0.8f, 0.5f);
        }

        public int GetCurrentTileSheetWidth()
        {
            return m_Textures[GetCurrentTileModeTextureIndex()].Texture.Width;
        }

        public int GetCurrentTileSheetHeight()
        {
            return m_Textures[GetCurrentTileModeTextureIndex()].Texture.Height;
        }

        private int GetCurrentTileModeTextureIndex()
        {
            if (m_TileMode == ETileMode.Design)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public void UpdateTileBasePosX(int value)
        {
            ref STileModeInfo tile_mode = ref GetCurrentTileModeInfoRef();

            tile_mode.TileSelectionOffset.X = value;

            UpdateSelection();
        }

        public void UpdateTileBasePosY(int value)
        {
            ref STileModeInfo tile_mode = ref GetCurrentTileModeInfoRef();

            tile_mode.TileSelectionOffset.Y = value;

            UpdateSelection();
        }

        public void UpdateSelection(SPosition selection_origin, SSize selection_size)
        {
            ref STileModeInfo tile_mode = ref GetCurrentTileModeInfoRef();

            int x_max = tile_mode.TileSheetSizeInTileCount.Width - 1;
            int y_max = tile_mode.TileSheetSizeInTileCount.Height - 1;

            tile_mode.SelectionOrigin.X = tile_mode.TileSelectionOffset.X + (selection_origin.X / tile_mode.TileSize.Width);
            tile_mode.SelectionOrigin.X = Math.Min(tile_mode.SelectionOrigin.X, x_max);

            tile_mode.SelectionOrigin.Y = tile_mode.TileSelectionOffset.Y + (selection_origin.Y / tile_mode.TileSize.Height);
            tile_mode.SelectionOrigin.Y = Math.Min(tile_mode.SelectionOrigin.Y, y_max);

            tile_mode.SelectionSizeInTileCount.Width = selection_size.Width / tile_mode.TileSize.Width;
            tile_mode.SelectionSizeInTileCount.Height = selection_size.Height / tile_mode.TileSize.Height;

            if (tile_mode.SelectionOrigin.X + tile_mode.SelectionSizeInTileCount.Width > x_max)
            {
                tile_mode.SelectionSizeInTileCount.Width = x_max - tile_mode.SelectionOrigin.X + 1;
            }

            if (tile_mode.SelectionOrigin.Y + tile_mode.SelectionSizeInTileCount.Height > y_max)
            {
                tile_mode.SelectionSizeInTileCount.Height = y_max - tile_mode.SelectionOrigin.Y + 1;
            }

            UpdateSelection();
        }

        public void UpdateSelection()
        {
            ref STileModeInfo tile_mode = ref GetCurrentTileModeInfoRef();

            m_Textures[2].Rect.X = (-tile_mode.TileSelectionOffset.X + tile_mode.SelectionOrigin.X) * tile_mode.TileSize.Width;
            m_Textures[2].Rect.Y = (-tile_mode.TileSelectionOffset.Y + tile_mode.SelectionOrigin.Y) * tile_mode.TileSize.Height;

            m_Textures[0].Rect.X = -tile_mode.TileSelectionOffset.X * tile_mode.TileSize.Width;
            m_Textures[0].Rect.Y = -tile_mode.TileSelectionOffset.Y * tile_mode.TileSize.Height;

            Invalidate();
        }

        private ref STileModeInfo GetCurrentTileModeInfoRef()
        {
            if (m_TileMode == ETileMode.Design)
            {
                return ref m_DesignTileInfo;
            }
            else
            {
                return ref m_MovementTileInfo;
            }
        }

        public SPosition GetCurrentSelectionOrigin()
        {
            ref STileModeInfo tile_mode = ref GetCurrentTileModeInfoRef();

            return tile_mode.SelectionOrigin;
        }

        public SSize GetCurrentSelectionSizeInTileCount()
        {
            ref STileModeInfo tile_mode = ref GetCurrentTileModeInfoRef();

            return tile_mode.SelectionSizeInTileCount;
        }

        public SSize GetTileSize()
        {
            return GetCurrentTileModeInfoRef().TileSize;
        }
    }

    public class MGSurfaceLevel : MGSurface
    {
        private int[,] m_LevelTilesDesign;
        private int[,] m_LevelTilesMovement;
        private Color HoverColor;
        private SSize m_LevelSizeInTileCount;

        private SPosition m_MouseHoverPos;
        private SPosition m_LevelBasePos;

        private ETileMode m_TileMode;
        private STileModeInfo m_DesignTileInfo;
        private STileModeInfo m_MovementTileInfo;

        private Stack<CHistory> m_HistoryStack = new Stack<CHistory>();
        private Stack<CHistory> m_UnHistoryStack = new Stack<CHistory>();
        private int m_HistoryStackLastGroup = 0;
        private int m_UnHistoryStackLastGroup = 0;

        public MGSurfaceLevel(string asset_dir) : base(asset_dir) { }

        protected override void Initialize()
        {
            base.Initialize();

            m_BGColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);

            HoverColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        }

        protected override void Draw()
        {
            base.Draw();

            BeginDrawing();

            ref STileModeInfo curr_mode = ref GetCurrentTileModeInfoRef();
            int texture_index = GetCurrentTileModeTextureIndex();

            if (m_LevelTilesDesign != null)
            {
                m_Textures[0].Rect.X = -m_LevelBasePos.X * curr_mode.TileSize.Width;
                m_Textures[0].Rect.Y = -m_LevelBasePos.Y * curr_mode.TileSize.Height;
                Editor.spriteBatch.Draw(m_Textures[0].Texture, m_Textures[0].Rect, m_Textures[0].BlendColor);

                float blend_factor = 1.0f;
                if (m_TileMode == ETileMode.Movement)
                {
                    blend_factor = 0.6f;
                }

                for (int x = 0; x < m_LevelSizeInTileCount.Width; ++x)
                {
                    for (int y = 0; y < m_LevelSizeInTileCount.Height; ++y)
                    {
                        if (m_LevelTilesDesign[x, y] == -1) continue;

                        int offset_x = -m_LevelBasePos.X + x;
                        int offset_y = -m_LevelBasePos.Y + y;

                        if ((offset_x < 0) || (offset_y < 0))
                        {
                            continue;
                        }


                        SPosition tile_xy = GetTileXYFromTileID(ref m_DesignTileInfo, m_LevelTilesDesign[x, y]);

                        Rectangle rect_dest = new Rectangle(
                                offset_x * m_DesignTileInfo.TileSize.Width, offset_y * m_DesignTileInfo.TileSize.Height,
                                m_DesignTileInfo.TileSize.Width, m_DesignTileInfo.TileSize.Height);

                        Rectangle rect_src = new Rectangle(
                                tile_xy.X * m_DesignTileInfo.TileSize.Width, tile_xy.Y * m_DesignTileInfo.TileSize.Height,
                                m_DesignTileInfo.TileSize.Width, m_DesignTileInfo.TileSize.Height);

                        Editor.spriteBatch.Draw(m_Textures[1].Texture,
                            rect_dest, rect_src, m_Textures[1].BlendColor * blend_factor);
                    }
                }

                if (m_TileMode == ETileMode.Movement)
                {
                    for (int x = 0; x < m_LevelSizeInTileCount.Width; ++x)
                    {
                        for (int y = 0; y < m_LevelSizeInTileCount.Height; ++y)
                        {
                            int offset_x = -m_LevelBasePos.X + x;
                            int offset_y = -m_LevelBasePos.Y + y;

                            if ((offset_x < 0) || (offset_y < 0))
                            {
                                continue;
                            }


                            SPosition tile_xy = GetTileXYFromTileID(ref m_MovementTileInfo, m_LevelTilesMovement[x, y]);

                            Rectangle rect_dest = new Rectangle(
                                    offset_x * m_MovementTileInfo.TileSize.Width, offset_y * m_MovementTileInfo.TileSize.Height,
                                    m_MovementTileInfo.TileSize.Width, m_MovementTileInfo.TileSize.Height);

                            Rectangle rect_src = new Rectangle(
                                    tile_xy.X * m_MovementTileInfo.TileSize.Width, tile_xy.Y * m_MovementTileInfo.TileSize.Height,
                                    m_MovementTileInfo.TileSize.Width, m_MovementTileInfo.TileSize.Height);

                            Editor.spriteBatch.Draw(m_Textures[2].Texture,
                                rect_dest, rect_src, m_Textures[2].BlendColor * 0.6f);
                        }
                    }

                }

            }


            {
                SPosition tile_xy = curr_mode.SelectionOrigin;

                Rectangle rect_dest = new Rectangle(
                        m_MouseHoverPos.X * curr_mode.TileSize.Width,
                        m_MouseHoverPos.Y * curr_mode.TileSize.Height,
                        curr_mode.SelectionSizeInTileCount.Width * curr_mode.TileSize.Width,
                        curr_mode.SelectionSizeInTileCount.Height * curr_mode.TileSize.Height);

                Rectangle rect_src = new Rectangle(
                        tile_xy.X * curr_mode.TileSize.Width,
                        tile_xy.Y * curr_mode.TileSize.Height,
                        curr_mode.SelectionSizeInTileCount.Width * curr_mode.TileSize.Width,
                        curr_mode.SelectionSizeInTileCount.Height * curr_mode.TileSize.Height);

                Editor.spriteBatch.Draw(m_Textures[texture_index].Texture,
                    rect_dest, rect_src, HoverColor);
            }

            EndDrawing();
        }

        public void UpdateTileMode(ETileMode mode)
        {
            if (m_TileMode == mode)
            {
                return;
            }

            ++m_HistoryStackLastGroup;

            CHistory new_history = new CHistory();
            new_history.eAction = EHistoryAction.ChangeTileMode;
            new_history.GroupID = m_HistoryStackLastGroup;
            new_history.CurrTileMode = m_TileMode;
            m_HistoryStack.Push(new_history);

            m_TileMode = mode;

            Invalidate();
        }

        public void UpdateMouseHoverPos(SPosition pos)
        {
            m_MouseHoverPos.X = pos.X / GetCurrentTileModeInfoRef().TileSize.Width;
            m_MouseHoverPos.Y = pos.Y / GetCurrentTileModeInfoRef().TileSize.Height;

            Invalidate();
        }

        public void UpdateSelectedTile(SPosition sel_origin, SSize sel_size)
        {
            ref STileModeInfo curr_mode = ref GetCurrentTileModeInfoRef();

            curr_mode.SelectionOrigin = sel_origin;
            curr_mode.SelectionSizeInTileCount = sel_size;
        }

        public void UpdateLevelBasePosX(int value)
        {
            m_LevelBasePos.X = value;

            Invalidate();
        }

        public void UpdateLevelBasePosY(int value)
        {
            m_LevelBasePos.Y = value;

            Invalidate();
        }

        public void SetLevelTile(int mouse_x, int mouse_y, bool should_erase = false)
        {
            bool is_set_new_tile = false;

            ref STileModeInfo tile_mode = ref GetCurrentTileModeInfoRef();
            ref int[,] tiles = ref GetCurrentModeTilesRef();

            int x_in_tiles = mouse_x / tile_mode.TileSize.Width;
            int y_in_tiles = mouse_y / tile_mode.TileSize.Height;

            int offset_x = m_LevelBasePos.X + x_in_tiles;
            int offset_y = m_LevelBasePos.Y + y_in_tiles;

            if (((offset_x >= 0) && (offset_x < m_LevelSizeInTileCount.Width)) &&
                ((offset_y >= 0) && (offset_y < m_LevelSizeInTileCount.Height)))
            {
                SPosition sized_selection = tile_mode.SelectionOrigin;
                int sized_x;
                int sized_y;

                ++m_HistoryStackLastGroup;

                for (int x = 0; x < tile_mode.SelectionSizeInTileCount.Width; ++x)
                {
                    for (int y = 0; y < tile_mode.SelectionSizeInTileCount.Height; ++y)
                    {
                        sized_x = offset_x + x;
                        sized_y = offset_y + y;

                        if ((sized_x < m_LevelSizeInTileCount.Width) && (sized_y < m_LevelSizeInTileCount.Height))
                        {
                            sized_selection.X = tile_mode.SelectionOrigin.X + x;
                            sized_selection.Y = tile_mode.SelectionOrigin.Y + y;

                            int new_tile_id = GetTileIDFromTileXY(ref tile_mode, sized_selection);

                            if (should_erase == true)
                            {
                                if (m_TileMode == ETileMode.Design)
                                {
                                    new_tile_id = -1;
                                }
                                else
                                {
                                    new_tile_id = 0;
                                }
                            }

                            ref int old_tile_id = ref tiles[sized_x, sized_y];

                            if (old_tile_id == new_tile_id)
                            {
                                continue;
                            }

                            is_set_new_tile = true;

                            CHistory new_history = new CHistory();
                            new_history.GroupID = m_HistoryStackLastGroup;
                            new_history.eAction = EHistoryAction.SetTile;
                            new_history.CurrTileMode = m_TileMode;
                            new_history.IntData[0] = sized_x;
                            new_history.IntData[1] = sized_y;
                            new_history.IntData[2] = old_tile_id;
                            new_history.IntData[3] = new_tile_id;

                            m_HistoryStack.Push(new_history);

                            old_tile_id = new_tile_id;
                        }
                    }
                }

                if (is_set_new_tile == false)
                {
                    --m_HistoryStackLastGroup;
                }
            }

            Invalidate();
        }

        public SSize GetLevelSize()
        {
            return m_LevelSizeInTileCount;
        }

        public SSize GetTileSize()
        {
            return GetCurrentTileModeInfoRef().TileSize;
        }

        private int GetCurrentTileModeTextureIndex()
        {
            if (m_TileMode == ETileMode.Design)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        private ref STileModeInfo GetCurrentTileModeInfoRef()
        {
            if (m_TileMode == ETileMode.Design)
            {
                return ref m_DesignTileInfo;
            }
            else
            {
                return ref m_MovementTileInfo;
            }
        }

        private ref int[,] GetTilesRef(ETileMode tile_mode)
        {
            if (tile_mode == ETileMode.Design)
            {
                return ref m_LevelTilesDesign;
            }
            else
            {
                return ref m_LevelTilesMovement;
            }
        }

        private ref int[,] GetCurrentModeTilesRef()
        {
            if (m_TileMode == ETileMode.Design)
            {
                return ref m_LevelTilesDesign;
            }
            else
            {
                return ref m_LevelTilesMovement;
            }
        }

        public ref int[,] GetDesignTilesRef()
        {
            return ref m_LevelTilesDesign;
        }

        public ref int[,] GetMovementTilesRef()
        {
            return ref m_LevelTilesMovement;
        }

        public ref STileModeInfo GetTileModeInfoRef(ETileMode mode)
        {
            if (mode == ETileMode.Design)
            {
                return ref m_DesignTileInfo;
            }
            else
            {
                return ref m_MovementTileInfo;
            }
        }

        private int GetTileIDFromTileXY(ref STileModeInfo mode, SPosition tile_xy)
        {
            return tile_xy.Y * mode.TileSheetSizeInTileCount.Width + tile_xy.X;
        }

        private SPosition GetTileXYFromTileID(ref STileModeInfo mode, int tile_id)
        {
            if (mode.TileSheetSizeInTileCount.Width == 0)
            {
                return new SPosition(0, 0);
            }

            int y = tile_id / mode.TileSheetSizeInTileCount.Width;
            int x = tile_id - y * mode.TileSheetSizeInTileCount.Width;

            return new SPosition(x, y);
        }

        public void CreateLevel(int size_x, int size_y, STileModeInfo design_tile, STileModeInfo movement_tile)
        {
            System.Diagnostics.Debug.Assert(design_tile.TileSize == movement_tile.TileSize);

            m_HistoryStack.Clear();
            m_UnHistoryStack.Clear();
            m_HistoryStackLastGroup = 0;
            m_UnHistoryStackLastGroup = 0;

            m_DesignTileInfo = design_tile;
            m_MovementTileInfo = movement_tile;

            m_LevelSizeInTileCount.Width = size_x;
            m_LevelSizeInTileCount.Height = size_y;

            m_LevelTilesDesign = new int[size_x, size_y];
            m_LevelTilesMovement = new int[size_x, size_y];

            for (int x = 0; x < size_x; ++x)
            {
                for (int y = 0; y < size_y; ++y)
                {
                    m_LevelTilesDesign[x, y] = -1;
                    m_LevelTilesMovement[x, y] = 0;
                }
            }

            m_Textures.Clear();

            // 배경 텍스처
            AddTexture(CreateBlankTexture2D(size_x * design_tile.TileSize.Width, size_y * design_tile.TileSize.Height));
            m_Textures[0].BlendColor = new Color(0.0f, 0.1f, 0.3f, 1.0f);

            // 디자인 타일 텍스처
            AddTexture(m_DesignTileInfo.TileSheetFileName);

            // 움직임 타일 텍스처
            AddTexture(m_MovementTileInfo.TileSheetFileName);

            Invalidate();
        }

        public void UndoHistory(ref TabControl tab_control)
        {
            ref Stack<CHistory> this_history = ref m_HistoryStack;
            ref Stack<CHistory> that_history = ref m_UnHistoryStack;
            ref int this_history_group_id = ref m_HistoryStackLastGroup;
            ref int that_history_group_id = ref m_UnHistoryStackLastGroup;

            if (this_history.Count() > 0)
            {
                while (true)
                {
                    if (this_history.Count() == 0)
                    {
                        break;
                    }

                    CHistory last_history = this_history.Peek();
                    if (last_history.GroupID == this_history_group_id)
                    {
                        if (last_history.eAction == EHistoryAction.SetTile)
                        {
                            ref STileModeInfo tile_mode = ref GetTileModeInfoRef(last_history.CurrTileMode);

                            ref int[,] tiles = ref GetTilesRef(last_history.CurrTileMode);

                            tiles[last_history.IntData[0], last_history.IntData[1]] = last_history.IntData[2];
                        }
                        else if (last_history.eAction == EHistoryAction.ChangeTileMode)
                        {
                            m_TileMode = last_history.CurrTileMode;

                            int tab_index = 0;
                            if (m_TileMode == ETileMode.Movement)
                            {
                                tab_index = 1;
                            }

                            tab_control.SelectedIndex = tab_index;
                        }

                        that_history.Push(this_history.Pop());
                    }
                    else
                    {
                        break;
                    }
                }

                that_history_group_id = this_history_group_id;
                --this_history_group_id;

                Invalidate();
            }
        }

        public void RedoHistory(ref TabControl tab_control)
        {
            ref Stack<CHistory> this_history = ref m_UnHistoryStack;
            ref Stack<CHistory> that_history = ref m_HistoryStack;
            ref int this_history_group_id = ref m_UnHistoryStackLastGroup;
            ref int that_history_group_id = ref m_HistoryStackLastGroup;

            if (this_history.Count() > 0)
            {
                while (true)
                {
                    if (this_history.Count() == 0)
                    {
                        break;
                    }

                    CHistory last_history = this_history.Peek();
                    if (last_history.GroupID == this_history_group_id)
                    {
                        if (last_history.eAction == EHistoryAction.SetTile)
                        {
                            ref STileModeInfo tile_mode = ref GetTileModeInfoRef(last_history.CurrTileMode);

                            ref int[,] tiles = ref GetTilesRef(last_history.CurrTileMode);

                            tiles[last_history.IntData[0], last_history.IntData[1]] = last_history.IntData[3];
                        }
                        else if (last_history.eAction == EHistoryAction.ChangeTileMode)
                        {
                            m_TileMode = last_history.CurrTileMode;

                            int tab_index = 0;
                            if (m_TileMode == ETileMode.Movement)
                            {
                                tab_index = 1;
                            }

                            tab_control.SelectedIndex = tab_index;
                        }

                        that_history.Push(this_history.Pop());
                    }
                    else
                    {
                        break;
                    }
                }

                that_history_group_id = this_history_group_id;
                --this_history_group_id;

                Invalidate();
            }
        }
    }

    public class CHistory
    {
        public ETileMode CurrTileMode;
        public EHistoryAction eAction;
        public int GroupID;
        public int[] IntData = new int[5];
    }
}
