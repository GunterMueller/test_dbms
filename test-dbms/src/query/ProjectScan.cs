using System;
using System.Collections.Generic;

namespace test_dbms.src.query
{
    public class ProjectScan : Scan
    {//“投影”关系代数操作符的scan类，除了hasField以外的其他方法都将它们的工作交给底层scan处理
        private Scan s;
        private ICollection<string> fieldlist;

        public ProjectScan(Scan s, ICollection<string> fieldlist)
        {//构造一个有指定底层scan和字段名称列表的“投影”扫描
            this.s = s;
            this.fieldlist = fieldlist;
        }

        public void beforeFirst()
        {
            s.beforeFirst();
        }

        public bool next()
        {
            return s.next();
        }

        public void close()
        {
            s.close();
        }

        public Constant getVal(string fldname)
        {
            if (hasField(fldname))
                return s.getVal(fldname);
            else
                throw new Exception("field " + fldname + " not found.");
        }

        public int getInt(string fldname)
        {
            if (hasField(fldname))
                return s.getInt(fldname);
            else
                throw new Exception("field " + fldname + " not found.");
        }

        public string getString(string fldname)
        {
            if (hasField(fldname))
                return s.getString(fldname);
            else
                throw new Exception("field " + fldname + " not found.");
        }

        public bool hasField(string fldname)
        {
            return fieldlist.Contains(fldname);
        }

    }
}
