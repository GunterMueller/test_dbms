using System;
using System.Collections.Generic;
using System.Threading;
using test_dbms.src.file;

namespace test_dbms.src.tx.cocurrency
{
    class LockTable
    {//维护对块的加锁和解锁（S 锁和 X 锁），所有事务共享一个等待列表
        private const int MAX_TIME = 10000;//10s
        private object threadLock = new object();
        private Dictionary<Block, int> locks = new Dictionary<Block, int>();

        private int getLockVal(Block blk)
        {
            if (locks.ContainsKey(blk))
                return locks[blk];
            locks.Add(blk, 0);
            return 0;
        }

        private bool hasXLock(Block blk)
        {
            return getLockVal(blk) < 0;
        }

        private bool hasOtherSLocks(Block blk)
        {
            return getLockVal(blk) > 1;
        }

        private bool waitingTooLong(long starttime)
        {
            return (DateTime.Now.Ticks - starttime) / 10000 > MAX_TIME;
        }

        public void sLock(Block blk)
        {//给blk加上 S 锁的方法
            lock(threadLock)
            {
                try
                {
                    long timestamp = DateTime.Now.Ticks;
                    while (hasXLock(blk) && !waitingTooLong(timestamp))
                    {//有 X 锁就要等待
                        Monitor.Wait(threadLock, MAX_TIME);
                    }
                    if (hasXLock(blk))//MAX_TIME后还有 X 锁就抛出异常
                        throw new LockAbortException();
                    //没有 X 锁的话就可以加 S 锁，若blk不存在就加blk，并将字典相应值置为0
                    int val = getLockVal(blk);
                    locks[blk] = val + 1;
                }catch(ThreadInterruptedException)
                {
                    throw new LockAbortException();
                }
            }
        }

        public void xLock(Block blk)
        {//给blk加上 X 锁的方法
            lock(threadLock)
            {
                try
                {
                    long timestamp = DateTime.Now.Ticks;
                    while ( hasOtherSLocks(blk) && !waitingTooLong(timestamp))
                    {//读优先加锁机制：若有两个或两个以上的 S 锁，也就意味着读的事务较多，此时不允许加X锁，而是先处理读事务
                        Monitor.Wait(threadLock, MAX_TIME);
                    }
                    if (hasOtherSLocks(blk))
                        throw new LockAbortException();
                    locks[blk] = -1;
                }catch(ThreadInterruptedException)
                {
                    throw new LockAbortException();
                }
            }
        }

        public void unlock(Block blk)
        {
            lock(threadLock)
            {
                int val = getLockVal(blk);
                if (val > 1)
                {//若为两个及以上的 S 锁，locks[blk]-1
                    locks[blk] = val - 1;
                }                
                else
                {//若为 X 锁或者一个 S 锁，直接remove掉当前block，并通知所有等待的线程
                    locks.Remove(blk);
                    Monitor.PulseAll(threadLock);
                }
            }
        }

    }
}
