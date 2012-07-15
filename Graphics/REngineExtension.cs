namespace RDotNet.Graphics
{
	public static class REngineExtension
	{
		public static void Install(this REngine engine, IGraphicsDevice device)
		{
			var adapter = new GraphicsDeviceAdapter(device);
			adapter.SetEngine(engine);
		}
	}
}
