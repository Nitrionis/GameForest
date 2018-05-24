using System;
using System.Runtime.InteropServices;
using Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Scene
{
	public class SnackGroup : GameObject
	{
		protected Game.Scene scene;

		protected int uniformTimeHandler;
		protected int uniformOffsetsHandler;

		protected VBO vbo;
		protected VAO vao;
		protected Texture texture;
		protected ShaderProgram shaderProgram;

		protected struct Snack
		{
			public TexturedRectangle graphics;
			public bool deleteFlag;

			public Snack(TexturedRectangle texturedRectangle, bool flag = false)
			{
				graphics = texturedRectangle;
				deleteFlag = flag;
			}
		}

		protected Snack[,] snacks;

		//protected TexturedRectangle[,] snacksTexturedRects;
		protected Button[,] snacksButtons;

		public MapGenerator mapGenerator { get; private set; }

		protected float time;
		protected float[] moveOffsets;
		protected int[,] snacksMap;

		protected int sizeX;
		protected int sizeY;

		public SnackGroup(Game.Scene scene, Texture texture, int sizeX, int sizeY)
		{
			this.scene = scene;
			this.texture = texture;
			this.sizeX = sizeX;
			this.sizeY = sizeY;

			//snacksTexturedRects = new TexturedRectangle[sizeY, sizeX];
			snacks = new Snack[sizeY, sizeX];
			snacksButtons = new Button[sizeY, sizeX];
			moveOffsets = new float[sizeX * sizeY];
			snacksMap = new int[sizeY, sizeX];

			CreateShaderProgram();
			CreateMesh();
		}

		class ButtonCheker : IButtonAction
		{
			private TexturedRectangle texturedRectangle;
			public int x, y; // are used to determine the VBO segment responsible for this snack

			public ButtonCheker(TexturedRectangle texturedRectangle, int x, int y)
			{
				this.texturedRectangle = texturedRectangle;
				this.x = x;
				this.y = y;
			}

			public void Event(int state)
			{
				texturedRectangle.offsetU = 0.125f * state;
				texturedRectangle.vbo.SetSubData(texturedRectangle.GetGpuDataAsSixPoints(), 6, (8*y + x)*6);
			}
		}

		public void CreateMesh()
		{
			vbo = new VBO();

			Random random = new Random();

			mapGenerator = new MapGenerator();
			mapGenerator.NewMap();

			TexturedRectangle.Vertex[] data = new TexturedRectangle.Vertex[6 * sizeX * sizeY];

			for (int y = 0; y < sizeY; y++)
			{
				for (int x = 0; x < sizeX; x++)
				{
					float startX = x * 0.2f - 1, startY = y * 0.2f - 1;
					float endX = startX + 0.2f, endY = startY + 0.2f;

					int eatId = mapGenerator.map[y, x];

					snacks[y,x] = new Snack(new TexturedRectangle(
						vbo,
						new PosSegment(startX,  startY, endX, endY),
						new UvSegment(0.125f * eatId, 0.0f, 0.125f * (eatId+1), 0.125f)));

					TexturedRectangle.Vertex[] dataPerSnack = snacks[y,x].graphics.GetGpuDataAsSixPoints();

					for (int srcIndex = 0, dstIndex = (sizeX*y + x)*6; srcIndex < 6; srcIndex++, dstIndex++)
					{
						data[dstIndex] = dataPerSnack[srcIndex];
					}

					snacksButtons[y,x] = new Button(snacks[y,x].graphics);

					ButtonCheker buttonCheker = new ButtonCheker(snacks[y,x].graphics, x, y);
					snacksButtons[y,x].listeners.Add(buttonCheker);
					scene.Instantiate(snacksButtons[y,x]);
				}
			}

			vbo.SetData(data);

			vao = new VAO(6 * sizeX * sizeY);
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

		public void DeleteVerticalLineOfSnacks(int x, int y, int height = 1)
		{

		}

		private void DeleteSnacks()
		{

		}

		public override void Draw()
		{
			if (vao != null)
			{
				shaderProgram.Use();
				time = DateTime.Now.Millisecond / 1000f;
				if (DateTime.Now.Second % 2 != 0)
					time = 1.0f - time;
				for (int i = 0; i < moveOffsets.Length; i++)
				{
					moveOffsets[i] = 1.0f - DateTime.Now.Millisecond / 1000f;
				}
				GL.Uniform1(uniformTimeHandler, 1, ref time);
				//GL.Uniform1(uniformOffsetsHandler, 64, moveOffsets);
				if (texture != null)
					texture.Bind();
				vao.Draw();
			}
		}
	}
}