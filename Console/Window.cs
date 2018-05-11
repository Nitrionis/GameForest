using System;
using System.ComponentModel;
using Console;
using Graphics;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Game
{
	public class Window : GameWindow
	{
		private VBO meshVbo;
		private VAO meshVao;

		private ShaderProgram shaderProgram;

		public Window() : base(
			800, 600,
			GraphicsMode.Default,
			"OpenGL Tutorial",
			GameWindowFlags.Default,
			DisplayDevice.Default,
			4, 0,
			GraphicsContextFlags.ForwardCompatible)
		{
			Run(60);
		}

		// Вызывается при первоначальной загрузке
		protected override void OnLoad(EventArgs e)
		{
			GL.ClearColor(Color4.Black); // Зададим цвет очистки окна
			BuildFrameData();
		}

		// Вызывается при изменение размеров окна
		protected override void OnResize(EventArgs e)
		{
			GL.Viewport(0, 0, Width, Height); // Зададим область перерисовки размером со всё окно
		}

		// Вызывается при отрисовке очередного кадра
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit); // Очищаем буфер цвета
			// Тут будет распологаться основной код отрисовки
			shaderProgram.Use();
			meshVao.Draw();
			// Переключаем задний и передний буферы
			SwapBuffers();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			meshVao.Dispose();
			shaderProgram.Dispose();
		}

		private void BuildFrameData()
		{
			meshVbo = new VBO();
			meshVbo.SetData(new[] {
				new Vertex( 0.0f,  0.5f, Color4.Red),
				new Vertex(-0.5f, -0.5f, Color4.Green),
				new Vertex( 0.5f, -0.5f, Color4.Blue)
			});
			meshVao = new VAO(3); // 3 вершины
			// Нулевой атрибут вершины – позиция, у неё 2 компонента типа float
			meshVao.AttachVBO(0, meshVbo, 2, VertexAttribPointerType.Float, 5 * sizeof(float), 0);
			// Первый атрибут вершины – цвет, у него 3 компонента типа float
			meshVao.AttachVBO(1, meshVbo, 3, VertexAttribPointerType.Float, 5 * sizeof(float), 2 * sizeof(float));

			shaderProgram = new ShaderProgram();

			using (var vertexShader = new Shader(ShaderType.VertexShader))
			using (var fragmentShader = new Shader(ShaderType.FragmentShader))
			{
				System.Console.WriteLine("vertexShader.Compile");
				vertexShader.Compile(@"
					#version 400

					layout(location = 0) in vec2 Position;
					layout(location = 1) in vec3 Color;

					out vec3 fragColor;

					void main()
					{
    					gl_Position = vec4(Position, 0.0, 1.0);
    					fragColor = Color;
					}
					");
				fragmentShader.Compile(@"
					#version 400

					in vec3 fragColor;

					layout(location = 0) out vec4 outColor;

					void main()
					{
    					outColor = vec4(fragColor, 1.0);
					}
					");
				System.Console.WriteLine("shaderProgram.AttachShader(vertexShader);");
				shaderProgram.AttachShader(vertexShader);
				shaderProgram.AttachShader(fragmentShader);
				shaderProgram.Link();
			}
		}
	}
}