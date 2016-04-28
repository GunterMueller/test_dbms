using test_dbms.src.file;

namespace test_dbms.src.buffer
{
    public interface PageFormatter
    {//一个接口，用来初始化磁盘块的格式，磁盘块上的每一种“类型”都将会有一个实现类

        void format(Page p);//初始化将被写到磁盘块上的page格式，
    }
}
