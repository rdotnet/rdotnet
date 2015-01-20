namespace RDotNet.Graphics
{
    public class Raster
    {
        private Color[,] raster;

        public Raster(int width, int height)
        {
            this.raster = new Color[width, height];
        }

        public Color this[int x, int y]
        {
            get { return this.raster[x, y]; }
            set { this.raster[x, y] = value; }
        }

        public int Width
        {
            get { return this.raster.GetLength(1); }
        }

        public int Height
        {
            get { return this.raster.GetLength(0); }
        }
    }
}