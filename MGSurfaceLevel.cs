using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ObjectSetData = DirectX11TutorialObjectEditor.ObjectSetData;

namespace DirectX11TutorialLevelEditor
{
    public enum EHistoryAction
    {
        SetTile, // IntData = { X, Y, OldTileID, NewTileID }
        ChangeTileMode,
    }

    public class CHistory
    {
        public ETileMode CurrTileMode;
        public EHistoryAction eAction;
        public int GroupID;
        public int[] IntData = new int[5];
    }

    public class CInsertedObject
    {
        public int ObjectID;
        public SPosition Position;
    }

    public class MGSurfaceLevel : MGSurface
    {
        private int[,] m_LevelTilesDesign;
        private int[,] m_LevelTilesMovement;
        private Color OverlayColor;
        private SSize m_LevelSizeInTileCount;

        private SPosition m_MouseHoverPosInTiles;
        private SPosition m_MouseHoverPosPhysical;
        private SPosition m_LevelBasePos;

        private ETileMode m_TileMode;
        private STileModeInfo m_DesignTileInfo;
        private STileModeInfo m_MovementTileInfo;

        private Stack<CHistory> m_HistoryStack = new Stack<CHistory>();
        private Stack<CHistory> m_UnHistoryStack = new Stack<CHistory>();
        private int m_HistoryStackLastGroup = 0;
        
        private float m_ScaleFactor = 1.0f;

        public bool ShouldDrawTileOverlay = true;
        private Texture2D m_ObjectSetTexture;
        private Texture2D m_ObjectBorderTexture;
        public List<CInsertedObject> InsertedObjects = new List<CInsertedObject>();
        private ObjectSetData m_ObjectSetData;
        public int SelectedInsertedObjectIndex = -1;

        public Rectangle ObjectOverlayRectangle { set; get; } = new Rectangle(0, 0, 0, 0);

        protected override void Initialize()
        {
            base.Initialize();

            BackgroundColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);

            OverlayColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        }

