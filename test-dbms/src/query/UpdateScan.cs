using test_dbms.src.record;

namespace test_dbms.src.query
{
    public interface UpdateScan : Scan
    {//所有“更新”性质的扫描都是通过这个接口实现的
        
        //更改当前记录中指定字段的值
        void setVal(string fldname, Constant val);
        
        //更改当前记录中指定字段的整数值
        void setInt(string fldname, int val);

        //更改当前记录中指定字段的字符串值
        void setString(string fldname, string val);

        //在scan中某处插入一个新的纪录
        void insert();

        //从scan中删除当前记录
        void delete();

        //返回当前记录的RID，包含了blknum，和块中这条记录的ID
        RID getRid();

        //定位scan到指定RID处的记录
        void moveToRid(RID rid);

    }
}
