
namespace test_dbms.src.parse
{
    public class CreateIndexData
    {//“create index”语句的数据：索引名称、表名称、字段名称
        private string idxname, tblname, fldname;

        public CreateIndexData(string idxname, string tblname, string fldname)
        {
            this.idxname = idxname;
            this.tblname = tblname;
            this.fldname = fldname;
        }

        public string indexName()
        {
            return idxname;
        }

        public string tableName()
        {
            return tblname;
        }

        public string fieldName()
        {
            return fldname;
        }
    }
}
