using System;

namespace RDotNet.Graphics
{
	public static class RasterExtension
	{
		public static IntegerMatrix CreateIntegerMatrix(this REngine engine, Raster raster)
		{
			if (engine == null)
			{
				throw new ArgumentNullException("engine");
			}
			if (!engine.IsRunning)
			{
				throw new InvalidOperationException();
			}
			if (raster == null)
			{
				throw new ArgumentNullException("raster");
			}

			int width = raster.Width;
			int height = raster.Height;
			IntegerMatrix matrix = new IntegerMatrix(engine, height, width);
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					matrix[x, y] = (int)raster[x, y];
				}
			}

			return matrix;
		}
	}
}
