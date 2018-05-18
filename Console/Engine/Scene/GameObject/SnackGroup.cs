using System;
using System.Runtime.InteropServices;
using Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Scene
{
	public class SnackGroup : GameObject
	{
		private Game.Scene scene;

		public Texture texture;
		public VBO vbo;
		public VAO vao;
		public ShaderProgram shaderProgram;

		public TexturedRectangle[,] snacksTexturedRects = new TexturedRectangle[8,8];
		public Button[,] snacksButtons = new Button[8,8];

		public SnackGroup(Game.Scene scene, uint sizeX, uint sizeY)
		{
			this.scene = scene;
			shaderProgram = TexturedRectangle.GetShaderProgram();
			CreateMesh();
		}

		class ButtonCheker : IButtonAction
		{
			private TexturedRectangle texturedRectangle;

			public ButtonCheker(TexturedRectangle texturedRectangle)
			{
				this.texturedRectangle = texturedRectangle;
			}

			public void Event(int state)
			{
				texturedRectangle.offsetU = 0.125f * state;
				texturedRectangle.updateFlaf = true;
				System.Console.WriteLine("MainScene Event(int state)");
			}
		}

		public void CreateMesh()
		{
			vao = new VAO(6 * 8 * 8);
			vao.AttachVBO(0, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 0);
			vao.AttachVBO(1, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
			vao.PrimitiveType = PrimitiveType.Triangles;

			vbo = new VBO();

			Random random = new Random();

			TexturedRectangle.Vertex[] data = new TexturedRectangle.Vertex[6 * 8 * 8];

			for (int y = 0; y < 8; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					float startX = x * 0.2f - 1, startY = y * 0.2f - 1;
					float endX = startX + 0.2f, endY = startY + 0.2f;

					int eatId = random.Next(5);

					snacksTexturedRects[y,x] = new TexturedRectangle(
						vbo,
						new PosSegment(startX,  startY, endX, endY),
						new UvSegment(0.125f * eatId, 0.0f, 0.125f * (eatId+1), 0.125f));

					TexturedRectangle.Vertex[] dataPerSnack = snacksTexturedRects[y,x].GetGpuDataAsSixPoints();

					for (int srcIndex = 0, dstIndex = 8*y + x; srcIndex < 6; srcIndex++, dstIndex++)
					{
						data[dstIndex] = dataPerSnack[srcIndex];
					}

					snacksButtons[y,x] = new Button(snacksTexturedRects[y,x]);

					ButtonCheker buttonCheker = new ButtonCheker(snacksTexturedRects[y,x]);
					snacksButtons[y,x].listeners.Add(buttonCheker);
					scene.Instantiate(snacksButtons[y,x]);
				}
			}

			vbo.SetData(data);
		}

		public override void Draw()
		{
			if (vao != null)
			{
				shaderProgram.Use();
				if (texture != null)
					texture.Bind();
				vao.Draw();
			}
		}
	}
}