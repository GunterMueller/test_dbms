using test_dbms.src.record;
using test_dbms.src.query;

namespace test_dbms.src.index
{
    public interface Index
    {
        //将索引定位在指定“搜索键”的第一条记录
        void beforeFirst(Constant searchkey);

        //移动索引到下一条记录（有上述方法中的“搜索键”）
        bool next();

        //返回存储在当前索引记录中，dataRID的值
        RID getDataRid();

        //插入指定常量dataval和dataRID的索引记录
        void insert(Constant dataval, RID datarid);

        //删除指定常量dataval和dataRID的索引记录
        void delete(Constant dataval, RID datarid);

        //关闭索引
        void close();

    }
}
