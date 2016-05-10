using test_dbms.src.record;
using test_dbms.src.tx;

namespace test_dbms.src.metadata
{
    class ViewMgr
    {
        private const int MAX_VIEWDEF = 100;
        TableMgr tblMgr;

        public ViewMgr(bool isNew, TableMgr tblMgr, Transaction tx)
        {
            this.tblMgr = tblMgr;
            if(isNew)
            {
                Schema sch = new Schema();
                sch.addStringField("viewname", TableMgr.MAX_NAME);
                sch.addStringField("viewdef", MAX_VIEWDEF);
                tblMgr.createTable("viewcat", sch, tx);
            }
        }

        public void createView(string vname, string vdef, Transaction tx)
        {//创建视图，在内存中创建RecordFile读取类往里面写信息
            TableInfo ti = tblMgr.getTableInfo("viewcat", tx);
            RecordFile rf = new RecordFile(ti, tx);
            rf.insert();
            rf.setString("viewname", vname);
            rf.setString("viewdef", vdef);
            rf.close();
        }

        public string getViewDef(string vname, Transaction tx)
        {
            string result = null;
            TableInfo ti = tblMgr.getTableInfo("viewcat", tx);
            RecordFile rf = new RecordFile(ti, tx);
            while(rf.next())
            {
                if(rf.getString("viewdef").Equals(vname))
                {
                    result = rf.getString("viewdef");
                    break;
                }
            }
            rf.close();
            return result;
        }

    }
}
