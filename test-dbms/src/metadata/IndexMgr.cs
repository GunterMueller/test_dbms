using System.Collections.Generic;
using test_dbms.src.record;
using test_dbms.src.tx;

namespace test_dbms.src.metadata
{
    class IndexMgr
    {
        private TableInfo ti;

        public IndexMgr(bool isnew, TableMgr tblMgr, Transaction tx)
        {//构造一个索引管理器，这个构造函数在系统启动时被调用，若数据库是新建的，那么idxcat表就会被创建
            if(isnew)
            {
                Schema sch = new Schema();
                sch.addStringField("indexname", TableMgr.MAX_NAME);
                sch.addStringField("tablename", TableMgr.MAX_NAME);
                sch.addStringField("fieldname", TableMgr.MAX_NAME);
                tblMgr.createTable("idxcat", sch, tx);
            }
            ti = tblMgr.getTableInfo("idxcat", tx);
        }

        public void createIndex(string idxname, string tblname, string fldname, Transaction tx)
        {//在指定表、指定字段上创造一个指定名称的索引
            RecordFile rf = new RecordFile(ti, tx);
            rf.insert();
            rf.setString("indexname", idxname);
            rf.setString("tablename", tblname);
            rf.setString("fieldname", fldname);
            rf.close();
        }

        public Dictionary<string, IndexInfo> getIndexInfo(string tblname, Transaction tx)
        {//返回一个映射，包含了指定表中所有索引的信息
            Dictionary<string, IndexInfo> result = new Dictionary<string, IndexInfo>();
            RecordFile rf = new RecordFile(ti, tx);
            while (rf.next())
            {
                if (rf.getString("tablename").Equals(tblname))
                {
                    string idxname = rf.getString("indexname");
                    string fldname = rf.getString("fieldname");
                    IndexInfo ii = new IndexInfo(idxname, tblname, fldname, tx);
                    result.Add(fldname, ii);
                }
            }
            rf.close();
            return result;
        }

    }
}
