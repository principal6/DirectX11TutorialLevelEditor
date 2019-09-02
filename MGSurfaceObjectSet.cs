using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace DirectX11TutorialLevelEditor
{
    class MGSurfaceObjectSet : MGSurface
    {
        public Rectangle DrawingRectangle = new Rectangle(0, 0, 0, 0);

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Draw()
        {
            base.Draw();

            Editor.graphics.Clear(BackgroundColor);

            BeginDrawing();

            foreach (MGTextureData i in m_Textures)
            {
                Editor.spriteBatch.Draw(i.Texture,
                    new Rectangle(0, 0, DrawingRectangle.Width, DrawingRectangle.Height),
                    DrawingRectangle,
                    i.BlendColor * ((float)i.BlendColor.A / 255.0f));
            }

            EndDrawing();
        }
    }
}
