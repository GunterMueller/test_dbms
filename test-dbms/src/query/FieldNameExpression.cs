using test_dbms.src.record;

namespace test_dbms.src.query
{
    public class FieldNameExpression : Expression
    {
        private string fldname;

        public FieldNameExpression(string fldname)
        {//通过字段名称创建一个新的表达式
            this.fldname = fldname;
        }

        public bool isConstant()
        {
            return false;
        }

        public bool isFieldName()
        {
            return true;
        }

        public Constant asConstant()
        {//此函数永远不会被调用，若调用则抛异常
            throw new System.InvalidCastException();
        }

        public string asFieldName()
        {
            return fldname;
        }

        public Constant evaluate(Scan s)
        {
            return s.getVal(fldname);
        }

        public bool appliesTo(Schema sch)
        {
            return sch.hasField(fldname);
        }

        public override string ToString()
        {
            return fldname;
        }

    }
}
