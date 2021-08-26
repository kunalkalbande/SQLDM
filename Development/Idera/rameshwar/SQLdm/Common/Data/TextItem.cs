namespace Idera.SQLdm.Common.Data
{
    public class TextItem<T>
    {
        public string Text { get; set; }
        public T Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
