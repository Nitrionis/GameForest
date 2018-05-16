using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Graphics
{
	public sealed class Texture : IDisposable
	{
		private const int InvalidHandle = -1;

		public int Handle { get; private set; }

		private Bitmap bitmap;

		public Texture(string path)
		{
			Handle = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, Handle);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			bitmap = new Bitmap(path);

			BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

			bitmap.UnlockBits(data);
		}

		private void AcquireHandle()
		{
			Handle = GL.GenTexture();
		}

		public void Bind()
		{
			GL.BindTexture(TextureTarget.Texture2D, Handle);
		}

		public void Dispose()
		{
			ReleaseHandle();
			bitmap.Dispose();
			GC.SuppressFinalize(this);
		}

		private void ReleaseHandle()
		{
			if (Handle == InvalidHandle)
				return;

			GL.DeleteTexture(Handle);

			Handle = InvalidHandle;
		}

		~Texture()
		{
			// При вызове финализатора контекст OpenGL может уже не существовать и попытка выполнить GL.DeleteTexture приведёт к ошибке
			if (GraphicsContext.CurrentContext != null && !GraphicsContext.CurrentContext.IsDisposed)
				ReleaseHandle();
		}
	}
}