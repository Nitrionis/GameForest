using System.Diagnostics;
using Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Scene
{
	public class ExplosionsGroup : GameObject
	{
		private Game.Scene scene;

		private int uniformBufferHandler;

		private VBO vbo;
		private VAO vao;
		private Texture texture;
		private ShaderProgram shaderProgram;

		private uint[] expData;
		private Stopwatch[] sw;

		public int sizeX { get; private set; }
		public int sizeY { get; private set; }

		private const float UvSnackSize = 0.125f;
		private const float XySnackSize = 0.2f;
		private const int VertexPerSnack = 6;

		public ExplosionsGroup(Game.Scene scene, Texture texture, int sizeX, int sizeY)
		{
			this.sizeX = sizeX;
			this.sizeY = sizeY;
			this.scene = scene;
			this.texture = texture;

			expData = new uint[sizeX * sizeY];
			sw = new Stopwatch[sizeX * sizeY];

			CreateShaderProgram();
			CreateMesh();

			for (int end = sizeX * sizeY, i = 0; i < end; i++)
			{
				sw[i] = new Stopwatch();
				sw[i].Start();
			}
		}

		public void CreateExplosionIn(int x, int y)
		{
			sw[y * sizeX + x].Restart();
		}

		protected void CreateMesh()
		{
			vbo = new VBO(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);

			var data = new TexturedRectangle.Vertex[6 * sizeX * sizeY];

			for (int y = 0; y < sizeY; y++)
			{
				for (int x = 0; x < sizeX; x++)
				{
					float startX = x * XySnackSize - 1f - 0.1f, startY = y * XySnackSize - 1f - 0.1f;
					float endX = startX + 2*XySnackSize, endY = startY + 2*XySnackSize;

					var snacksTexturedRects = new TexturedRectangle(
						vbo,
						new RectLocation(startX,  startY, endX, endY),
						new RectUv(0.0f, UvSnackSize, UvSnackSize, 2*UvSnackSize));

					TexturedRectangle.Vertex[] dataPerSnack = snacksTexturedRects.GetGpuDataAsSixPoints();

					for (int srcIndex = 0, dstIndex = VertexPerSnack*(sizeX*y + x);
						srcIndex < VertexPerSnack; srcIndex++, dstIndex++)
					{
						data[dstIndex] = dataPerSnack[srcIndex];
					}
				}
			}

			vbo.SetData(data);

			vao = new VAO(VertexPerSnack * sizeX * sizeY);
			vao.AttachVBO(0, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 0);
			vao.AttachVBO(1, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
			vao.PrimitiveType = PrimitiveType.Triangles;
		}

		protected void CreateShaderProgram()
		{
			shaderProgram = new ShaderProgram();
			using (var vertexShader = new Shader(ShaderType.VertexShader))
			using (var fragmentShader = new Shader(ShaderType.FragmentShader))
			{
				vertexShader.Compile(@"
					#version 400

					uniform uint exp_id[64];

					layout(location = 0) in vec2 in_Position;
					layout(location = 1) in vec2 in_UV;

					layout(location = 0) out vec2 out_UV;

					void main() {
						uint value = exp_id[gl_VertexID / 6];
						vec2 uvOffset = vec2(
								(value & 0xff) * 0.125f,
								(value >> 16) * 0.125f);
    					gl_Position = vec4(in_Position, 0.0, 1.0);
						out_UV = in_UV + uvOffset;
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
			uniformBufferHandler = shaderProgram.GetUniformLocation("exp_id");
		}

		public override void Draw()
		{
			if (vao != null)
			{
				for (int end = sizeX * sizeY, i = 0; i < end; i++)
				{
					long time = sw[i].ElapsedMilliseconds;
					uint y_id, x_id;

					if (sw[i].ElapsedMilliseconds < 999)
					{
						y_id = (uint) (time / 250);
						x_id = (uint)(time - y_id * 250) / 63;
						expData[i] = (y_id << 16) + x_id;
						if (x_id != 0)
						{
							x_id = 0;
						}
					}
					else
					{
						expData[i] = (3 << 16) + 3;
					}
				}
				shaderProgram.Use();
				GL.Uniform1(uniformBufferHandler, 64, expData);
				if (texture != null)
					texture.Bind();
				vao.Draw();
			}
		}
	}
}