        protected override void Draw()
        {
            base.Draw();

            BeginDrawing();

            ref STileModeInfo curr_mode = ref GetCurrentTileModeInfoRef();
            int texture_index = GetCurrentTileModeTextureIndex();

            if (m_LevelTilesDesign != null)
            {
                // 배경 그리기
                m_Textures[0].Rect.X = 
                    (int)(-m_LevelBasePos.X * m_DesignTileInfo.TileSize.Width * m_ScaleFactor);
                m_Textures[0].Rect.Y = 
                    (int)(-m_LevelBasePos.Y * m_DesignTileInfo.TileSize.Width * m_ScaleFactor);
                m_Textures[0].Rect.Width = 
                    (int)(m_LevelSizeInTileCount.Width * m_DesignTileInfo.TileSize.Width * m_ScaleFactor);
                m_Textures[0].Rect.Height =
                    (int)(m_LevelSizeInTileCount.Height * m_DesignTileInfo.TileSize.Width * m_ScaleFactor);

                Editor.spriteBatch.Draw(m_Textures[0].Texture,
                    m_Textures[0].Rect, new Rectangle(0, 0, 1, 1),
                    m_Textures[0].BlendColor);

                // 등록된 오브젝트들 그리기
                if (m_ObjectSetTexture != null)
                {
                    Rectangle rect_src;
                    Rectangle rect_dest;

                    foreach (CInsertedObject inserted in InsertedObjects)
                    {
                        rect_src = new Rectangle(
                            m_ObjectSetData.Elements[inserted.ObjectID].OffsetU,
                            m_ObjectSetData.Elements[inserted.ObjectID].OffsetV,
                            m_ObjectSetData.Elements[inserted.ObjectID].Size.Width,
                            m_ObjectSetData.Elements[inserted.ObjectID].Size.Height);

                        SPosition pos = new SPosition(
                            (int)(inserted.Position.X * m_ScaleFactor),
                            (int)(inserted.Position.Y * m_ScaleFactor)
                            );

                        rect_dest = new Rectangle(
                        pos.X - (int)(m_LevelBasePos.X * m_DesignTileInfo.TileSize.Width * m_ScaleFactor),
                        pos.Y - (int)(m_LevelBasePos.Y * m_DesignTileInfo.TileSize.Height * m_ScaleFactor),
                        (int)(m_ObjectSetData.Elements[inserted.ObjectID].Size.Width * m_ScaleFactor),
                        (int)(m_ObjectSetData.Elements[inserted.ObjectID].Size.Height * m_ScaleFactor));

                        Editor.spriteBatch.Draw(m_ObjectSetTexture, rect_dest, rect_src, Color.White);
                    }
                }

                // 디자인 타일 그리기
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
                                (int)(offset_x * m_DesignTileInfo.TileSize.Width * m_ScaleFactor),
                                (int)(offset_y * m_DesignTileInfo.TileSize.Height * m_ScaleFactor),
                                (int)(m_DesignTileInfo.TileSize.Width * (m_ScaleFactor + 0.02f)), 
                                (int)(m_DesignTileInfo.TileSize.Width * (m_ScaleFactor + 0.02f)));

                        Rectangle rect_src = new Rectangle(
                                tile_xy.X * m_DesignTileInfo.TileSize.Width, tile_xy.Y * m_DesignTileInfo.TileSize.Height,
                                m_DesignTileInfo.TileSize.Width, m_DesignTileInfo.TileSize.Height);

                        Editor.spriteBatch.Draw(m_Textures[1].Texture,
                            rect_dest, rect_src, m_Textures[1].BlendColor * blend_factor);
                    }
                }

                // 움직임 타일 그리기
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
                                (int)(offset_x * m_DesignTileInfo.TileSize.Width * m_ScaleFactor),
                                (int)(offset_y * m_DesignTileInfo.TileSize.Height * m_ScaleFactor),
                                (int)(m_DesignTileInfo.TileSize.Width * (m_ScaleFactor + 0.02f)),
                                (int)(m_DesignTileInfo.TileSize.Width * (m_ScaleFactor + 0.02f)));

                            Rectangle rect_src = new Rectangle(
                                    tile_xy.X * FixedMovementTileSize.Width, tile_xy.Y * FixedMovementTileSize.Height,
                                    FixedMovementTileSize.Width, FixedMovementTileSize.Height);

                            Editor.spriteBatch.Draw(m_Textures[2].Texture,
                                rect_dest, rect_src, m_Textures[2].BlendColor * 0.6f);
                        }
                    }
                }
            }

            // 선택된 타일 오버레이 그리기
            if (ShouldDrawTileOverlay == true)
            {
                SPosition tile_xy = curr_mode.SelectionOrigin;

                Rectangle rect_dest = new Rectangle(
                        (int)(m_MouseHoverPosInTiles.X * m_DesignTileInfo.TileSize.Width * m_ScaleFactor),
                        (int)(m_MouseHoverPosInTiles.Y * m_DesignTileInfo.TileSize.Height * m_ScaleFactor),
                        (int)(curr_mode.SelectionSizeInTileCount.Width * m_DesignTileInfo.TileSize.Width * m_ScaleFactor),
                        (int)(curr_mode.SelectionSizeInTileCount.Height * m_DesignTileInfo.TileSize.Height * m_ScaleFactor));

                Rectangle rect_src = new Rectangle(
                        tile_xy.X * curr_mode.TileSize.Width,
                        tile_xy.Y * curr_mode.TileSize.Height,
                        curr_mode.SelectionSizeInTileCount.Width * curr_mode.TileSize.Width,
                        curr_mode.SelectionSizeInTileCount.Height * curr_mode.TileSize.Height);

                if (m_TileMode == ETileMode.Movement)
                {
                    rect_src.X = tile_xy.X * FixedMovementTileSize.Width;
                    rect_src.Y = tile_xy.Y * FixedMovementTileSize.Height;
                    rect_src.Width = curr_mode.SelectionSizeInTileCount.Width * FixedMovementTileSize.Width;
                    rect_src.Height = curr_mode.SelectionSizeInTileCount.Height * FixedMovementTileSize.Height;
                }

                Editor.spriteBatch.Draw(m_Textures[texture_index].Texture,
                rect_dest, rect_src, OverlayColor);
            }
            else
            {
                if (m_ObjectSetTexture != null)
                {
                    Rectangle rect_dest;
                    Rectangle rect_src;

                    // 선택된 등록 오브젝트 테두리 그리기
                    if (SelectedInsertedObjectIndex != -1)
                    {
                        CInsertedObject inserted = InsertedObjects[SelectedInsertedObjectIndex];
                        Color color = new Color(1.0f, 0.0f, 0.0f);

                        rect_src = new Rectangle(0, 0, 1, 1);

                        SPosition pos = inserted.Position;
                        pos.X = (int)(pos.X * m_ScaleFactor);
                        pos.Y = (int)(pos.Y * m_ScaleFactor);
                        pos.X -= (int)(m_LevelBasePos.X * m_DesignTileInfo.TileSize.Width * m_ScaleFactor);
                        pos.Y -= (int)(m_LevelBasePos.Y * m_DesignTileInfo.TileSize.Height * m_ScaleFactor);

                        System.Drawing.Size size = m_ObjectSetData.Elements[inserted.ObjectID].Size;
                        size.Width = (int)(size.Width * m_ScaleFactor);
                        size.Height = (int)(size.Height * m_ScaleFactor);

                        rect_dest = new Rectangle(
                        pos.X,
                        pos.Y,
                        size.Width,
                        1);
                        Editor.spriteBatch.Draw(m_ObjectBorderTexture, rect_dest, rect_src, color);

                        rect_dest = new Rectangle(
                        pos.X,
                        pos.Y + size.Height,
                        size.Width,
                        1);
                        Editor.spriteBatch.Draw(m_ObjectBorderTexture, rect_dest, rect_src, color);

                        rect_dest = new Rectangle(
                        pos.X,
                        pos.Y,
                        1,
                        size.Height);
                        Editor.spriteBatch.Draw(m_ObjectBorderTexture, rect_dest, rect_src, color);

                        rect_dest = new Rectangle(
                        pos.X + size.Width,
                        pos.Y,
                        1,
                        size.Height);
                        Editor.spriteBatch.Draw(m_ObjectBorderTexture, rect_dest, rect_src, color);
                    }

                    
                    // 선택된 오브젝트 오버레이
                    rect_dest = new Rectangle(
                        m_MouseHoverPosPhysical.X - (int)(ObjectOverlayRectangle.Width / 2 * m_ScaleFactor),
                        m_MouseHoverPosPhysical.Y - (int)(ObjectOverlayRectangle.Height / 2 * m_ScaleFactor),
                        (int)(ObjectOverlayRectangle.Width * m_ScaleFactor),
                        (int)(ObjectOverlayRectangle.Height * m_ScaleFactor));

                    Editor.spriteBatch.Draw(m_ObjectSetTexture, rect_dest, ObjectOverlayRectangle, OverlayColor);
                }
            }

            EndDrawing();
        }

        public void SetObjectSet(ObjectSetData ObjectSet)
        {
            m_ObjectSetData = ObjectSet;

            m_ObjectSetTexture = 
                Texture2D.FromStream(Editor.graphics, File.OpenRead(m_AssetDir + m_ObjectSetData.TextureFileName));
        }

        public void UpdateTileMode(ETileMode mode)
        {
            if (m_TileMode == mode)
            {
                return;
            }

            ++m_HistoryStackLastGroup;

            CHistory new_history = new CHistory
            {
                eAction = EHistoryAction.ChangeTileMode,
                GroupID = m_HistoryStackLastGroup,
                CurrTileMode = m_TileMode
            };
            m_HistoryStack.Push(new_history);

            m_TileMode = mode;

            Invalidate();
        }

        public void UpdateMouseHoverPos(SPosition pos)
        {
            m_MouseHoverPosPhysical = pos;

            m_MouseHoverPosInTiles.X = (int)(pos.X / (m_DesignTileInfo.TileSize.Width * m_ScaleFactor));
            m_MouseHoverPosInTiles.Y = (int)(pos.Y / (m_DesignTileInfo.TileSize.Height * m_ScaleFactor));

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

        public SPosition GetLevelBasePos()
        {
            return m_LevelBasePos;
        }

        public SPosition GetDesignTileScaledLevelBasePos()
        {
            return new SPosition(m_LevelBasePos.X * m_DesignTileInfo.TileSize.Width,
                m_LevelBasePos.Y * m_DesignTileInfo.TileSize.Height);
        }

        public void SetLevelTile(int mouse_x, int mouse_y, bool should_erase = false)
        {
            if (m_TileMode == ETileMode.Object) return;

            bool is_set_new_tile = false;

            ref STileModeInfo tile_mode = ref GetCurrentTileModeInfoRef();
            ref int[,] tiles = ref GetCurrentModeTilesRef();

            int x_in_tiles = (int)(mouse_x / (m_DesignTileInfo.TileSize.Width * m_ScaleFactor));
            int y_in_tiles = (int)(mouse_y / (m_DesignTileInfo.TileSize.Height * m_ScaleFactor));

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

                            CHistory new_history = new CHistory
                            {
                                GroupID = m_HistoryStackLastGroup,
                                eAction = EHistoryAction.SetTile,
                                CurrTileMode = m_TileMode
                            };
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
            AddTexture(CreateBlankTexture2D());
            m_Textures[0].BlendColor = new Color(0.0f, 0.1f, 0.3f, 1.0f);

            // 디자인 타일 텍스처
            AddTexture(m_DesignTileInfo.TileSheetFileName);

            // 움직임 타일 텍스처
            AddTexture(m_MovementTileInfo.TileSheetFileName);

            // 오브젝트 테두리 텍스처
            m_ObjectBorderTexture = CreateBlankTexture2D();

            Invalidate();
        }

        public void UndoHistory(ref TabControl tab_control)
        {
            ref Stack<CHistory> this_history = ref m_HistoryStack;
            ref Stack<CHistory> that_history = ref m_UnHistoryStack;

            if (this_history.Count() > 0)
            {
                int captured_group_id = this_history.Peek().GroupID;

                while (true)
                {
                    if (this_history.Count() == 0)
                    {
                        break;
                    }

                    CHistory last_history = this_history.Peek();
                    if (last_history.GroupID == captured_group_id)
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

                --m_HistoryStackLastGroup;

                Invalidate();
            }
        }

        public void RedoHistory(ref TabControl tab_control)
        {
            ref Stack<CHistory> this_history = ref m_UnHistoryStack;
            ref Stack<CHistory> that_history = ref m_HistoryStack;

            if (this_history.Count() > 0)
            {
                int captured_group_id = this_history.Peek().GroupID;

                while (true)
                {
                    if (this_history.Count() == 0)
                    {
                        break;
                    }

                    CHistory last_history = this_history.Peek();
                    if (last_history.GroupID == captured_group_id)
                    {
                        if (last_history.eAction == EHistoryAction.SetTile)
                        {
                            ref STileModeInfo tile_mode = ref GetTileModeInfoRef(last_history.CurrTileMode);

                            ref int[,] tiles = ref GetTilesRef(last_history.CurrTileMode);

                            tiles[last_history.IntData[0], last_history.IntData[1]] = last_history.IntData[3];
                        }
                        else if (last_history.eAction == EHistoryAction.ChangeTileMode)
                        {
                            switch (last_history.CurrTileMode)
                            {
                                case ETileMode.Design:
                                    m_TileMode = ETileMode.Movement;
                                    break;
                                case ETileMode.Movement:
                                    m_TileMode = ETileMode.Design;
                                    break;
                            }

                            int tab_index = 1;
                            if (m_TileMode == ETileMode.Movement)
                            {
                                tab_index = 0;
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

                ++m_HistoryStackLastGroup;

                Invalidate();
            }
        }

        public void SetScaleFactor(float Factor)
        {
            m_ScaleFactor = Factor;
        }

        public float GetScaleFactor()
        {
            return m_ScaleFactor;
        }

    }
}
