
namespace test_dbms.src.query
{
    public class ProductScan : Scan
    {//“叉积”关系代数操作符的scan类
        private Scan s1, s2;

        public ProductScan(Scan s1, Scan s2)
        {
            this.s1 = s1;
            this.s2 = s2;
            s1.next();
        }

        public void beforeFirst()
        {
            s1.beforeFirst();
            s1.next();
            s2.beforeFirst();
        }

        public bool next()
        {//将scan s2移动到RHS记录的下一条，如果没有的话，则将scan s1移动到下一条并且将RHS定位到第一条
            if (s2.next())
                return true;
            else
            {
                s2.beforeFirst();
                return s2.next() && s1.next();//没有LHS记录的话就返回false
            }
        }

        public void close()
        {
            s1.close();
            s2.close();
        }

        public Constant getVal(string fldname)
        {
            if (s1.hasField(fldname))
                return s1.getVal(fldname);//若scan s1中有指定的字段名称，则返回值
            else
                return s2.getVal(fldname);
        }

        public int getInt(string fldname)
        {
            if (s1.hasField(fldname))
                return s1.getInt(fldname);
            else
                return s2.getInt(fldname);
        }

        public string getString(string fldname)
        {
            if (s1.hasField(fldname))
                return s1.getString(fldname);
            else
                return s2.getString(fldname);
        }

        public bool hasField(string fldname)
        {
            return s1.hasField(fldname) || s2.hasField(fldname);
        }

    }
}
