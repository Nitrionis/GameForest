using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Console;
using Graphics;
using OpenTK.Input;
using Scene;

namespace Game
{
	public class Window : GameWindow
	{
		public Window() : base(
			800/*width*/, 600/*height*/,
			GraphicsMode.Default,
			"Ночной ДОЖОР",
			GameWindowFlags.Default,
			DisplayDevice.Default,
			4, 0, // unknow
			GraphicsContextFlags.ForwardCompatible)
		{
			GlobalRef.window = this;
			Run(60/*FPS*/);
		}

		protected override void OnLoad(EventArgs e)
		{
			GL.ClearColor(new Color4(34, 34, 34, 255));
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GlobalRef.scene = new StartUiScene();

			GlobalRef.fixedUpdateTimer = new Timer(300);
			GlobalRef.fixedUpdateTimer.Elapsed += new ElapsedEventHandler(OnTimerTick);
			GlobalRef.fixedUpdateTimer.Start();
		}

		private void OnTimerTick(object source, ElapsedEventArgs e)
		{
			GlobalRef.scene.FixedUpdate();
		}

		protected override void OnResize(EventArgs e)
		{
			GL.Viewport(0, 0, Width, Height);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			GlobalRef.scene.Update();
			GlobalRef.scene.Draw();

			SwapBuffers();
		}

		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			base.OnMouseMove(e);
			GlobalRef.cursorPos.X = e.X;
			GlobalRef.cursorPos.Y = e.Y;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			GlobalRef.fixedUpdateTimer.Stop();
			GlobalRef.scene.OnApplicationClosing();
		}
	}
}