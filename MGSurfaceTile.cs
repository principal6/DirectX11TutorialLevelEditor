using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DirectX11TutorialLevelEditor
{
    public class MGSurfaceTile : MGSurface
    {
        private ETileMode m_TileMode;
        private STileModeInfo m_DesignTileInfo;
        private STileModeInfo m_MovementTileInfo;

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
                Rectangle rect_src = new Rectangle(0, 0, 1, 1);
                Rectangle rect_dest = m_Textures[2].Rect;
                rect_dest.Width = tile_mode.TileSize.Width;
                rect_dest.Height = tile_mode.TileSize.Height;

                for (int x = 0; x < tile_mode.SelectionSizeInTileCount.Width; ++x)
                {
                    for (int y = 0; y < tile_mode.SelectionSizeInTileCount.Height; ++y)
                    {
                        rect_dest.X = m_Textures[2].Rect.X + x * tile_mode.TileSize.Width;
                        rect_dest.Y = m_Textures[2].Rect.Y + y * tile_mode.TileSize.Height;

                        Editor.spriteBatch.Draw(m_Textures[2].Texture,
                            rect_dest, rect_src,
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
            AddTexture(CreateBlankTexture2D());

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

}
