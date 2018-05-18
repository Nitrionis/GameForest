using System.Runtime.InteropServices;
using Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using DrawBufferMode = OpenTK.Graphics.ES11.DrawBufferMode;

namespace Scene
{
	public class UvSegment
	{
		public float startU;
		public float startV;
		public float endU;
		public float endV;

		public UvSegment(float startU, float startV, float endU, float endV)
		{
			this.startU = startU;
			this.startV = startV;
			this.endU = endU;
			this.endV = endV;
		}
	}

	public class PosSegment
	{
		public float startX;
		public float startY;
		public float endX;
		public float endY;

		public PosSegment(float startX, float startY, float endX, float endY)
		{
			if (startX < -1) startX = -1;
			if (startX > 1) startX = 1;
			if (startY < -1) startY = -1;
			if (startY > 1) startY = 1;
			if (endX < -1) endX = -1;
			if (endX > 1) endX = 1;
			if (endY < -1) endY = -1;
			if (endY > 1) endY = 1;

			this.startX = startX;
			this.startY = startY;
			this.endX = endX;
			this.endY = endY;
		}
	}

	public interface IRectangleGetter
	{
		PosSegment GetPosSegment();
	}

	public class TexturedRectangle : GameObject, IRectangleGetter
	{
		public bool updateFlaf = false;
		public float offsetU = 0, offsetV = 0;

		public Texture texture;
		public VBO vbo;
		public VAO vao;
		public ShaderProgram shaderProgram = new ShaderProgram();

		[StructLayout(LayoutKind.Sequential)]
		public struct Vertex
		{
			public float X, Y;
			public float U, V;

			public Vertex(float x, float y, float u, float v)
			{
				X = x; Y = y;
				U = u; V = v;
			}
		}

		public UvSegment uv { get; private set; }
		public PosSegment pos { get; private set; }

		public PosSegment GetPosSegment()
		{
			return pos;
		}

		public TexturedRectangle(VBO vbo, PosSegment pos, UvSegment uv)
		{
			this.vbo = vbo;
			this.pos = pos;
			this.uv = uv;
			Initialize();
		}

		public TexturedRectangle(PosSegment pos, UvSegment uv)
		{
			this.pos = pos;
			this.uv = uv;
			Initialize();
		}

		private void Initialize()
		{
			if (vbo == null)
			{
				CreateSaders();
				CreateMesh();
			}
		}

		private void CreateSaders()
		{
			using (var vertexShader = new Shader(ShaderType.VertexShader))
			using (var fragmentShader = new Shader(ShaderType.FragmentShader))
			{
				vertexShader.Compile(@"
					#version 400

					//uniform mat4 coeff;

					layout(location = 0) in vec2 in_Position;
					layout(location = 1) in vec2 in_UV;

					layout(location = 0) out vec2 out_UV;

					void main() {
    					gl_Position = vec4(in_Position, 0.0, 1.0);
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
		}

		static public Vertex[] GetGpuDataAsFourPoints(TexturedRectangle tr)
		{
			return new []
			{
				new Vertex(tr.pos.startX, tr.pos.startY, tr.uv.startU + tr.offsetU, tr.uv.endV + tr.offsetV),
				new Vertex(tr.pos.startX, tr.pos.endY,   tr.uv.startU + tr.offsetU, tr.uv.startV + tr.offsetV),
				new Vertex(tr.pos.endX,   tr.pos.endY,   tr.uv.endU + tr.offsetU,   tr.uv.startV + tr.offsetV),
				new Vertex(tr.pos.endX,   tr.pos.startY, tr.uv.endU + tr.offsetU,   tr.uv.endV + tr.offsetV)
			};
		}

		static public Vertex[] GetGpuDataAsSixPoints(TexturedRectangle tr)
		{
			return new []
			{
				new Vertex(tr.pos.startX, tr.pos.startY, tr.uv.startU + tr.offsetU, tr.uv.endV + tr.offsetV),
				new Vertex(tr.pos.startX, tr.pos.endY,   tr.uv.startU + tr.offsetU, tr.uv.startV + tr.offsetV),
				new Vertex(tr.pos.endX,   tr.pos.endY,   tr.uv.endU + tr.offsetU,   tr.uv.startV + tr.offsetV),
				new Vertex(tr.pos.startX, tr.pos.startY, tr.uv.startU + tr.offsetU, tr.uv.endV + tr.offsetV),
				new Vertex(tr.pos.endX,   tr.pos.endY,   tr.uv.endU + tr.offsetU,   tr.uv.startV + tr.offsetV),
				new Vertex(tr.pos.endX,   tr.pos.startY, tr.uv.endU + tr.offsetU,   tr.uv.endV + tr.offsetV)
			};
		}

		private void CreateMesh()
		{
			if (vbo == null)
			{
				vbo = new VBO();
			}
			else
			{
				return;
			}

			vbo.SetData(GetGpuDataAsFourPoints(this));

			if (vao == null)
				vao = new VAO(4);
			vao.AttachVBO(0, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 0);
			vao.AttachVBO(1, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
			vao.PrimitiveType = PrimitiveType.TriangleFan;

		}

		private void RecalculateMesh()
		{
			vbo.SetSubData(GetGpuDataAsFourPoints(this), 4);
		}

		public override void Draw()
		{
			if (vao != null)
			{
				if (updateFlaf)
				{
					RecalculateMesh();
					updateFlaf = false;
				}
				shaderProgram.Use();
				if (texture != null)
					texture.Bind();
				vao.Draw();
			}
		}


	}
}