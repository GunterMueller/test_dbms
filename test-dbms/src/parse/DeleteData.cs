using test_dbms.src.query;

namespace test_dbms.src.parse
{
    public class DeleteData
    {// "delete from tblname where ..."语句的数据：表名称、predicate
        private string tblname;
        private Predicate i_pred = new Predicate();

        public DeleteData(string tblname, Predicate pred)
        {
            this.tblname = tblname;
            this.i_pred = pred;
        }

        public string tableName()
        {
            return tblname;
        }

        public Predicate pred()
        {
            return i_pred;
        }

    }
}
