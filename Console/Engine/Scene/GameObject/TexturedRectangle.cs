using System.Runtime.InteropServices;
using Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using DrawBufferMode = OpenTK.Graphics.ES11.DrawBufferMode;

namespace Scene
{
	public class TexturedRectangle : GameObject
	{
		//public
		public VBO vbo { get; private set; }
		public VAO vao { get; private set; }
		private ShaderProgram shaderProgram = new ShaderProgram();
		public Texture texture;

		[StructLayout(LayoutKind.Sequential)]
		struct Vertex
		{
			public float X, Y;
			public float U, V;

			public Vertex(float x, float y, float u, float v)
			{
				X = x; Y = y;
				U = u; V = v;
			}
		}

		public float startX { get; private set; }
		public float startY { get; private set; }
		public float endX { get; private set; }
		public float endY { get; private set; }

		public float startU { get; private set; }
		public float startV { get; private set; }
		public float endU { get; private set; }
		public float endV { get; private set; }

		public TexturedRectangle(float startX, float startY, float endX, float endY,
			float startU, float startV, float endU, float endV) : base()
		{
			if (startX < -1) startX = -1;
			if (startX > 1) startX = 1;
			if (startY < -1) startY = -1;
			if (startY > 1) startY = 1;
			if (endX < 0) endX = 0;
			if (endX > 1) endX = 1;
			if (endY < 0) endY = 0;
			if (endY > 1) endY = 1;

			this.startX = startX;
			this.startY = startY;
			this.endX = endX;
			this.endY = endY;
			this.startU = startU;
			this.startV = startV;
			this.endU = endU;
			this.endV = endV;

			CreateSaders();
			CreateMesh();
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

		private void CreateMesh()
		{
			if (vbo == null)
				vbo = new VBO();
			vbo.SetData(new[] {
				new Vertex(startX, startY, startU, endV),
				new Vertex(startX,   endY, startU, startV),
				new Vertex(  endX,   endY, endU,   startV),
				new Vertex(  endX, startY, endU,   endV)
			});
			if (vao == null)
				vao = new VAO(4);
			vao.AttachVBO(0, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 0);
			vao.AttachVBO(1, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
			vao.PrimitiveType = PrimitiveType.TriangleFan;
		}

		public override void Draw()
		{
			shaderProgram.Use();
			if (texture != null)
				texture.Bind();
			vao.Draw();
		}
	}
}