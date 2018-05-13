namespace Scene
{
	public class GameObject
	{
		public GameObject()
		{
			Start();
		}
		public virtual void Draw() {}
		public virtual void Start(){}
		public virtual void Update(){}
		public virtual void FixedApdate(){}
		public virtual void OnApplicationClosing(){}
	}
}