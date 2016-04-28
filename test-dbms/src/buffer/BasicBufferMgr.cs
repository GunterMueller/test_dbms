using test_dbms.src.file;

namespace test_dbms.src.buffer
{
    internal class BasicBufferMgr
    {
        private Buffer[] bufferpool;
        private int numAvailable;
        private object threadLock = new object();
        
        private Buffer findExistingBuffer(Block blk)
        {//输入一个block的引用，遍历缓冲池的缓冲片，看是否有buffer已经分配给该block
            foreach(Buffer buff in bufferpool)
            {
                Block b = buff.block();//返回当前缓冲区所绑定的磁盘块一个引用
                if (b != null && b.Equals(blk))
                    return buff;
            }
            return null;
        }

        private Buffer chooseUnpinnedBuffer()
        {
            foreach (Buffer buff in bufferpool)
            {
                if (!buff.isPinned())
                    return buff;
            }
            return null;
        }

        internal BasicBufferMgr(int numbuffs)
        {//创建一个缓冲区管理器，管理指定数目的缓冲区
            bufferpool = new Buffer[numbuffs];
            numAvailable = numbuffs;
            for (int i = 0; i < numbuffs; i++)
                bufferpool[i] = new Buffer();
        }

        internal void flushAll(int txnum)
        {//所有被指定事务id修改的脏buffer信息（日志记录、内容）都写回磁盘
            lock(threadLock)
            {
                foreach(Buffer buff in bufferpool)
                {
                    if (buff.isModifiedBy(txnum))
                        buff.flush();
                }
            }
        }

        internal Buffer pin(Block blk)
        {//绑定一个缓冲区到指定block,若已经有缓冲区绑定到这个块了
            lock(threadLock)
            {
                Buffer buff = findExistingBuffer(blk);
                if(buff == null)
                {//当前没有buffer被绑定到输入的blk
                    buff = chooseUnpinnedBuffer();//从缓冲池选择未被绑定的一个buffer
                    if (buff == null)
                        return null;//若没有可用的buffer，则返回空
                    buff.assignToBlock(blk);//若有，则从文件中将此blk读到缓冲区中
                }
                if (!buff.isPinned())//若当前buffer没有绑定，则将其绑定
                    numAvailable--;//可用buffer数-1
                buff.pin();//绑定
                return buff;
            }
        }

        internal Buffer pinNew(string filename, PageFormatter fmtr)
        {//给指定文件分配一个新块，然后为其绑定一个buffer，若没有可用buffer返回空
            lock(threadLock)
            {
                Buffer buff = chooseUnpinnedBuffer();
                if (buff == null)
                    return null;
                buff.assignToNew(filename, fmtr);
                numAvailable--;
                buff.pin();
                return buff;
            }
        }

        internal void unpin(Buffer buff)
        {//解锁指定buffer
            lock(threadLock)
            {
                buff.unpin();
                if (!buff.isPinned())
                    numAvailable++;
            }
        }

        internal int available()
        {
            return numAvailable;
        }

    }
}
