using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;
using Microsoft.Xna.Framework;

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

        private SSize m_ScaledTileSize;
        private float m_ScaleFactor = 1.0f;

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
                m_Textures[0].Rect.X = -m_LevelBasePos.X * m_ScaledTileSize.Width;
                m_Textures[0].Rect.Y = -m_LevelBasePos.Y * m_ScaledTileSize.Height;
                m_Textures[0].Rect.Width = m_LevelSizeInTileCount.Width * m_ScaledTileSize.Width;
                m_Textures[0].Rect.Height = m_LevelSizeInTileCount.Height * m_ScaledTileSize.Height;

                Editor.spriteBatch.Draw(m_Textures[0].Texture,
                    m_Textures[0].Rect, new Rectangle(0, 0, 1, 1),
                    m_Textures[0].BlendColor);

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
                                offset_x * m_ScaledTileSize.Width, offset_y * m_ScaledTileSize.Height,
                                m_ScaledTileSize.Width, m_ScaledTileSize.Height);

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
                                    offset_x * m_ScaledTileSize.Width, offset_y * m_ScaledTileSize.Height,
                                    m_ScaledTileSize.Width, m_ScaledTileSize.Height);

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
                        m_MouseHoverPos.X * m_ScaledTileSize.Width,
                        m_MouseHoverPos.Y * m_ScaledTileSize.Height,
                        curr_mode.SelectionSizeInTileCount.Width * m_ScaledTileSize.Width,
                        curr_mode.SelectionSizeInTileCount.Height * m_ScaledTileSize.Height);

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
            m_MouseHoverPos.X = pos.X / m_ScaledTileSize.Width;
            m_MouseHoverPos.Y = pos.Y / m_ScaledTileSize.Height;

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

            int x_in_tiles = mouse_x / m_ScaledTileSize.Width;
            int y_in_tiles = mouse_y / m_ScaledTileSize.Height;

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
            m_ScaledTileSize = design_tile.TileSize;

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

            m_ScaledTileSize.Width = (int)(m_DesignTileInfo.TileSize.Width * m_ScaleFactor);
            m_ScaledTileSize.Height = (int)(m_DesignTileInfo.TileSize.Height * m_ScaleFactor);
        }

        public float GetScaleFactor()
        {
            return m_ScaleFactor;
        }

    }
}
