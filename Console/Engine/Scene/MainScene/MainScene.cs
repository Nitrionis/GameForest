using System;
using Graphics;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using Scene;

namespace Game
{
	public class MainScene : Scene
	{
		private const float UvSnackSize = 0.125f;
		private const float XySnackSize = 0.2f;
		private const int VertexPerSnack = 6;

		private const int BgQuadSize = 10; // Bg is background
		private const int XyMapQuadSize = 8;

		private DateTime endTime = DateTime.Now.AddMinutes(1);

		private MovableQuads movableQuads;

		private SnackMap snackMap;

		private ExplosionsGroup explosionsGroup;

		private TexturedRectangle snackBackGround;

		private TextObject scoreObject;
		private TextObject timeObject;

		public MainScene()
		{
			GlobalReference.score = 0;

			Texture mainAtlas = new Texture(".\\resources\\MainScene\\atlas.png");

			CreateBackGroung();

			explosionsGroup = new ExplosionsGroup(this, mainAtlas, XyMapQuadSize, XyMapQuadSize);
			snackMap = new SnackMap(explosionsGroup, mainAtlas, XyMapQuadSize, XyMapQuadSize);

			movableQuads = new MovableQuads(this, snackMap, mainAtlas);
			Instantiate(movableQuads);

			Instantiate(snackMap);

			Instantiate(explosionsGroup);

			TexturedRectangle scoreText = new TexturedRectangle(
				new RectLocation(-0.95f,  0.78f, -0.45f, 0.92f),
				new RectUv(4*UvSnackSize, UvSnackSize, 8*UvSnackSize, 2*UvSnackSize));
			scoreText.texture = mainAtlas;
			Instantiate(scoreText);

			scoreObject = new TextObject(
				new RectLocation(-2*XySnackSize,  4*XySnackSize, -2*XySnackSize + 0.1f, 0.9f), "0123456789");
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

			var data = new TexturedRectangle.Vertex[VertexPerSnack * BgQuadSize * BgQuadSize];

			for (int y = 0; y < BgQuadSize; y++)
			{
				for (int x = 0; x < BgQuadSize; x++)
				{
					float startX = x * XySnackSize - 1f, startY = y * XySnackSize - 1f;
					float endX = startX + XySnackSize, endY = startY + XySnackSize;

					TexturedRectangle segment = new TexturedRectangle(
						vbo,
						new RectLocation(startX,  startY, endX, endY),
						new RectUv(0.0f, 0.9375f, 0.0625f, 1.0f));

					TexturedRectangle.Vertex[] dataPerSnack = segment.GetGpuDataAsSixPoints();

					for (int srcIndex = 0, dstIndex = (BgQuadSize*y + x)*VertexPerSnack;
						srcIndex < VertexPerSnack; srcIndex++, dstIndex++)
					{
						data[dstIndex] = dataPerSnack[srcIndex];
					}
				}
			}

			vbo.SetData(data);

			VAO vao = new VAO(VertexPerSnack * BgQuadSize * BgQuadSize);
			vao.AttachVBO(0, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 0);
			vao.AttachVBO(1, vbo, 2, VertexAttribPointerType.Float, 4 * sizeof(float), 2 * sizeof(float));
			vao.PrimitiveType = PrimitiveType.Triangles;

			snackBackGround.vao = vao;
			snackBackGround.vbo = vbo;
			snackBackGround.shaderProgram = TexturedRectangle.GetShaderProgram();
			Instantiate(snackBackGround);

		}
		//private Random random = new Random();

		public override void Update()
		{
			base.Update();

			/*if (score % 200 == 0)
			{
				int expCount = random.Next(10);
				for (int i = 0; i < expCount; i++)
				{
					int x = random.Next(8), y = random.Next(8);
					explosionsGroup.CreateExplosionIn(x, y);
					snackMap.DeleteSnack(x, y);
				}
				snackMap.DeleteSnacks();
			}*/

			if (endTime > DateTime.Now)
			{
				timeObject.SetText((endTime - DateTime.Now).Seconds.ToString());
				scoreObject.SetText(GlobalReference.score.ToString());
			}
			else
			{
				GlobalReference.window.ChangeScene<EndUiScene>();
			}
		}


	}
}