using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGameTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            var scaleFactor = 4.0;
            var textureFilename = "tiles.png";
            var xPosition = 0;
            var yPosition = 0;
            var xPositionChange = 0;
            var yPositionChange = 0;

            var window = new GameWindow();
            window.Load += (s, a) =>
            {
                GL.Enable(EnableCap.Texture2D);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                var texturId = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texturId);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

                var texturbild = new Bitmap(textureFilename);
                var texturyta = new Rectangle(0, 0, texturbild.Width, texturbild.Height);
                var data = texturbild.LockBits(texturyta, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                texturbild.UnlockBits(data);

                GL.MatrixMode(MatrixMode.Texture);
                GL.LoadIdentity();
                GL.Scale(1.0 / texturyta.Width, 1.0 / texturyta.Height, 1.0);
                GL.MatrixMode(MatrixMode.Modelview);
            };

            window.Resize += (s, a) =>
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0.0, window.Width / scaleFactor, 0.0, window.Height / scaleFactor, 0.0, 4.0);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.Viewport(0, 0, window.Width, window.Height);
            };

            window.RenderFrame += (s, a) =>
            {
                GL.Clear(ClearBufferMask.ColorBufferBit);
                CopyTextureAreaToScreenBuffer(xPosition, yPosition, 0, 0, 16, 16);
                window.SwapBuffers();
            };

            window.UpdateFrame += (s, a) =>
            {
                xPosition += xPositionChange;
                yPosition += yPositionChange;
            };

            window.KeyDown += (s, a) =>
            {
                switch (a.Key)
                {
                    case OpenTK.Input.Key.Escape:
                        if (!window.IsExiting) window.Exit();
                        break;
                    case OpenTK.Input.Key.Up:
                        yPositionChange = 1;
                        break;
                    case OpenTK.Input.Key.Down:
                        yPositionChange = -1;
                        break;
                    case OpenTK.Input.Key.Right:
                        xPositionChange = 1;
                        break;
                    case OpenTK.Input.Key.Left:
                        xPositionChange = -1;
                        break;
                }
            };

            window.KeyUp += (s, a) =>
            {
                switch(a.Key)
                {
                    case OpenTK.Input.Key.Up:
                        yPositionChange = 0;
                        break;
                    case OpenTK.Input.Key.Down:
                        yPositionChange = 0;
                        break;
                    case OpenTK.Input.Key.Right:
                        xPositionChange = 0;
                        break;
                    case OpenTK.Input.Key.Left:
                        xPositionChange = 0;
                        break;
                }
            };

            window.MouseDown += (s, a) =>
            {
                if(a.Button == OpenTK.Input.MouseButton.Left)
                {
                    xPosition = a.Position.X / (int)scaleFactor - 8;
                    yPosition = (window.Height - a.Position.Y) / (int)scaleFactor - 8;
                }
            };

            window.Run();
        }

        static void CopyTextureAreaToScreenBuffer(int skärmX, int skärmY, int texturX, int texturY, int bredd, int höjd)
        {
            var skärmX2 = skärmX + bredd;
            var skärmY2 = skärmY + höjd;
            var texturX2 = texturX + bredd;
            var texturY2 = texturY + höjd;

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(texturX, texturY2);
            GL.Vertex2(skärmX, skärmY);

            GL.TexCoord2(texturX2, texturY2);
            GL.Vertex2(skärmX2, skärmY);

            GL.TexCoord2(texturX2, texturY);
            GL.Vertex2(skärmX2, skärmY2);

            GL.TexCoord2(texturX, texturY);
            GL.Vertex2(skärmX, skärmY2);
            GL.End();
        }
    }
}
