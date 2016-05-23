using test_dbms.src.record;

namespace test_dbms.src.query
{
    public class SelectScan : UpdateScan
    {//“选择”关系代数操作符的scan类，除了next以外的其他方法都将它们的工作交给底层scan处理

        private Scan s;
        private Predicate pred;

        public SelectScan(Scan s, Predicate pred)
        {
            this.s = s;
            this.pred = pred;
        }

        //实现Scan中的所有接口
        public void beforeFirst()
        {
            s.beforeFirst();
        }

        public bool next()
        {//判断predicate->Term->Expression中计算Scan的值是否相等
            while (s.next())
                if (pred.isSatisfied(s))
                    return true;
            return false;
        }

        public void close()
        {
            s.close();
        }

        public Constant getVal(string fldname)
        {
            return s.getVal(fldname);
        }

        public int getInt(string fldname)
        {
            return s.getInt(fldname);
        }

        public string getString(string fldname)
        {
            return s.getString(fldname);
        }

        public bool hasField(string fldname)
        {
            return s.hasField(fldname);
        }

        //实现UpdateScan中所有接口
        public void setVal(string fldname, Constant val)
        {
            UpdateScan us = (UpdateScan)s;
            us.setVal(fldname, val);
        }

        public void setInt(string fldname, int val)
        {
            UpdateScan us = (UpdateScan)s;
            us.setInt(fldname, val);
        }

        public void setString(string fldname, string val)
        {
            UpdateScan us = (UpdateScan)s;
            us.setString(fldname, val);
        }

        public void delete()
        {
            UpdateScan us = (UpdateScan)s;
            us.delete();
        }

        public void insert()
        {
            UpdateScan us = (UpdateScan)s;
            us.insert();
        }

        public RID getRid()
        {
            UpdateScan us = (UpdateScan)s;
            return us.getRid();
        }

        public void moveToRid(RID rid)
        {
            UpdateScan us = (UpdateScan)s;
            us.moveToRid(rid);
        }
        
    }
}
