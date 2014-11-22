namespace RDotNet.R.Adapter.Graphics
{
    public class Raster
    {
        private readonly Color[,] _raster;

        public Raster(int width, int height)
        {
            _raster = new Color[width, height];
        }

        public Color this[int x, int y]
        {
            get { return _raster[x, y]; }
            set { _raster[x, y] = value; }
        }

        public int Width
        {
            get { return _raster.GetLength(1); }
        }

        public int Height
        {
            get { return _raster.GetLength(0); }
        }
    }
}
