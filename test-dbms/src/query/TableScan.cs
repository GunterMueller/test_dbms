using test_dbms.src.tx;
using test_dbms.src.record;

namespace test_dbms.src.query
{
    public class TableScan : UpdateScan
    {//一个表的扫描类，就是RecordFile对象的包装，大部分方法交给RecordFile类去处理
        private RecordFile rf;
        private Schema sch;

        public TableScan(TableInfo ti, Transaction tx)
        {
            rf = new RecordFile(ti, tx);
            sch = ti.schema();
        }

        public void beforeFirst()
        {
            rf.beforeFirst();
        }

        public bool next()
        {
            return rf.next();
        }

        public void close()
        {
            rf.close();
        }

        public Constant getVal(string fldname)
        {//以常量形式返回指定字段名称的值
            if (sch.type(fldname) == Schema.INTEGER)
                return new IntConstant(rf.getInt(fldname));
            else
                return new StringConstant(rf.getString(fldname));
        }

        public int getInt(string fldname)
        {
            return rf.getInt(fldname);
        }

        public string getString(string fldname)
        {
            return rf.getString(fldname);
        }

        public bool hasField(string fldname)
        {
            return sch.hasField(fldname);
        }

        //实现UpdateScan接口
        public void setVal(string fldname, Constant val)
        {
            if (sch.type(fldname) == Schema.INTEGER)
                rf.setInt(fldname, (int)val.asCsharpVal());
            else
                rf.setString(fldname, (string)val.asCsharpVal());
        }

        public void setInt(string fldname, int val)
        {
            rf.setInt(fldname, val);
        }

        public void setString(string fldname, string val)
        {
            rf.setString(fldname, val);
        }

        public void delete()
        {
            rf.delete();
        }

        public void insert()
        {
            rf.insert();
        }

        public RID getRid()
        {
            return rf.currentRid();
        }

        public void moveTo(RID rid)
        {
            rf.moveToRid(rid);
        }

    }
}
