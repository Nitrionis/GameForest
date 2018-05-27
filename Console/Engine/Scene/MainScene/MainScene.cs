using System;
using Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Scene;

namespace Game
{
	public class MainScene : Scene
	{
		private DateTime endTime = DateTime.Now.AddMinutes(1);

		private SnackMap snackMap;

		private ExplosionsGroup explosionsGroup;

		private TexturedRectangle snackBackGround;

		private TextObject scoreObject;
		private TextObject timeObject;

		public MainScene()
		{
			Texture mainAtlas = new Texture("D:/pdf_sit/GameForest/Console/resources/MainScene/atlas.png");

			CreateBackGroung();

			snackMap = new SnackMap(this, mainAtlas, 8, 8);
			Instantiate(snackMap);

			explosionsGroup = new ExplosionsGroup(this, mainAtlas, 8, 8);
			Instantiate(explosionsGroup);

			TexturedRectangle scoreText = new TexturedRectangle(
				new RectLocation(-0.95f,  0.78f, -0.45f, 0.92f),
				new RectUv(0.5f, 0.125f, 1.0f, 0.25f));
			scoreText.texture = mainAtlas;
			Instantiate(scoreText);

			scoreObject = new TextObject(
				new RectLocation(-0.4f,  0.8f, -0.3f, 0.9f), "0123456789");
			Instantiate(scoreObject);

			timeObject = new TextObject(
				new RectLocation(0.7f,  0.0f, 0.8f, 0.1f), "0123456789");
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
						new RectLocation(startX,  startY, endX, endY),
						new RectUv(0.0f, 0.9375f, 0.0625f, 1.0f));

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
		// TODO remove
		private int seconds;

		private Random random = new Random();

		public override void Update()
		{
			base.Update();

			if (seconds % 200 == 0)
			{
				int expCount = random.Next(3);
				expCount = 0; // TODO
				for (int i = 0; i < expCount; i++)
				{
					int x = random.Next(8), y = random.Next(8);
					explosionsGroup.CreateExplosionIn(x, y);
					snackMap.DeleteSnack(x, y);
				}
				snackMap.DeleteSnack(0, 0);
				snackMap.DeleteSnack(0, 4);
				snackMap.DeleteSnacks();
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