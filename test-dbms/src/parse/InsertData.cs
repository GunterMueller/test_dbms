using System.Collections.Generic;
using test_dbms.src.query;

namespace test_dbms.src.parse
{
    public class InsertData
    {// "insert into tblname(fld1,fld2...) values ('string',int...)"数据：表名称、字段名称、值
        private string tblname;
        private List<string> flds;
        private List<Constant> i_vals;

        public InsertData(string tblname, List<string> flds, List<Constant> vals)
        {
            this.tblname = tblname;
            this.flds = flds;
            this.i_vals = vals;
        }

        public string tableName()
        {//返回受影响的表名称
            return tblname;
        }

        public List<string> fields()
        {//返回受影响的字段名称
            return flds;
        }

        public List<Constant> vals()
        {//返回更改值的列表，与上述字段名称有一对一的对应
            return i_vals;
        }

    }
}
