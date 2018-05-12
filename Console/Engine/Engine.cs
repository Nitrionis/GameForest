using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Console;
using Graphics;

namespace Game
{
	public class Engine : GameWindow
	{
		private Matrix4 matrix;

		private VBO meshVbo;
		private VAO meshVao;

		private ShaderProgram shaderProgram;

		private Bitmap bitmap = new Bitmap("D:/pdf_sit/GameForest/Console/resources/atlas.jpg");
		int texture;

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
			matrix = Matrix4.Identity;

			shaderProgram.Use();

			int matHandle = shaderProgram.GetUniformLocation("coeff");
			GL.UniformMatrix4(matHandle, false, ref matrix);

			GL.BindTexture(TextureTarget.Texture2D, texture);

			meshVao.Draw();


			// Переключаем задний и передний буферы
			SwapBuffers();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			meshVao.Dispose();
			shaderProgram.Dispose();
			GL.DeleteTextures(1, ref texture);
		}

		private void BuildFrameData()
		{
			GL.GenTextures(1, out texture);
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
				ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
				OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

			bitmap.UnlockBits(data);

			meshVbo = new VBO();
			meshVbo.SetData(new[] {
				new Vertex( 0.0f,  0.5f, Color4.Red, 0.0f, 0.0f),
				new Vertex(-0.5f, -0.5f, Color4.Green, 0.0f, 1.0f),
				new Vertex( 0.5f, -0.5f, Color4.Blue, 1.0f, 1.0f)
			});
			meshVao = new VAO(3); // 3 вершины
			// Нулевой атрибут вершины – позиция, у неё 2 компонента типа float
			meshVao.AttachVBO(0, meshVbo, 2, VertexAttribPointerType.Float, 7 * sizeof(float), 0);
			// Первый атрибут вершины – цвет, у него 3 компонента типа float
			meshVao.AttachVBO(1, meshVbo, 3, VertexAttribPointerType.Float, 7 * sizeof(float), 2 * sizeof(float));
			// Второй атрибут вершины – цвет, у него 3 компонента типа float
			meshVao.AttachVBO(2, meshVbo, 2, VertexAttribPointerType.Float, 7 * sizeof(float), 5 * sizeof(float));

			shaderProgram = new ShaderProgram();

			using (var vertexShader = new Shader(ShaderType.VertexShader))
			using (var fragmentShader = new Shader(ShaderType.FragmentShader))
			{
				vertexShader.Compile(@"
					#version 400

					layout(location = 0) in vec2 Position;
					layout(location = 1) in vec3 Color;
					layout(location = 2) in vec2 UV;

 					uniform mat4 coeff;

					layout(location = 0) out vec2 outUV;

					void main() {
    					gl_Position = vec4(Position, 0.0, 1.0);
    					//fragColor = Color * coeff[0][0];
						outUV = UV;
					}
					");
				fragmentShader.Compile(@"
					#version 400

					uniform sampler2D tex;

					layout(location = 0) in vec2 UV;

					layout(location = 0) out vec4 outColor;

					void main() {
    					outColor = texture(tex, UV);
					}
					");
				shaderProgram.AttachShader(vertexShader);
				shaderProgram.AttachShader(fragmentShader);
				shaderProgram.Link();
			}
		}
	}
}