using System;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Graphics
{
	public sealed class VAO : IDisposable
	{
		private const int InvalidHandle = -1;

		public int Handle { get; private set; }
		public int VertexCount { get; private set; } // Число вершин для отрисовки

		public VAO(int vertexCount)
		{
			VertexCount = vertexCount;
			AcquireHandle();
		}

		private void AcquireHandle()
		{
			Handle = GL.GenVertexArray();
		}

		public void Use()
		{
			GL.BindVertexArray(Handle);
		}

		public void AttachVBO(int index, VBO vbo, int elementsPerVertex, VertexAttribPointerType pointerType, int stride, int offset)
		{
			Use();
			vbo.Bind();
			GL.EnableVertexAttribArray(index);
			GL.VertexAttribPointer(index, elementsPerVertex, pointerType, false, stride, offset);
		}

		public void Draw()
		{
			Use();
			GL.DrawArrays(PrimitiveType.Triangles, 0, VertexCount);
		}

		private void ReleaseHandle()
		{
			if (Handle == InvalidHandle)
				return;

			GL.DeleteVertexArray(Handle);

			Handle = InvalidHandle;
		}

		public void Dispose()
		{
			ReleaseHandle();
			GC.SuppressFinalize(this);
		}

		~VAO()
		{
			if (GraphicsContext.CurrentContext != null && !GraphicsContext.CurrentContext.IsDisposed)
				ReleaseHandle();
		}
	}
}