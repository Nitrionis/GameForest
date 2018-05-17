using System;
using System.Runtime.InteropServices;
using Graphics;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Scene
{
	public class TextObject : GameObject
	{
		public bool updateFlaf = false;

		public string text { get; private set; }

		public Texture texture;
		public VBO vbo;
		public VAO vao;
		public ShaderProgram shaderProgram = new ShaderProgram();
		public PosSegment posSegment { get; private set; }

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

		public TextObject(PosSegment posSegment, string text)
		{
			this.posSegment = posSegment;
			this.text = text;
			Initialize();
		}

		private void Initialize()
		{
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
			SetText(text);
			if (vao == null)
				vao = new VAO(4 * text.Length);

			vao.AttachVBO(0, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 0);
			vao.AttachVBO(1, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
			vao.PrimitiveType = PrimitiveType.Quads;

		}

		public override void Draw()
		{
			if (vao != null)
			{
				if (updateFlaf)
				{
					SetText(text);
					updateFlaf = false;
				}
				shaderProgram.Use();
				if (texture != null)
					texture.Bind();
				vao.Draw();
			}
		}

		public void SetText(string str)
		{
			Vertex[] data = new Vertex[4 * str.Length];
			int firstCharId = Convert.ToInt32('0');
			float offsetX = 0;
			for (int end = 4 * str.Length, charIndex = 0, i = 0; i < end; i += 4, offsetX += 0.1f, charIndex++)
			{
				int charId = Convert.ToInt32(str[charIndex]) - firstCharId;

				data[i]   = new Vertex(
					posSegment.startX + offsetX, posSegment.startY,
					0.044921875f * charId, 	0.625f + 0.0625f);
				data[i+1] = new Vertex(
					posSegment.startX + offsetX, posSegment.endY,
					0.044921875f * charId, 	0.625f);
				data[i+2] = new Vertex(
					posSegment.endX + offsetX, posSegment.endY,
					0.044921875f *(charId+1), 0.625f);
				data[i+3] = new Vertex(
					posSegment.endX + offsetX, posSegment.startY,
					0.044921875f *(charId+1), 0.625f + 0.0625f);
			}
			vbo.SetData(data);
		}
	}
}