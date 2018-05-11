using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Graphics
{
	public sealed class Shader : IDisposable
	{
		private const int InvalidHandle = -1;

		public int Handle { get; private set; }
		public ShaderType Type { get; private set; }

		public Shader(ShaderType type)
		{
			Type = type;
			AcquireHandle();
		}

		private void AcquireHandle()
		{
			Handle = GL.CreateShader(Type);
		}

		public void Compile(string source)
		{
			GL.ShaderSource(Handle, source);
			GL.CompileShader(Handle);

			int compileStatus;
			GL.GetShader(Handle, ShaderParameter.CompileStatus, out compileStatus);

			// Если произошла ошибка, выведем сообщение
			if (compileStatus == 0)
				System.Console.WriteLine(GL.GetShaderInfoLog(Handle));
		}

		private void ReleaseHandle()
		{
			if (Handle == InvalidHandle)
				return;

			GL.DeleteShader(Handle);

			Handle = InvalidHandle;
		}

		public void Dispose()
		{
			ReleaseHandle();
			GC.SuppressFinalize(this);
		}

		~Shader()
		{
			if (GraphicsContext.CurrentContext != null && !GraphicsContext.CurrentContext.IsDisposed)
				ReleaseHandle();
		}
	}
}