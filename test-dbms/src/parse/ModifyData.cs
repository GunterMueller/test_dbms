using System.Collections.Generic;
using test_dbms.src.query;

namespace test_dbms.src.parse
{
    public class ModifyData
    {// "UPDATE tblnae SET fldname = newcal WHERE predicate"语句的数据：表名称、要更改字段的名称和新值、predicate
        private string tblname;
        private string fldname;
        private Expression newval;
        private Predicate i_pred = new Predicate();

        public ModifyData(string tblname, string fldname, Expression newval, Predicate pred)
        {
            this.tblname = tblname;
            this.fldname = fldname;
            this.newval = newval;
            this.i_pred = pred;
        }

        public string tableName()
        {
            return tblname;
        }

        public string targetField()
        {//将要被改变的字段名称
            return fldname;
        }

        public Expression newValue()
        {
            return newval;
        }

        public Predicate pred()
        {
            return i_pred;
        }

    }
}
