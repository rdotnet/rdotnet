namespace RDotNet.Client
{
    public class DataFrameRow
    {
        public DataFrameRow(DataFrame frame, int rowIndex)
        {
            DataFrame = frame;
            RowIndex = rowIndex;
        }

        public object this[int index]
        {
            get
            {
                var column = DataFrame[index];
                return column[RowIndex];
            }
            set
            {
                var column = DataFrame[index];
                column[RowIndex] = value;
            }
        }

        internal object GetInnerValue(int index)
        {
            var column = DataFrame[index];
            if (column.IsFactor())
            {
                return column.ToIntegerVector()[RowIndex];
            }

            return column[RowIndex];
        }

        internal void SetInnerValue(int index, object value)
        {
            DynamicVector column = DataFrame[index];
            if (column.IsFactor())
                column.ToIntegerVector()[RowIndex] = (int)value;
            else
                column[RowIndex] = value;
        }

        public object this[string name]
        {
            get
            {
                DynamicVector column = DataFrame[name];
                return column[RowIndex];
            }
            set
            {
                DynamicVector column = DataFrame[name];
                column[RowIndex] = value;
            }
        }

        public DataFrame DataFrame { get; private set; }
        public int RowIndex { get; private set; }
    }
}
