﻿using System;
using System.IO;
using Graphics;
using OpenTK.Graphics.OpenGL4;

namespace Scene
{
	public class SnackGroup : GameObject
	{
		private int uniformTimeHandler;
		private int uniformOffsetsHandler;

		private VBO vbo;
		private VAO vao;
		private Texture texture;
		private ShaderProgram shaderProgram;

		protected Snack[] snacks;

		protected MapGenerator mapGenerator { get; private set; }

		protected float time;
		protected float[] moveOffsets;

		protected int sizeX;
		protected int sizeY;

		protected const float UvSnackSize = 0.125f;
		protected const float XySnackSize = 0.2f;
		protected const int VertexPerSnack = 6;

		public SnackGroup(Texture texture, int sizeX, int sizeY)
		{
			this.texture = texture;
			this.sizeX = sizeX;
			this.sizeY = sizeY;

			snacks = new Snack[sizeX * sizeY];

			moveOffsets = new float[sizeX * sizeY];

			CreateShaderProgram();
			CreateMesh();
		}

		public void CreateMesh()
		{
			vbo = new VBO();

			mapGenerator = new MapGenerator();
			mapGenerator.NewMap();

			var data = new TexturedRectangle.Vertex[VertexPerSnack * sizeX * sizeY];

			for (int snackIndex = 0, x = 0; x < sizeX; x++)
			{
				for (int y = 0; y < sizeY; y++, snackIndex++)
				{
					float startX = x * XySnackSize - 1f, startY = y * XySnackSize - 1f;
					float endX = startX + XySnackSize, endY = startY + XySnackSize;

					int snackId = mapGenerator.map[x, y];
					//int snackId = y / 4;

					snacks[snackIndex] = new Snack(
						vbo,
						new RectLocation(startX, -1.0f, endX, -0.8f),
						new RectUv(UvSnackSize * snackId, 0.0f, UvSnackSize * (snackId + 1), UvSnackSize),
						x, VertexPerSnack*(x * sizeY + y));

					snacks[snackIndex].height = y;
					snacks[snackIndex].snackId = snackId;

					var dataPerSnack = snacks[snackIndex].GetGpuDataAsSixPoints();

					for (int srcIndex = 0, dstIndex = VertexPerSnack*(x * sizeY + y); srcIndex < VertexPerSnack; srcIndex++, dstIndex++)
					{
						data[dstIndex] = dataPerSnack[srcIndex];
					}

					snacks[snackIndex].pos.startY = startY;
					snacks[snackIndex].pos.endY = endY;

					moveOffsets[x * sizeY + y] = y * XySnackSize;
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

					out vec2 uv;

					const float yPos[] = {-1.0f, -0.8f, -0.8f, -1.0f, -0.8f, -1.0f};

					void main() {
						vec2 pos = vec2(in_Position.x, yPos[gl_VertexID % 6]);
						pos.x += sin(time + pos.x * 10) * 0.01f;
						float yOffset = offsets[gl_VertexID / 6];
						pos.y += cos(time + pos.y + yOffset) * 0.01f + yOffset;
    					gl_Position = vec4(pos, 0.0, 1.0f);
						uv = in_UV;
					}
					");
				fragmentShader.Compile(@"
					#version 400

					uniform sampler2D tex;

					in vec2 uv;

					layout(location = 0) out vec4 out_Color;

					void main() {
    					out_Color = texture(tex, uv);
					}
					");
				shaderProgram.AttachShader(vertexShader);
				shaderProgram.AttachShader(fragmentShader);
				shaderProgram.Link();
			}
			uniformTimeHandler = shaderProgram.GetUniformLocation("time");
			uniformOffsetsHandler = shaderProgram.GetUniformLocation("offsets");
		}

		private void UpdateMoveOffset()
		{
			for (int end = sizeX * sizeY, i = 0; i < end; i++)
			{
				var snack = snacks[i];
				long value = snack.animationTime - snack.sw.ElapsedMilliseconds;
				if (value < 0)
					value = 0;
				moveOffsets[i] = snack.height * XySnackSize + value / 500f;
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


				UpdateMoveOffset();

				GL.Uniform1(uniformTimeHandler, 1, ref time);
				GL.Uniform1(uniformOffsetsHandler, 64, moveOffsets);

				if (texture != null)
					texture.Bind();
				vao.Draw();
			}
		}
	}
}