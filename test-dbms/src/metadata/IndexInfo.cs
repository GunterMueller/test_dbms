using test_dbms.src.file;
using test_dbms.src.record;
using test_dbms.src.tx;
using test_dbms.src.server;
using test_dbms.src.index;
using test_dbms.src.index.hash;

namespace test_dbms.src.metadata
{
    public class IndexInfo
    {
        private string idxname, fldname;
        private Transaction tx;
        private TableInfo ti;
        private StatInfo si;

        private Schema schema()
        {
            Schema sch = new Schema();
            sch.addIntField("block");
            sch.addIntField("id");
            if (ti.schema().type(fldname) == Schema.INTEGER)
                sch.addIntField("dataval");
            else
            {
                int fldlen = ti.schema().length(fldname);
                sch.addStringField("dataval", fldlen);
            }
            return sch;
        }

//---------------------------------------------------------------------------------------
        public IndexInfo(string idxname, string tblname, string fldname, Transaction tx)
        {
            this.idxname = idxname;
            this.fldname = fldname;
            this.tx = tx;
            ti = SimpleDB.mdMgr().getTableInfo(tblname, tx);
            si = SimpleDB.mdMgr().getStatInfo(tblname, ti, tx);
        }

        public Index open()
        {
            Schema sch = schema();
            return new HashIndex(idxname, sch, tx);
        }

        public int blockAccessed()
        {
            int rpb = Page.BLOCK_SIZE / ti.recordLength();
            int numblocks = si.recordsOutput() / rpb;
            return HashIndex.searchCost(numblocks, rpb);
        }

        public int recordsOutput()
        {
            return si.recordsOutput() / si.distinctValues(fldname);
        }

        public int distinctValues(string fname)
        {
            if (fldname.Equals(fname))
                return 1;
            else
                return System.Math.Min(si.distinctValues(fldname), recordsOutput());
        }

    }
}
