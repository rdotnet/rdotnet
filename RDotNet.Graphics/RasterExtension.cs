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

			var width = raster.Width;
			var height = raster.Height;
			var matrix = new IntegerMatrix(engine, height, width);
			for (var x = 0; x < width; x++)
			{
				for (var y = 0; y < height; y++)
				{
					matrix[x, y] = ToInteger(raster[x, y]);
				}
			}

			return matrix;
		}

		private static int ToInteger(Color color)
		{
			return (color.Alpha << 24) | (color.Blue << 16) | (color.Green << 8) | color.Red;
		}
	}
}
