using test_dbms.src.file;
using test_dbms.src.buffer;
using test_dbms.src.tx.cocurrency;
using test_dbms.src.tx.recovery;
using test_dbms.src.server;

namespace test_dbms.src.tx
{
    public class Transaction
    {/* 为客户端提供事务管理器，确保每个事务都是可串行化的，可恢复的，并且基本满足ACID特性，
      * Concurrency保证了Consistency(一致性)和Isolation(隔离性)、Recovery保证了Atomicity(原子性)和Durability(持久性)
      * 包含了：并发管理器、恢复管理器、事务ID、下一个事务ID、事务缓冲区管理等
      */ 
        private static int nextTxNum = 0;
        private const int END_OF_FILE = -1;
        private RecoveryMgr recoveryMgr;
        private ConcurrencyMgr concurMgr;
        private int txnum;
        private BufferList myBuffers = new BufferList();
        private static object threadLock = new object();
        
        private static int nextTxNumber()
        {
            lock(threadLock)
            {
                nextTxNum++;
                //System.Console.WriteLine("new transaction: " + nextTxNum);
                return nextTxNum;
            }
        }

        public Transaction()
        {//创建一个新的事务、关联的恢复管理器和并发管理器
            txnum = nextTxNumber();
            recoveryMgr = new RecoveryMgr(txnum);
            concurMgr = new ConcurrencyMgr();
        }

        public void commit()
        {/* 提交当前事务：将所有被该事务修改过的缓冲区页和日志记录写回磁盘；
          * 再将一个COMMIT记录写到日志中；释放所有的锁和绑定的缓冲区*/
            recoveryMgr.commit();
            concurMgr.release();
            myBuffers.unpinAll();
            //System.Console.WriteLine("transaction " + txnum + " committed");
        }

        public void rollback()
        {/* 回滚当前事务：undo所有修改过的值；将所有缓冲区页flush；
          * 将一个ROLLBACK记录flush；释放所有的锁和绑定的缓冲区*/
            recoveryMgr.rollback();
            concurMgr.release();
            myBuffers.unpinAll();
            //System.Console.WriteLine("transaction " + txnum + " rolled back");
        }

        public void recover()
        {/* 将所有被该事务修改过的buffer页flush；从后往前遍历日志记录，回滚所有未提交的事务
          * 最后写一个静态检查点记录到日志；这个函数只在系统启动时、在用户事务前调用*/
            SimpleDB.bufferMgr().flushAll(txnum);
            recoveryMgr.recover();
        }

        public void pin(Block blk)
        {//绑定指定的block
            myBuffers.pin(blk);
        }

        public void unpin(Block blk)
        {//释放指定的block
            myBuffers.unpin(blk);
        }

        public int getInt(Block blk, int offset)
        {//返回指定块指定offset处的整数值，函数一开始在块上加 S 锁，然后通过缓冲区把整数值取回
            concurMgr.sLock(blk);//若当前块有 X 锁就什么都做不了
            Buffer buff = myBuffers.getBuffer(blk);
            return buff.getInt(offset);
        }

        public string getString(Block blk, int offset)
        {//返回指定块指定offset处的字符串，函数一开始在块上加 S 锁，然后通过缓冲区把字符串取回
            concurMgr.sLock(blk);//若当前块有 X 锁就什么都做不了
            Buffer buff = myBuffers.getBuffer(blk);
            return buff.getString(offset);
        }

        public void setInt(Block blk, int offset, int val)
        {/* 向指定块的指定offset处存储一个整数值，加 X 锁，然后将offset处的旧值放入
          * 一个更新的日志记录中，然后写回磁盘；最后让buffer将新的值存储下来，还有LSN和事务ID。
          */ 
            concurMgr.xLock(blk);//若当前块有 X 锁就什么都做不了，有 S 锁的话会升级为 X 锁
            Buffer buff = myBuffers.getBuffer(blk);
            int lsn = recoveryMgr.setInt(buff, offset, val);//将之前旧的值写回日志，val这里没有什么实际作用
            buff.setInt(offset, val, txnum, lsn);//假设这里已经正确写回日志记录到磁盘，然后往缓冲页中写新的值
        }

        public void setString(Block blk, int offset, string val)
        {
            concurMgr.xLock(blk);
            Buffer buff = myBuffers.getBuffer(blk);
            int lsn = recoveryMgr.setString(buff, offset, val);
            buff.setString(offset, val, txnum, lsn);
        }

        public int size(string filename)
        {//返回指定文件的block数目，在要求文件管理器返回数目之前，先加 S 锁到“end of the file”块上
            Block dummyblk = new Block(filename, END_OF_FILE);
            concurMgr.sLock(dummyblk);
            return SimpleDB.fileMgr().size(filename);
        }

        public Block append(string filename, PageFormatter fmtr)
        {//向文件末尾添加一个新的块，返回这个块的引用，在“添加”操作之前先加 X 锁
            Block dummyblk = new Block(filename, END_OF_FILE);
            concurMgr.xLock(dummyblk);
            Block blk = myBuffers.pinNew(filename, fmtr);
            unpin(blk);
            return blk;
        }

    }
}
