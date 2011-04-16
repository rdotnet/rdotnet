namespace RDotNet.Graphics
{
	public class Raster
	{
		private Color[,] raster;
		public Color this[int x, int y]
		{
			get
			{
				return raster[x, y];
			}
			set
			{
				raster[x, y] = value;
			}
		}

		public int Width
		{
			get
			{
				return raster.GetLength(1);
			}
		}

		public int Height
		{
			get
			{
				return raster.GetLength(0);
			}
		}

		public Raster(int width, int height)
		{
			raster = new Color[width, height];
		}
	}
}
