using test_dbms.src.tx;
using test_dbms.src.record;
using test_dbms.src.metadata;
using test_dbms.src.server;

namespace test_dbms.src.query
{
    public class TablePlan : Plan
    {
        private Transaction tx;
        private TableInfo ti;
        private StatInfo si;

        public TablePlan(string tblname, Transaction tx)
        {
            this.tx = tx;
            ti = SimpleDB.mdMgr()
        }


    }
}
