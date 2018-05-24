using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using static Game.GlobalReference;
using Console = System.Console;

namespace Game
{
	public class Window : GameWindow
	{
		public volatile bool sceneChanging = false;

		public Window() : base(
			SystemInformation.PrimaryMonitorMaximizedWindowSize.Height - 50,
			SystemInformation.PrimaryMonitorMaximizedWindowSize.Height - 50,
			GraphicsMode.Default,
			"Ночной ДОЖОР",
			GameWindowFlags.Default,
			DisplayDevice.Default,
			4, 0, // unknow
			GraphicsContextFlags.ForwardCompatible)
		{
			window = this;
			X = 0;
			Y = 0;
			VSync = VSyncMode.On;
			Run(60 /*FPS*/);
		}

		protected override void OnLoad(EventArgs e)
		{
			GL.ClearColor(new Color4(34, 34, 34, 255));
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			scene = new StartUiScene();

			/*fixedUpdateTimer = new Timer(100);
			fixedUpdateTimer.Elapsed += new ElapsedEventHandler(OnTimerTick);
			fixedUpdateTimer.Start();*/
		}

		protected override void OnResize(EventArgs e)
		{
			GL.Viewport(0, 0, Width, Height);
		}

		protected void OnTimerTick(object source, ElapsedEventArgs e)
		{
			if (!sceneChanging)
				scene.FixedUpdate();
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			if (!sceneChanging)
			{
				GL.Clear(ClearBufferMask.ColorBufferBit);

				scene.FixedUpdate();
				scene.Update();
				scene.Draw();

				SwapBuffers();
			}
		}

		private System.Object lockThis = new System.Object();

		public bool ChangeScene<T>() where T : Scene, new ()
		{
			lock (lockThis)
			{
				if (!sceneChanging && !(scene is T))
				{
					sceneChanging = true;
					System.Console.WriteLine("New Scene Created");
					scene = new T();
					sceneChanging = false;
					return true;
				}
			}
			return false;
		}

		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			base.OnMouseMove(e);
			cursorPos.X = e.X;
			cursorPos.Y = e.Y;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (fixedUpdateTimer != null)
				fixedUpdateTimer.Stop();
			scene.OnApplicationClosing();
		}
	}
}