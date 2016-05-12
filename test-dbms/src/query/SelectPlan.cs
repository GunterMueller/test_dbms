using System;
using test_dbms.src.record;

namespace test_dbms.src.query
{
    public class SelectPlan : Plan
    {
        private Plan p;
        private Predicate pred;

        public SelectPlan(Plan p, Predicate pred)
        {
            this.p = p;
            this.pred = pred;
        }

        public Scan open()
        {
            Scan s = p.open();
            return new SelectScan(s, pred);
        }

        public int blocksAccessed()
        {
            return p.blocksAccessed();
        }

        public int recordsOutput()
        {//估计“选择”运算结果的记录数，由predicate的reduction factor决定
            return p.recordsOutput() / pred.reductionFactor(p);
        }

        public int distinctValues(string fldname)
        {//若fldname是等于一个常量，那么这个值为1；否则就是底层扫描的非重复记录数，但不能超过输出结果大小
            if (pred.equatesWithConstant(fldname) != null)
                return 1;
            else
                return Math.Min(p.distinctValues(fldname), recordsOutput());
        }

        public Schema schema()
        {
            return p.schema();
        }

    }
}
