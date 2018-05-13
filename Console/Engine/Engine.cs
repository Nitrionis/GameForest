using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Console;
using Graphics;
using Scene;

namespace Game
{
	public class Engine : GameWindow
	{
		private Scene.Scene scene;

		public Engine() : base(
			800/*width*/, 600/*height*/,
			GraphicsMode.Default,
			"Ночной ДОЖОР",
			GameWindowFlags.Default,
			DisplayDevice.Default,
			4, 0, // unknow
			GraphicsContextFlags.ForwardCompatible)
		{
			Run(60/*FPS*/);
		}

		protected override void OnLoad(EventArgs e)
		{
			GL.ClearColor(Color.DimGray);
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			scene = new StartUiScene();
		}

		protected override void OnResize(EventArgs e)
		{
			GL.Viewport(0, 0, Width, Height);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			scene.Update();
			scene.Draw();

			SwapBuffers();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			scene.OnApplicationClosing();
		}
	}
}