using System;
using System.Diagnostics;
using Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Scene
{
	public class SnackGroup : GameObject
	{
		private Game.Scene scene;

		private int uniformTimeHandler;
		private int uniformOffsetsHandler;

		private VBO vbo;
		private VAO vao;
		private Texture texture;
		private ShaderProgram shaderProgram;

		private Snack[,] snacks;

		private Button[,] snacksButtons;

		public MapGenerator mapGenerator { get; private set; }

		private float time;
		private float[] moveOffsets;

		private int sizeX;
		private int sizeY;

		private const float UvSnackSize = 0.125f;
		private const float XySnackSize = 0.2f;
		private const int VertexPerSnack = 6;

		public SnackGroup(Game.Scene scene, Texture texture, int sizeX, int sizeY)
		{
			this.scene = scene;
			this.texture = texture;
			this.sizeX = sizeX;
			this.sizeY = sizeY;

			snacks = new Snack[sizeY, sizeX];
			snacksButtons = new Button[sizeY, sizeX];
			moveOffsets = new float[sizeY * sizeX];

			CreateShaderProgram();
			CreateMesh();
		}

		class ButtonCheker : IButtonAction
		{
			private Snack snack;

			public ButtonCheker(Snack snack)
			{
				this.snack = snack;
			}

			public void Event(int state)
			{
				snack.offsetU = UvSnackSize * state;
				snack.vbo.SetSubData(new []
				{
					/* Когда обновляем данные снэков, необходимо пересчитывать
					 * позиции по Y, так как они не связаны с данными на видеокарте,
					 * а используются для определения кликов по снэкам
					 */
					new TexturedRectangle.Vertex(snack.pos.startX, -1.0f,
						snack.uv.startU + snack.offsetU, snack.uv.endV),
					new TexturedRectangle.Vertex(snack.pos.startX, -1.0f + XySnackSize,
						snack.uv.startU + snack.offsetU, snack.uv.startV),
					new TexturedRectangle.Vertex(snack.pos.endX,   -1.0f + XySnackSize,
						snack.uv.endU   + snack.offsetU, snack.uv.startV),
					new TexturedRectangle.Vertex(snack.pos.startX, -1.0f,
						snack.uv.startU + snack.offsetU, snack.uv.endV),
					new TexturedRectangle.Vertex(snack.pos.endX,   -1.0f + XySnackSize,
						snack.uv.endU   + snack.offsetU, snack.uv.startV),
					new TexturedRectangle.Vertex(snack.pos.endX,   -1.0f,
						snack.uv.endU   + snack.offsetU, snack.uv.endV)
				}, 6, snack.vboDataOffset);
			}
		}

		public void CreateMesh()
		{
			vbo = new VBO();

			mapGenerator = new MapGenerator();
			mapGenerator.NewMap();

			var data = new TexturedRectangle.Vertex[6 * sizeX * sizeY];

			for (int y = 0; y < sizeY; y++)
			{
				for (int x = 0; x < sizeX; x++)
				{
					float startX = x * XySnackSize - 1f, startY = y * XySnackSize - 1f;
					float endX = startX + XySnackSize, endY = startY + XySnackSize;

					int eatId = mapGenerator.map[y, x];

					snacks[y, x] = new Snack(
						vbo,
						new RectLocation(startX, -1.0f, endX, -0.8f),
						new RectUv(UvSnackSize * eatId, 0.0f, UvSnackSize * (eatId + 1), UvSnackSize),
						y, VertexPerSnack*(y*sizeX + x));

					var dataPerSnack = snacks[y,x].GetGpuDataAsSixPoints();

					for (int srcIndex = 0, dstIndex = VertexPerSnack*(sizeX*y + x); srcIndex < VertexPerSnack; srcIndex++, dstIndex++)
					{
						data[dstIndex] = dataPerSnack[srcIndex];
					}

					snacks[y, x].pos.startY = startY;
					snacks[y, x].pos.endY = endY;

					snacksButtons[y,x] = new Button(snacks[y,x]);

					ButtonCheker buttonCheker = new ButtonCheker(snacks[y,x]);
					snacksButtons[y,x].listeners.Add(buttonCheker);
					scene.Instantiate(snacksButtons[y,x]);
				}
			}

			vbo.SetData(data);

			vao = new VAO(VertexPerSnack * sizeX * sizeY);
			vao.AttachVBO(0, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 0);
			vao.AttachVBO(1, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
			vao.PrimitiveType = PrimitiveType.Triangles;
		}

		private void CreateShaderProgram()
		{
			shaderProgram = new ShaderProgram();
			using (var vertexShader = new Shader(ShaderType.VertexShader))
			using (var fragmentShader = new Shader(ShaderType.FragmentShader))
			{
				vertexShader.Compile(@"
					#version 400

					uniform float time;
					uniform float offsets[64];

					layout(location = 0) in vec2 in_Position;
					layout(location = 1) in vec2 in_UV;

					layout(location = 0) out vec2 out_UV;

					void main() {
						vec2 pos = in_Position;
						pos.x += sin(time + pos.x * 10) * 0.01f;
						pos.y += cos(time + pos.y * 10) * 0.01f + offsets[gl_VertexID / 6];
    					gl_Position = vec4(pos, 0.0, 1.0);
						out_UV = in_UV;
					}
					");
				fragmentShader.Compile(@"
					#version 400

					uniform sampler2D tex;

					layout(location = 0) in vec2 in_UV;

					layout(location = 0) out vec4 out_Color;

					void main() {
    					out_Color = texture(tex, in_UV);
					}
					");
				shaderProgram.AttachShader(vertexShader);
				shaderProgram.AttachShader(fragmentShader);
				shaderProgram.Link();
			}
			uniformTimeHandler = shaderProgram.GetUniformLocation("time");
			uniformOffsetsHandler = shaderProgram.GetUniformLocation("offsets");
		}

		public void DeleteSnack(int x, int y)
		{
			snacks[y, x].deleteFlag = true;
		}

		private void DeleteSnacks()
		{
			for (int x = 0; x < sizeX; x++)
			{
				for (int srcIndex = 1, dstIndex = 0; srcIndex < sizeY; srcIndex++, dstIndex++)
				{
					for (; srcIndex < sizeY; srcIndex++, dstIndex++)
						if (snacks[dstIndex, x].deleteFlag)
						{
							Snack value = snacks[dstIndex, x];
							snacks[dstIndex, x] = snacks[srcIndex, x];
							snacks[srcIndex, x] = value;
							break;
						}
				}

				for (int y = sizeY - 1; y >= 0; y--)
				{
					float startY = y * XySnackSize - 1;
					float endY = startY + XySnackSize;

					snacks[y, x].pos.startY = startY;
					snacks[y, x].pos.endY = endY;

					if (snacks[y, x].deleteFlag)
					{
						snacks[y, x].deleteFlag = false;
						snacks[y, x].sw.Restart();
					}
				}
			}
		}

		public override void Draw()
		{
			if (vao != null)
			{
				shaderProgram.Use();
				time = DateTime.Now.Millisecond / 1000f;
				if (DateTime.Now.Second % 2 != 0)
					time = 1.0f - time;

				if ((DateTime.Now.Second % 5 == 0) && (DateTime.Now.Millisecond < 20))
				{
					DeleteSnack(0,6);
				}
				DeleteSnacks();
				for (int y = 0; y < sizeY; y++)
				{
					for (int x = 0; x < sizeX; x++)
					{
						long value = 1000 - snacks[y, x].sw.ElapsedMilliseconds;
						if (value < 0)
							value = 0;

						moveOffsets[y * sizeX + x] = snacks[y, x].height * XySnackSize + value / 1000f;
					}
				}

				GL.Uniform1(uniformTimeHandler, 1, ref time);
				GL.Uniform1(uniformOffsetsHandler, 64, moveOffsets);
				if (texture != null)
					texture.Bind();
				vao.Draw();
			}
		}
	}
}