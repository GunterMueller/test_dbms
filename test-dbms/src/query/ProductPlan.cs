using test_dbms.src.record;

namespace test_dbms.src.query
{
    public class ProductPlan : Plan
    {
        private Plan p1, p2;
        private Schema sch = new Schema();

        public ProductPlan(Plan p1, Plan p2)
        {
            this.p1 = p1;
            this.p2 = p2;
            sch.addAll(p1.schema());//为当前 schema 添加指定计划 p1 的所有模式信息
            sch.addAll(p2.schema());
        }

        public Scan open()
        {
            Scan s1 = p1.open();
            Scan s2 = p2.open();
            return new ProductScan(p1, p2);
        }


    }
}
