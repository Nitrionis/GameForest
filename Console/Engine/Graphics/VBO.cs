using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Graphics
{
	public sealed class VBO : IDisposable
	{
		private const int InvalidHandle = -1;

		public int handle { get; private set; } // Идентификатор VBO
		public BufferTarget target { get; private set; } // Тип VBO
		public BufferUsageHint usage { get; private set; }

		public VBO(BufferTarget target = BufferTarget.ArrayBuffer, BufferUsageHint usage = BufferUsageHint.DynamicDraw)
		{
			this.target = target;
			this.usage = usage;
			AcquireHandle();
		}

		// Создаёт новый VBO и сохраняет его идентификатор в свойство Handle
		private void AcquireHandle()
		{
			handle = GL.GenBuffer();
		}

		// Делает данный VBO текущим
		public void Bind()
		{
			GL.BindBuffer(target, handle);
		}

		// Заполняет VBO массивом data
		public void SetData<T>(T[] data) where T : struct
		{
			if (data.Length == 0)
				throw new ArgumentException("Массив должен содержать хотя бы один элемент", "data");

			Bind();
			GL.BufferData(target, (IntPtr)(data.Length * Marshal.SizeOf(typeof(T))), data, usage);
		}

		// Заполняет часть VBO массивом data
		public void SetSubData<T>(T[] data, int size, int offset = 0) where T : struct
		{
			Bind();
			GL.BufferSubData(target, (IntPtr)(offset * Marshal.SizeOf(typeof(T))), (IntPtr)(size * Marshal.SizeOf(typeof(T))), data);
		}

		// Освобождает занятые данным VBO ресурсы
		private void ReleaseHandle()
		{
			if (handle == InvalidHandle)
				return;

			GL.DeleteBuffer(handle);

			handle = InvalidHandle;
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