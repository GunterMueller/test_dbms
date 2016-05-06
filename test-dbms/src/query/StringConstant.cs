
namespace test_dbms.src.query
{
    public class StringConstant : Constant
    {
        private string val;

        public StringConstant(string s)
        {
            val = s;
        }

        public object asCsharpVal()
        {
            return val;
        }

        public int CompareTo(Constant c)
        {
            StringConstant sc = (StringConstant)c;
            return val.CompareTo(sc.val);
        }

        public override bool Equals(object obj)
        {
            StringConstant sc = (StringConstant)obj;
            return sc != null && val.Equals(sc.val);
        }

        public override int GetHashCode()
        {
            return val.GetHashCode();
        }

        public override string ToString()
        {
            return "'" + val + "'";
        }

    }
}
