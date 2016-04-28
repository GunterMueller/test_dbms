using System.Collections;
using System.Collections.Generic;
using test_dbms.src.file;

namespace test_dbms.src.log
{
    //能够移以逆序的方式移动日志文件中的记录（内容）
    internal class LogIterator : IEnumerator<BasicLogRecord>
    {
        private Block blk;
        private Page pg = new Page();
        private Block currentblk;
        private int currentrec;

        private bool hasNext()
        {//确定当前日志记录内容是否是最早的记录
            return currentrec > 0 || currentblk.number() > 0;
        }

        private void moveToNextBlock()
        {//以逆序的方式移动至下一个日志块，并且定位在那个块最后记录的下一位
            currentblk = new Block(currentblk.fileName(), currentblk.number() - 1);
            pg.read(currentblk);
            currentrec = pg.getInt(LogMgr.LAST_POS);
        }

        public LogIterator(Block blk)
        {//为日志文件中的记录创建一个迭代器，指向某日志记录最后的下一位，这个构造函数只能被LogMgr.GetEnumerator调用
            this.blk = blk;
            pg.read(blk);
            Reset();
        }

        public void Reset()
        {//将枚举器设置为它的初始位置
            currentblk = blk;
            currentrec = pg.getInt(LogMgr.LAST_POS);
        }

        public BasicLogRecord Current
        {//获取当前日志记录
            get
            {
                return new BasicLogRecord(pg, currentrec + Page.INT_SIZE);
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public bool MoveNext()
        {
            if(!hasNext())
            {
                return false;
            }
            if(currentrec == 0)
            {
                moveToNextBlock();
            }
            currentrec = pg.getInt(currentrec);
            return true;
        }

        public void Dispose()
        {//执行一些应用级定义的任务比如冻结、释放或者重设置一些未管理的资源
        }

    }
}
