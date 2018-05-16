using System.Runtime.InteropServices;
using Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Scene
{
	public class SquaresGroup : GameObject
	{
		public Texture texture;
		public VBO vbo;
		public VAO vao;
		public ShaderProgram shaderProgram = new ShaderProgram();

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

		public SquaresGroup(uint sizeX, uint sizeY)
		{
			CreateSaders();
		}

		private void CreateSaders()
		{
			using (var vertexShader = new Shader(ShaderType.VertexShader))
			using (var fragmentShader = new Shader(ShaderType.FragmentShader))
			{
				vertexShader.Compile(@"
					#version 400

					uniform vec2 uv_offsets[64];

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