
namespace test_dbms.src.query
{
    public class IntConstant : Constant
    {
        private int val;

        public IntConstant(int n)
        {//通过一个整数值，创建一个整数常量
            val = n;
        }

        object asCsharpVal()
        {
            return val;
        }

        public int CompareTo(Constant c)
        {//将当前常量的值与指定的常量c的值进行比较，返回它们的相对值的表示
            IntConstant ic = (IntConstant)c;
            return val.CompareTo(ic.val);
        }

        public override bool Equals(object obj)
        {//判断指定obj对象是否与当前常量相等
            IntConstant ic = (IntConstant)obj;
            return ic != null && val.Equals(ic.val);
        }

        public override int GetHashCode()
        {
            return val.GetHashCode();
        }

        public override string ToString()
        {
            return val.ToString();
        }

    }
}
