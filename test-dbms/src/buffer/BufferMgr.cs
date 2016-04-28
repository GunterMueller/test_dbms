using System;
using System.Threading;
using test_dbms.src.file;

namespace test_dbms.src.buffer
{
    public class BufferMgr
    {/* 公共可访问的缓冲区管理器，包括了BasicBufferMgr，提供相同的方法，区别在于pin和pinNew不会返回空
      */
        private const int MAX_TIME = 10000;//10s
        private BasicBufferMgr bbm;
        private object threadLock = new object();

        private bool waitingTooLong(long starttime)
        {
            return (DateTime.Now.Ticks - starttime) / 10000 > MAX_TIME;
        }

        public BufferMgr(int numbuffers)
        {
            bbm = new BasicBufferMgr(numbuffers);
        }

        public Buffer pin(Block blk)
        {
            lock(threadLock)
            {
                try
                {
                    long timestamp = DateTime.Now.Ticks;
                    Buffer buff = bbm.pin(blk);
                    while (buff == null && !waitingTooLong(timestamp))
                    {
                        Monitor.Wait(threadLock, MAX_TIME);
                        buff = bbm.pin(blk);
                    }
                    if (buff == null)
                        throw new BufferAbortException();
                    return buff;
                }catch(ThreadInterruptedException)
                {
                    throw new BufferAbortException();
                }
            }
        }

        public Buffer pinNew(string filename, PageFormatter fmtr)
        {
            lock(threadLock)
            {
                try
                {
                    long timestamp = DateTime.Now.Ticks;
                    Buffer buff = bbm.pinNew(filename, fmtr);
                    while (buff == null && !waitingTooLong(timestamp))
                    {
                        Monitor.Wait(threadLock, MAX_TIME);
                        buff = bbm.pinNew(filename, fmtr);
                    }
                    if (buff == null)
                        throw new BufferAbortException();
                    return buff;
                }catch(ThreadInterruptedException)
                {
                    throw new BufferAbortException();
                }
            }
        }

        public void unpin(Buffer buff)
        {//解锁指定buffer，若当前buffer的pin数为0，则通知等待列表上的所有的线程
            lock(threadLock)
            {
                bbm.unpin(buff);
                if (!buff.isPinned())
                    Monitor.PulseAll(threadLock);
            }
        }

        public void flushAll(int txnum)
        {
            bbm.flushAll(txnum);
        }

        public int available()
        {
            return bbm.available();
        }

    }
}
