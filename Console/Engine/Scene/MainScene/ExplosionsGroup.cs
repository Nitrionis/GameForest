﻿using System;
using System.Diagnostics;
using Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Scene
{
	public class ExplosionsGroup : GameObject
	{
		protected Game.Scene scene;

		protected int uniformBufferHandler;

		protected VBO vbo;
		protected VAO vao;
		protected Texture texture;
		protected ShaderProgram shaderProgram;

		protected uint[] expData;
		protected Stopwatch[] sw;

		public int sizeX { get; private set; }
		public int sizeY { get; private set; }

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

			TexturedRectangle.Vertex[] data = new TexturedRectangle.Vertex[6 * sizeX * sizeY];

			for (int y = 0; y < sizeY; y++)
			{
				for (int x = 0; x < sizeX; x++)
				{
					float startX = x * 0.2f - 1 - 0.1f, startY = y * 0.2f - 1 - 0.1f;
					float endX = startX + 0.2f + 0.2f, endY = startY + 0.2f + 0.2f;

					TexturedRectangle snacksTexturedRects = new TexturedRectangle(
						vbo,
						new PosSegment(startX,  startY, endX, endY),
						new UvSegment(0.0f, 0.125f, 0.125f, 0.25f));

					TexturedRectangle.Vertex[] dataPerSnack = snacksTexturedRects.GetGpuDataAsSixPoints();

					for (int srcIndex = 0, dstIndex = (sizeX*y + x)*6; srcIndex < 6; srcIndex++, dstIndex++)
					{
						data[dstIndex] = dataPerSnack[srcIndex];
					}
				}
			}

			vbo.SetData(data);

			vao = new VAO(6 * sizeX * sizeY);
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
								(value & 65535) * 0.125f,
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