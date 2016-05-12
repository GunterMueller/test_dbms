using test_dbms.src.server;
namespace test_dbms.src.file
{
    /*用于存储磁盘块在内存中的内容，页可以被当作是一组大小为BLOCK_SIZE的比特流
     */
    public class Page
    {
        private byte[] contents = new byte[BLOCK_SIZE];
        private FileMgr filemgr = SimpleDB.fileMgr();//FileMgr类的filemgr对象通过服务器（server）返回一个静态变量的FileMgr对象
        private object threadLock = new object();

        public const int BLOCK_SIZE = 512;//定义块的大小512B，便于测试包含许多块的数据库，实际大小一般是4KB
        public const int INT_SIZE = sizeof(int) / sizeof(byte);//一个整数4个字节
        /*C#中char变量永远是2个字节，详见MSDN值类型*/
        public static int STR_SIZE(int n)
        {//一个包含n个字符的字符串的字节数
            return INT_SIZE + n * sizeof(char);
        }

        public void read(Block blk)
        {
            lock (threadLock)
            {//调用internal程序集中的read方法来读取整个块的内容
                filemgr.read(blk, contents);
            }
        }

        public void write(Block blk)
        {
            lock(threadLock)
            {
                filemgr.write(blk, contents);
            }
        }

        public Block append(string filename)
        {
            lock(threadLock)
            {
                return filemgr.append(filename, contents);
            }
        }

        public int getInt(int offset)
        {//获取指定偏移量offset处的整数值，一共是4个字节单元做“或运算”，小字节序
            lock(threadLock)
            {
                return (int)(contents[offset] | contents[offset + 1] << 8 | contents[offset + 2] << 16 | contents[offset] << 24);
            }
        }

        public void setInt(int offset, int val)
        {//向页中指定偏移量处写一个值
            lock(threadLock)
            {//小字节序、小端模式存放
                contents[offset] = (byte)val;
                contents[offset + 1] = (byte)(val >> 8);
                contents[offset + 2] = (byte)(val >> 16);
                contents[offset + 3] = (byte)(val >> 24);
            }
        }

        public string getString(int offset)
        {//获取页中指定偏移量出的字符串
            lock(threadLock)
            {
                int len = getInt(offset);
                byte[] byteval = new byte[len];
                for (int i = 0; i < len; i++)
                    byteval[i] = contents[offset + 4 + i];
                return System.Text.Encoding.Default.GetString(byteval);
            }
        }

        public void setString(int offset, string val)
        {//向页中指定偏移量处写一个字符串
            lock(threadLock)
            {
                byte[] byteval = System.Text.Encoding.Default.GetBytes(val);
                setInt(offset, byteval.Length);
                for (int i = 0; i < byteval.Length; i++)
                    contents[offset + 4 + i] = byteval[i];
            }
        }
    }
}