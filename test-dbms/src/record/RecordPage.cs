using test_dbms.src.file;
using test_dbms.src.tx;

namespace test_dbms.src.record
{
    class RecordPage
    {/* 管理对应块中数据记录的放置和访问，是一个特殊的Page，略有不同的是RecordPage
      * 不维护contents数组，宏观处理粒度是Record，具体数据读写交给transaction对象来完成*/
        
        //记录的状态有以下几点
        public const int EMPTY = 0, INUSE = 1;

        private Block blk;//当前正在访问的磁盘文件块
        private TableInfo ti;//当前表文件的字段信息
        private Transaction tx;//当前查询操作所在的事务
        private int slotsize;//当前Block中，一条记录实际占用的长度
        private int currentslot = -1;//当前可访问的记录的指针

        private int currentpos()
        {//表示当前指向第几条记录的开始位置
            return currentslot * slotsize;
        }

        private int fieldpos(string fldname)
        {
            int offset = Page.INT_SIZE + ti.offset(fldname);//计算每条记录中该字段的偏移值
            return currentpos() + offset;//结果 = 记录在表中的偏移 + 该字段的偏移值
        }

        private bool isValidSlot()
        {//判断当前指向的第某条记录再往后走一条记录的长度是否会超出块的大小，若是，则说明后面没有记录了
            return currentpos() + slotsize <= Page.BLOCK_SIZE;
        }

        private bool searchFor(int flag)
        {
            currentslot++;
            while(isValidSlot())
            {
                int position = currentpos();
                if (tx.getInt(blk, position) == flag)//若标记位等于INUSE的值则返回true
                    return true;
                currentslot++;//否则标记位为0，继续寻找
            }
            return false;//找不到则说明没有下一条记录
        }
//---------------------------------------------------------------------------
        public RecordPage(Block blk, TableInfo ti, Transaction tx)
        {
            this.blk = blk;
            this.ti = ti;//数据表的元数据
            this.tx = tx;//正在执行操作的事务
            slotsize = ti.recordLength() + Page.INT_SIZE;//一条记录占用的长度：一个标记位加上schema中所有field的长度
            tx.pin(blk);
        }

        public void close()
        {//通过释放块，来关闭管理器
            if (blk != null)
            {
                tx.unpin(blk);
                blk = null;
            }
        }

        public bool next()
        {//寻找下一条标记位为 1 的记录
            return searchFor(INUSE);
        }

        public int getInt(string fldname)
        {
            int position = fieldpos(fldname);
            return tx.getInt(blk, position);
        }

        public string getString(string fldname)
        {
            int position = fieldpos(fldname);
            return tx.getString(blk, position);
        }

        public void setInt(string fldname, int val)
        {
            int position = fieldpos(fldname);
            tx.setInt(blk, position, val);
        }

        public void setString(string fldname, string val)
        {
            int position = fieldpos(fldname);
            tx.setString(blk, position, val);
        }

        public void delete()
        {//用标记删除法，将IS_USED值设置为 0 即可，表示未使用，不删除数据
            int position = currentpos();
            tx.setInt(blk, position, EMPTY);
        }

        public bool insert()
        {
            currentslot = -1;
            bool found = searchFor(EMPTY);//找到第一个“空”（实际上不一定为空）的位置
            if(found)
            {
                int position = currentpos();
                tx.setInt(blk, position, INUSE);
            }
            return found;
        }

        public void moveToId(int id)
        {//设置当前所访问的记录为表中指定ID的某条记录
            currentslot = id;
        }

        public int currentId()
        {//返回当前访问记录的ID
            return currentslot;
        }

    }
}
