using test_dbms.src.tx;
using test_dbms.src.record;
using test_dbms.src.metadata;
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
        {//根据指定表在查询树种创建一个叶子节点
            this.tx = tx;
            ti = SimpleDB.mdMgr().getTableInfo(tblname, tx);
            si = SimpleDB.mdMgr().getStatInfo(tblname, ti, tx);
        }

        public Scan open()
        {
            return new TableScan(ti, tx);
        }

        public int blocksAccessed()
        {
            return si.blockAccessed();
        }

        public int recordsOutput()
        {
            return si.recordsOutput();
        }

        public int distinctValues(string fldname)
        {
            return si.distinctValues(fldname);
        }

        public Schema schema()
        {
            return ti.schema();
        }

    }
}
