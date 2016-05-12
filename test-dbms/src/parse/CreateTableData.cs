using test_dbms.src.record;

namespace test_dbms.src.parse
{
    public class CreateTableData
    {// SQL语句：“create table”需要的数据信息：表名称、模式信息
        private string tblname;
        private Schema sch;

        public CreateTableData(string tblname, Schema sch)
        {//保存表名称、模式信息
            this.tblname = tblname;
            this.sch = sch;
        }

        public string tableName()
        {
            return tblname;
        }

        public Schema newSchema()
        {
            return sch;
        }
    }
}
