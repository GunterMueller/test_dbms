using System.Collections.Generic;
using test_dbms.src.record;

namespace test_dbms.src.query
{
    public class ProjectPlan : Plan
    {
        private Plan p;
        private Schema sch = new Schema();

        public ProjectPlan(Plan p, ICollection<string> fieldlist)
        {
            this.p = p;
            foreach (string fldname in fieldlist)
                sch.add(fldname, p.schema());
        }

        public Scan open()
        {
            Scan s = p.open();
            return new ProjectScan(s, sch.fields());
        }

        public int blocksAccessed()
        {
            return p.blocksAccessed();
        }

        public int recordsOutput()
        {
            return p.recordsOutput();
        }

        public int distinctValues(string fldname)
        {
            return p.distinctValues(fldname);
        }

        public Schema schema()
        {
            return sch;
        }

    }
}
