using System.Collections.Generic;

namespace Scene
{
	public class Scene
	{
		protected List<GameObject> objects = new List<GameObject>();

		public void Draw()
		{
			foreach (var obj in objects)
			{
				obj.Draw();
			}
		}
		public void Update()
		{
			foreach (var obj in objects)
			{
				obj.Update();
			}
		}
		public void FixedUpdate()
		{
			foreach (var obj in objects)
			{
				obj.FixedApdate();
			}
		}
		public void OnApplicationClosing()
		{
			foreach (var obj in objects)
			{
				obj.OnApplicationClosing();
			}
		}
	}
}