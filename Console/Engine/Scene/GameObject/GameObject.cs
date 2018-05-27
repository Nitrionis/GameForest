namespace Scene
{
	public class GameObject
	{
		public bool active = true;

		public GameObject()
		{
			Start();
		}
		public virtual void Draw(){}
		public virtual void Start(){}
		public virtual void Update(){}
		public virtual void FixedApdate(){}
		public virtual void OnApplicationClosing(){}

		public virtual void MbdDawn(){}
		public virtual void MbdUp(){}
	}
}