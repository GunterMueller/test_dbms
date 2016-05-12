using test_dbms.src.record;

namespace test_dbms.src.query
{
    public interface Plan
    {//接口，被每一种类的“查询计划”所实现
        
        //打开一个与当前plan所关联的scan，scan在第一条记录之前
        Scan open();

        //当scan读取完成时，返回一个块访问数的估计值
        int blocksAccessed();

        //返回一个查询结果输出记录数的估计值
        int recordsOutput();

        //为查询结果输出中的指定字段，返回非重复记录数的估计值
        int distinctValues(string fldname);

        //返回查询的schema
        Schema schema();

    }
}
