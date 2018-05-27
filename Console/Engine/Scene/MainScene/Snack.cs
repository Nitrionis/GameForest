using System.Diagnostics;
using Graphics;

namespace Scene
{
	public class Snack : TexturedRectangle
	{
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
	}
}