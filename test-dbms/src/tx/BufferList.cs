using System.Collections.Generic;
using test_dbms.src.file;
using test_dbms.src.buffer;
using test_dbms.src.server;

namespace test_dbms.src.tx
{
    class BufferList
    {//管理当前事务正被绑定着的缓冲片
        //buffers保存当前事务关联的Buffer
        private Dictionary<Block, Buffer> buffers = new Dictionary<Block, Buffer>();
        //pins保存当前事务涉及的磁盘块
        private List<Block> pins = new List<Block>();
        private BufferMgr bufferMgr = SimpleDB.bufferMgr();

        public Buffer getBuffer(Block blk)
        {//返回正绑定着指定block的缓冲片
            return buffers.ContainsKey(blk) ? buffers[blk] : null;
        }

        public void pin(Block blk)
        {//让缓冲区绑定一个块，并且在内部字典一直维护这个缓冲区和block的联系
            Buffer buff = bufferMgr.pin(blk);
            if (!buffers.ContainsKey(blk))
                buffers.Add(blk, buff);
            pins.Add(blk);
        }

        public Block pinNew(string filename, PageFormatter fmtr)
        {//将一个新的block添加到指定文件，然后绑定到缓冲区
            Buffer buff = bufferMgr.pinNew(filename, fmtr);
            Block blk = buff.block();
            buffers.Add(blk, buff);
            pins.Add(blk);
            return blk;
        }

        public void unpin(Block blk)
        {//释放指定的block
            Buffer buff = buffers[blk];
            bufferMgr.unpin(buff);
            pins.Remove(blk);
            if (!pins.Contains(blk))//当前pins里的这个block没有被其他缓冲区绑定的时候
                buffers.Remove(blk);//就从字典里删除这个blk的所有信息
        }

        public void unpinAll()
        {//释放所有被当前事务所绑定的缓冲区
            foreach(Block blk in pins)
            {
                Buffer buff = buffers[blk];
                bufferMgr.unpin(buff);
            }
            buffers.Clear();
            pins.Clear();
        }
    }
}
