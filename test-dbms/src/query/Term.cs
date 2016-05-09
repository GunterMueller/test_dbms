using System;
using test_dbms.src.record;

namespace test_dbms.src.query
{
    public class Term
    {// Term 表示两个expression之间的比较
        private Expression lhs, rhs;//the LHS、RHS expression（left-hand-side）

        public Term(Expression lhs, Expression rhs)
        {
            this.lhs = lhs;
            this.rhs = rhs;
        }

        public int reductionFactor(Plan p)
        {//选择term，能够减少查询结果输出记录数的程度，若为2，说明term减少了输出的一半
            string lhsName, rhsName;
            if (lhs.isFieldName() && rhs.isFieldName())
            {//若两者都是字段名称，那么返回当前非重复记录数较大的
                lhsName = lhs.asFieldName();
                rhsName = rhs.asFieldName();
                return Math.Max(p.distinctValues(lhsName), p.distinctValues(rhsName));
            }

            if(lhs.isFieldName())
            {
                lhsName = lhs.asFieldName();
                return p.distinctValues(lhsName);
            }

            if(rhs.isFieldName())
            {
                rhsName = rhs.asFieldName();
                return p.distinctValues(rhsName);
            }
            //否则term中两个Expression都是常量
            if (lhs.asConstant().Equals(rhs.asConstant()))
                return 1;//查询结果全部输出
            else
                return int.MaxValue;//查询结果为0
        }

        public Constant equatesWithConstant(string fldname)
        {//判断当前term是不是形如“F=c”，如果是，那么此函数返回那个常量
            if (lhs.isFieldName() && lhs.asFieldName().Equals(fldname) && rhs.isConstant())
                return rhs.asConstant();
            else if (rhs.isFieldName() && rhs.asFieldName().Equals(fldname) && lhs.isConstant())
                return lhs.asConstant();
            else
                return null;
        }

        public string equatesWithField(string fldname)
        {//判断当前term是不是形如“F1=F2”，如果是，那么此函数返回那个字段名称
            if (lhs.isFieldName() && lhs.asFieldName().Equals(fldname) && rhs.isFieldName())
                return rhs.asFieldName();
            else if (rhs.isFieldName() && rhs.asFieldName().Equals(fldname) && lhs.isFieldName())
                return lhs.asFieldName();
            else
                return null;
        }


        public bool appliesTo(Schema sch)
        {//两个Expression的字段名称都属于指定schema的话，返回true
            return lhs.appliesTo(sch) && rhs.appliesTo(sch);
        }

        public bool isSatisfied(Scan s)
        {//通过指定scan，两个Expression的值都计算出相同的常量，则返回true
            Constant lhsval = lhs.evaluate(s);
            Constant rhsval = rhs.evaluate(s);
            return rhsval.Equals(lhsval);
        }

        public override string ToString()
        {
            return lhs.ToString() + "=" + rhs.ToString();
        }

    }
}
