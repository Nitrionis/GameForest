using System.Diagnostics;
using Graphics;

namespace Scene
{
	public class Snack : TexturedRectangle
	{
		private const float UvSnackSize = 0.125f;

		public int vboDataOffset;
		public int height;
		public bool deleteFlag;
		public long animationTime;
		public int snackId;

		public Stopwatch sw;

		public Snack(VBO vbo, RectLocation pos, RectUv uv, int height, int vboDataOffset) : base(vbo, pos, uv)
		{
			animationTime = 1000;
			this.height = height;
			this.vboDataOffset = vboDataOffset;
			sw = new Stopwatch();
			sw.Start();
		}

		public void UpdateUvOffsetUsingId()
		{
			uv.startU = snackId * UvSnackSize;
			uv.endU = uv.startU + UvSnackSize;

			vbo.SetSubData(new []
			{
				/* Y они не связаны с данными на видеокарте,
				 * а используются для определения кликов по снэкам.
				 */
				new TexturedRectangle.Vertex(pos.startX, -1.0f, uv.startU, uv.endV),
				new TexturedRectangle.Vertex(pos.startX, -1.0f, uv.startU, uv.startV),
				new TexturedRectangle.Vertex(pos.endX,   -1.0f, uv.endU  , uv.startV),
				new TexturedRectangle.Vertex(pos.startX, -1.0f, uv.startU, uv.endV),
				new TexturedRectangle.Vertex(pos.endX,   -1.0f, uv.endU  , uv.startV),
				new TexturedRectangle.Vertex(pos.endX,   -1.0f, uv.endU  , uv.endV)
			}, 6, vboDataOffset);
		}
	}
}