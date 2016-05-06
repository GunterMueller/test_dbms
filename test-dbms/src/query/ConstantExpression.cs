using test_dbms.src.record;

namespace test_dbms.src.query
{
    public class ConstantExpression : Expression
    {
        private Constant val;

        public ConstantExpression(Constant c)
        {//通过抓取一个常量创建一个新的表达式
            val = c;
        }

        public bool isConstant()
        {
            return true;
        }

        public bool isFieldName()
        {
            return false;
        }

        public Constant asConstant()
        {//返回常量值
            return val;
        }

        public string asFieldName()
        {//此函数应该永远都不被调用，如果不小心调用了则抛异常
            throw new System.InvalidCastException();
        }

        public Constant evaluate(Scan s)
        {//忽略scan，直接返回当前scan处的常量值
            return val;
        }

        public bool appliesTo(Schema sch)
        {//常量表达式适用于任何schema
            return true;
        }

        public override string ToString()
        {
            return val.ToString();
        }
    }
}
