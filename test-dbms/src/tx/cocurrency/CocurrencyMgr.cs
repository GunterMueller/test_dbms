using System.Collections.Generic;
using test_dbms.src.file;

namespace test_dbms.src.tx.cocurrency
{
    class CocurrencyMgr
    {/* 用static维护一个全局得到的LockTable，所有事务共享这一个LockTable
      * 维护了一个<Block,string>的字典，标记block上是什么锁
      */ 
        private static LockTable locktbl = new LockTable();
        private Dictionary<Block, string> locks = new Dictionary<Block, string>();

        private bool hasXLock(Block blk)
        {//这个方法可以先判断当前是否有锁，然后再判断是否为 X 锁
            return locks.ContainsKey(blk) ? locks[blk].Equals("X") : false;
        }

        public void sLock(Block blk)
        {
            if(!locks.ContainsKey(blk))
            {
                locktbl.sLock(blk);
                locks.Add(blk, "S");
            }
        }

        public void xLock(Block blk)
        {//若当前只有一个sLock会总是竞争不过xLock的，有可能导致饥饿，所以已经设置MAX_TIME来确定阈值，超过的直接抛异常
            if(!hasXLock(blk))
            {//如果没有 X 锁的话
                sLock(blk);//先上 S 锁
                locktbl.xLock(blk);//再上 X 锁
                locks[blk] = "X";
            }
        }

        public void release()
        {
            foreach (Block blk in locks.Keys)
                locktbl.unlock(blk);//因为S锁的值最多为1，X锁的值为-1，所以unlock只能执行else部分，能全部释放掉
            locks.Clear();
        }

    }
}
