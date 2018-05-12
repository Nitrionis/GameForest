using System.Runtime.InteropServices;
using OpenTK.Graphics;

namespace Console
{
	[StructLayout(LayoutKind.Sequential)]
	public struct Vertex
	{
		public float X, Y;
		public float R, G, B;
		public float U, V;

		public Vertex(float x, float y, Color4 color, float u, float v)
		{
			X = x; Y = y;
			R = color.R; G = color.G; B = color.B;
			U = u; V = v;
		}
	}
}