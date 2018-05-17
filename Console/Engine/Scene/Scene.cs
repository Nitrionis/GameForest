using System.Collections.Generic;
using Scene;

namespace Game
{
	public abstract class Scene
	{
		protected List<GameObject> objects = new List<GameObject>();

		public void Instantiate(GameObject obj)
		{
			objects.Add(obj);
		}
		public virtual void Draw()
		{
			foreach (var obj in objects)
				obj.Draw();
		}
		public virtual void Update()
		{
			foreach (var obj in objects)
				obj.Update();
		}
		public virtual void FixedUpdate()
		{
			foreach (var obj in objects)
				obj.FixedApdate();
		}
		public virtual void OnApplicationClosing()
		{
			foreach (var obj in objects)
				obj.OnApplicationClosing();
		}
	}
}