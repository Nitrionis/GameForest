using System;
using System.ComponentModel;
using System.Timers;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
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
			GlobalReference.window = this;
			Run(60/*FPS*/);
		}

		protected override void OnLoad(EventArgs e)
		{
			GL.ClearColor(new Color4(34, 34, 34, 255));
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			GlobalReference.scene = new StartUiScene();

			/*GlobalReference.fixedUpdateTimer = new Timer(100);
			GlobalReference.fixedUpdateTimer.Elapsed += new ElapsedEventHandler(OnTimerTick);
			GlobalReference.fixedUpdateTimer.Start();*/
		}

		protected override void OnResize(EventArgs e)
		{
			GL.Viewport(0, 0, Width, Height);
		}

		protected void OnTimerTick(object source, ElapsedEventArgs e)
		{
			GlobalReference.scene.FixedUpdate();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			GlobalReference.scene.FixedUpdate();
			GlobalReference.scene.Update();
			GlobalReference.scene.Draw();

			SwapBuffers();
		}

		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			base.OnMouseMove(e);
			GlobalReference.cursorPos.X = e.X;
			GlobalReference.cursorPos.Y = e.Y;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			//GlobalReference.fixedUpdateTimer.Stop();
			GlobalReference.scene.OnApplicationClosing();
		}
	}
}