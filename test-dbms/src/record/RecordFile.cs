using test_dbms.src.file;
using test_dbms.src.tx;

namespace test_dbms.src.record
{
    public class RecordFile
    {/* RecordPage只负责对一个数据块内容的访问，RecordFile则是对整个表记录文件的遍历和访问
      * RecordFile被访问时，不是一次读进来整个文件，而是分成若干个Block，一个块一个块的读
      * 这样在每次访问一个Block的时候，就需要一个RecordPage来访问Block
      */
        private TableInfo ti;
        private Transaction tx;
        private string filename;
        private RecordPage rp;
        private int currentblknum;

        private void moveTo(int b)
        {//移动到某个块号b的块处，创建一个rp来读取这个被格式化的块
            if (rp != null)
                rp.close();
            currentblknum = b;
            Block blk = new Block(filename, currentblknum);
            rp = new RecordPage(blk, ti, tx);
        }

        private bool atLastBlock()
        {
            return currentblknum == tx.size(filename) - 1;
        }

        private void appendBlock()
        {//格式化块、绑定文件、取一个块到BufferList缓冲区
            RecordFormatter fmtr = new RecordFormatter(ti);
            tx.append(filename, fmtr);
        }

//--------------------------------------------------------------------------- 

        public RecordFile(TableInfo ti, Transaction tx)
        {
            this.ti = ti;
            this.tx = tx;
            filename = ti.fileName();//"表名 + .tbl"
            if (tx.size(filename) == 0)
                appendBlock();
            moveTo(0);
        }

        public void close()
        {
            rp.close();
        }

        public void beforeFirst()
        {
            moveTo(0);
        }

        public bool next()
        {
            while(true)
            {
                if (rp.next())
                    return true;//找到下一个 INUSE 值的记录，则返回true
                if (atLastBlock())
                    return false;
                moveTo(currentblknum + 1);
            }
        }

        public int getInt(string fldname)
        {
            return rp.getInt(fldname);
        }

        public string getString(string fldname)
        {
            return rp.getString(fldname);
        }

        public void setInt(string fldname, int val)
        {
            rp.setInt(fldname, val);
        }

        public void setString(string fldname, string val)
        {
            rp.setString(fldname, val);
        }

        public void delete()
        {
            rp.delete();
        }

        public void insert()
        {
            while(!rp.insert())
            {//若当前块没有可以插入的地方（没有EMPTY值的记录），
                if (atLastBlock())
                    appendBlock();//若是最后一块，则再读取一个块到缓冲区中
                moveTo(currentblknum + 1);//否则移动到下一个块
            }
        }

        public void moveToRid(RID rid)
        {//移动到指定RID记录处：需要块号和当前块下记录的ID
            moveTo(rid.blockNumber());
            rp.moveToId(rid.Id());
        }

        public RID currentRid()
        {//返回当前RID对象
            int id = rp.currentId();
            return new RID(currentblknum, id);
        }

    }
}
