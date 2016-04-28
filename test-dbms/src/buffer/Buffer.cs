using test_dbms.src.file;
using test_dbms.src.server;

namespace test_dbms.src.buffer
{
    /* 系统中其他上层类直接使用对象，也就是说，Log、Transaction等需要读写数据的时候，
     * 首先创建一个Buffer对象，然后利用这个对象来进行读写
     */
    public class Buffer
    {
        private Page contents = new Page();
        private Block blk = null;
        private int pins = 0;//Buffer被绑定的次数
        private int modifiedBy = -1;//页的内容是否被修改过(负数表示未修改)，如果改过，那么对应事务的id和日志记录的序列号是多少
        private int logSequenceNumber = -1;//负数表示没有对应的日志记录

        public int getInt(int offset)
        {//获取当前缓冲区中页->指定偏移量处->整数值
            return contents.getInt(offset);
        }

        public string getString(int offset)
        {//获取当前缓冲区中页->指定偏移量处->字符串
            return contents.getString(offset);
        }

        public void setInt(int offset, int val, int txnum, int lsn)
        {/*向当前缓冲区中的页指定偏移量处写一个整数值，这个方法假设事务已经写了一个正确的日志记录
           缓冲区需要保存事务的id和LSN，若lsn为负表示没有必要将日志记录写回磁盘
          */
            modifiedBy = txnum;
            if(lsn >= 0)
                logSequenceNumber = lsn;
            contents.setString(offset, val);
        }

        public void setString(int offset, string val, int txnum, int lsn)
        {
            modifiedBy = txnum;
            if (lsn >= 0)
                logSequenceNumber = lsn;
            contents.setString(offset, val);
        }

        public Block block()
        {//返回当前缓冲区所固定(pin)磁盘块的一个引用(reference)
            return blk;
        }

        internal void flush()
        {//将Page写回磁盘块，若当前Page为脏，那么先将它的日志记录写回磁盘块，再将page写回
            if(modifiedBy >= 0)
            {
                SimpleDB.logMgr().flush(logSequenceNumber);
                contents.write(blk);
                modifiedBy = -1;//修改的事务id置为负数
            }
        }

        internal void pin()
        {//增加buffer的固定数
            pins++;
        }

        internal void unpin()
        {//减少buffer的固定数
            pins--;
        }

        internal bool isPinned()
        {//若buffer当前正被固定则返回true
            return pins > 0;
        }

        internal bool isModifiedBy(int txnum)
        {//若当前buffer为脏则返回true
            return modifiedBy == txnum;
        }

        internal void assignToBlock(Block b)
        {//将指定块读入buffer的page中，若当前buffer为脏，先将页的内容写回磁盘中
            flush();
            blk = b;
            contents.read(blk);
            pins = 0;
        }

        internal void assignToNew(string filename, PageFormatter fmtr)
        {//按照特定格式初始化buffer中的页，然后将这个页添加到指定文件中，若当前buffer为脏，先将页内容写回磁盘中
            flush();
            fmtr.format(contents);
            blk = contents.append(filename);
            pins = 0;
        }
        /* 新打开的文件，最先被调用的是assignToNew，绑定文件，取该文件一个块到缓冲区；
         * 之后依次从文件块中读数据到缓冲的时候，用的是assignToBlock
         */
    }
}
