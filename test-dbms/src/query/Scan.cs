
namespace test_dbms.src.query
{
    public interface Scan
    {
        //将扫描scan定位到第一条记录之前
        void beforeFirst();

        //移动扫描scan到下一位
        bool next();

        //关闭扫描scan和它的子扫描（如果有的话）
        void close();

        //返回当前记录中指定字段的值 —— 常量
        Constant getVal(string fldname);

        //返回当前记录中指定整数字段的值 —— 整数
        int getInt(string fldname);

        //返回当前记录中指定字段字符串的值 —— 字符串
        string getString(string fldname);
        
        //如果scan有指定的字段名称，则返回true
        bool hasField(string fldname);

    }
}
