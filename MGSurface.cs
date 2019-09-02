using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DirectX11TutorialLevelEditor
{
    public class MGTextureData
    {
        public Texture2D Texture;
        public Rectangle Rect;
        public Color BlendColor = Color.White;
    }

    public class MGSurface : MonoGame.Forms.Controls.InvalidationControl
    {
        protected string m_AssetDir;
        protected List<MGTextureData> m_Textures = new List<MGTextureData>();
        public Color BackgroundColor = Color.White;
        public SSize FixedMovementTileSize;

        ~MGSurface()
        {
            m_Textures.Clear();
            Dispose();
        }

        public void SetAssetDir(string AssetDir)
        {
            m_AssetDir = AssetDir;
        }

        public Texture2D CreateBlankTexture2D()
        {
            Texture2D blank_texture = new Texture2D(Editor.graphics, 1, 1);

            Color[] data = new Color[1];

            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = Color.White;
            }

            blank_texture.SetData(data);

            return blank_texture;
        }

        public void AddTexture(string TextureFileName)
        {
            m_Textures.Add(new MGTextureData());

            Texture2D texture = Texture2D.FromStream(Editor.graphics, File.OpenRead(m_AssetDir + TextureFileName));

            m_Textures.ElementAt(m_Textures.Count - 1).Texture = texture;
            m_Textures.ElementAt(m_Textures.Count - 1).Rect = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public void AddTexture(Texture2D Texture)
        {
            m_Textures.Add(new MGTextureData());

            m_Textures.ElementAt(m_Textures.Count - 1).Texture = Texture;
            m_Textures.ElementAt(m_Textures.Count - 1).Rect = new Rectangle(0, 0, Texture.Width, Texture.Height);
        }

        public void ClearTextures()
        {
            m_Textures.Clear();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Draw()
        {
            base.Draw();

            Editor.graphics.Clear(BackgroundColor);
        }

        protected void BeginDrawing()
        {
            Editor.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
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
}
