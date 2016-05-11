using test_dbms.src.record;

namespace test_dbms.src.query
{
    public class ProductPlan : Plan
    {
        private Plan p1, p2;
        private Schema sch = new Schema();

        public ProductPlan(Plan p1, Plan p2)
        {//在查询树中新建一个“叉积”节点（有两个指定的子查询）
            this.p1 = p1;
            this.p2 = p2;
            sch.addAll(p1.schema());//为当前 schema 添加指定计划 p1 的所有模式信息
            sch.addAll(p2.schema());
        }

        public Scan open()
        {
            Scan s1 = p1.open();
            Scan s2 = p2.open();
            return new ProductScan(s1, s2);
        }

        public int blocksAccessed()
        {//计算叉积运算用到的块：B(product(p1,p2)) = B(p1) + R(p1)*B(p2)
            return p1.blockAccessed() + p1.recordsOutput() * p2.blockAccessed();
        }

        public int recordsOutput()
        {//计算叉积运算输出的记录：R(product(p1,p2)) = R(p1)*R(p2)
            return p1.recordsOutput() * p2.recordsOutput();
        }

        public int distinctValues(string fldname)
        {
            if (p1.schema().hasField(fldname))
                return p1.distinctValues(fldname);
            else
                return p2.distinctValues(fldname);
        }

        public Schema schema()
        {//返回叉积运算的schema —— 底层查询的并集
            return sch;
        }

    }
}
