using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Graphics
{
	public sealed class VBO : IDisposable
	{
		private const int InvalidHandle = -1;

		public int Handle { get; private set; } // Идентификатор VBO
		public BufferTarget Type { get; private set; } // Тип VBO

		public VBO(BufferTarget type = BufferTarget.ArrayBuffer)
		{
			Type = type;
			AcquireHandle();
		}

		// Создаёт новый VBO и сохраняет его идентификатор в свойство Handle
		private void AcquireHandle()
		{
			Handle = GL.GenBuffer();
		}

		// Делает данный VBO текущим
		public void Bind()
		{
			GL.BindBuffer(Type, Handle);
		}

		// Заполняет VBO массивом data
		public void SetData<T>(T[] data) where T : struct
		{
			if (data.Length == 0)
				throw new ArgumentException("Массив должен содержать хотя бы один элемент", "data");

			Bind();
			GL.BufferData(Type, (IntPtr)(data.Length * Marshal.SizeOf(typeof(T))), data, BufferUsageHint.StaticDraw);
		}

		// Освобождает занятые данным VBO ресурсы
		private void ReleaseHandle()
		{
			if (Handle == InvalidHandle)
				return;

			GL.DeleteBuffer(Handle);

			Handle = InvalidHandle;
		}

		public void Dispose()
		{
			ReleaseHandle();
			GC.SuppressFinalize(this);
		}

		~VBO()
		{
			// При вызове финализатора контекст OpenGL может уже не существовать и попытка выполнить GL.DeleteBuffer приведёт к ошибке
			if (GraphicsContext.CurrentContext != null && !GraphicsContext.CurrentContext.IsDisposed)
				ReleaseHandle();
		}
	}
}