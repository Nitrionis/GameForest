using System;
using System.Diagnostics;
using Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using Scene;
using Button = Scene.Button;

namespace Game
{
	public class MainScene : Scene
	{
		private DateTime endTime = DateTime.Now.AddMinutes(1);

		private SnackGroup snackGroup;

		private ExplosionsGroup explosionsGroup;

		private TexturedRectangle snackBackGround;

		private TextObject scoreObject;
		private TextObject timeObject;

		public MainScene()
		{
			Texture mainAtlas = new Texture("D:/pdf_sit/GameForest/Console/resources/MainScene/atlas.png");

			CreateBackGroung();

			snackGroup = new SnackGroup(this, mainAtlas, 8, 8);
			Instantiate(snackGroup);

			explosionsGroup = new ExplosionsGroup(this, mainAtlas, 8, 8);
			Instantiate(explosionsGroup);

			TexturedRectangle scoreText = new TexturedRectangle(
				new PosSegment(-0.95f,  0.78f, -0.45f, 0.92f),
				new UvSegment(0.5f, 0.125f, 1.0f, 0.25f));
			scoreText.texture = mainAtlas;
			Instantiate(scoreText);

			scoreObject = new TextObject(
				new PosSegment(-0.4f,  0.8f, -0.3f, 0.9f), "0123456789");
			Instantiate(scoreObject);

			timeObject = new TextObject(
				new PosSegment(0.7f,  0.0f, 0.8f, 0.1f), "0123456789");
			Instantiate(timeObject);

			GL.ClearColor(new Color4(34, 34, 34, 255));
		}

		private void CreateBackGroung()
		{
			VBO vbo = new VBO(BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw);
			snackBackGround = new TexturedRectangle(vbo, null, null);

			TexturedRectangle.Vertex[] data = new TexturedRectangle.Vertex[6 * 10 * 10];

			for (int y = 0; y < 10; y++)
			{
				for (int x = 0; x < 10; x++)
				{
					float startX = x * 0.2f - 1, startY = y * 0.2f - 1;
					float endX = startX + 0.2f, endY = startY + 0.2f;

					TexturedRectangle segment = new TexturedRectangle(
						vbo,
						new PosSegment(startX,  startY, endX, endY),
						new UvSegment(0.0f, 0.9375f, 0.0625f, 1.0f));

					TexturedRectangle.Vertex[] dataPerSnack = segment.GetGpuDataAsSixPoints();

					for (int srcIndex = 0, dstIndex = (10*y + x)*6; srcIndex < 6; srcIndex++, dstIndex++)
					{
						data[dstIndex] = dataPerSnack[srcIndex];
					}
				}
			}

			vbo.SetData(data);

			VAO vao = new VAO(6 * 10 * 10);
			vao.AttachVBO(0, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 0);
			vao.AttachVBO(1, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
			vao.PrimitiveType = PrimitiveType.Triangles;

			snackBackGround.vao = vao;
			snackBackGround.vbo = vbo;
			snackBackGround.shaderProgram = TexturedRectangle.GetShaderProgram();
			Instantiate(snackBackGround);

		}

		private int seconds;

		Random random = new Random();

		public override void Update()
		{
			base.Update();

			if (DateTime.Now.Millisecond < 20 || (DateTime.Now.Millisecond > 480 && DateTime.Now.Millisecond < 500))
			{
				int expCount = random.Next(10);
				for (int i = 0; i < expCount; i++)
				{
					explosionsGroup.CreateExplosionIn(random.Next(8), random.Next(8));
				}
			}

			if (endTime > DateTime.Now)
			{
				CheckEvents();
				timeObject.SetText((endTime - DateTime.Now).Seconds.ToString());
				scoreObject.SetText(seconds.ToString());
				seconds++;
			}
		}

		private void CheckEvents()
		{

		}
	}
}