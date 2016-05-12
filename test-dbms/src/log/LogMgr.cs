using System.Collections;
using System.Collections.Generic;
using test_dbms.src.file;
using test_dbms.src.server;

namespace test_dbms.src.log
{
    /*底层的日志管理器，负责向日志文件中写记录，日志记录可以是任意整数和字符串的值组合，
      且日志记录被test_dbms.src.tx.recovery.RecoveryMgr读和写*/
    public class LogMgr : IEnumerable<BasicLogRecord>
    {
        internal const int LAST_POS = 0;//当前页中最后整数的位置，0表示指针指向当前页的第一个值（字符串或整数）

        private string logfile;
        private Page mypage = new Page();
        private Block currentblk;
        private int currentpos;
        private object threadLock = new object();
        
        public LogMgr(string logfile)
        {/* 为制定日志文件创建的管理器，如果日志文件不存在，会用一个空的第一个块创建
         * 这个构造函数依赖于一个 FileMgr 的 object 对象（来自server.SimpleDB.filemgr创建于系统初始化时）
         * 所以这个构造函数必须要通过先调用server.SimpleDB.initFileMgr之后才能调用*/
            this.logfile = logfile;
            int logsize = SimpleDB.fileMgr().size(logfile);
            if(logsize == 0)
                appendNewBlock();
            else
            {
                currentblk = new Block(logfile, logsize - 1);
                mypage.read(currentblk);
                currentpos = getLastRecordPosition() + Page.INT_SIZE;
            }
        }

        public void flush(int lsn)
        {
            if (lsn >= currentLSN())
                flush();
        }

        public IEnumerator<BasicLogRecord> GetEnumerator()
        {
            lock(threadLock)
            {
                flush();
                return new LogIterator(currentblk);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock(threadLock)
            {
                flush();
                return new LogIterator(currentblk);
            }
        }

        public int append(object[] rec)
        {//向日志文件中（由多个块组成）添加一t条日志记录（混合序列）
            lock(threadLock)
            {
                int recsize = Page.INT_SIZE;

                foreach (object obj in rec)
                    recsize += size(obj);//调用此类自定义的size方法计算当前混合序列的大小再加上保存一个整数值
                if(currentpos + recsize >= Page.BLOCK_SIZE)
                {
                    flush();
                    appendNewBlock();
                }//若混合序列和整数值大小加上当前指针大小大于一个块，那么将之前的内容写到磁盘上，然后新建一个块
                foreach (object obj in rec)
                    appendVal(obj);//把每个混合序列（一条日志记录）中的obj对象写到新创建的块上
                finalizeRecord();
                return currentLSN();
            }
        }

        private void appendVal(object val)
        {
            if (val is string)
                mypage.setString(currentpos, (string)val);
            else
                mypage.setInt(currentpos, (int)val);
            currentpos += size(val);
        }

        private int size(object val)
        {//计算混合序列中每个val对象的大小，分别调用Page类中对整数值和字符串大小的计算函数
            if (val is string)
            {
                string sval = (string)val;
                return Page.STR_SIZE(sval.Length);
            }
            else
                return Page.INT_SIZE;
        }

        private int currentLSN()
        {//当前日志文件的序列号，也就是当前日志文件中的块号
            return currentblk.number();
        }

        private void flush()
        {//将当前块持久化到磁盘中
            mypage.write(currentblk);
        }

        private void appendNewBlock()
        {//若当前指定文件名的日志文件为空，则向当前日志文件中新添加一个块
            setLastRecordPosition(0);//将LAST_POS的值设置成整数值0
            currentpos = Page.INT_SIZE;//整数值占4字节，currentpos指向4
            //根据FileMgr中的append方法，获取指定日志文件名的日志文件中块的个数(假设4)，然后新建一个块号为4的Block，并将其用bb数组初始化
            currentblk = mypage.append(logfile);
        }

        private void finalizeRecord()
        {//创建一个环形链表，内存页的前4个字节记录了最后一个日志记录的位置
            //在currentpos处设置LAST_POS中的值，即指向上一个记录的末尾
            mypage.setInt(currentpos, getLastRecordPosition());
            //将LAST_POS设置为当前currentpos指向的值，为下一次记录的位置做准备
            setLastRecordPosition(currentpos);
            //currentpos后移4位，因为已经记录了一个整数值
            currentpos += Page.INT_SIZE;
        }

        private int getLastRecordPosition()
        {
            return mypage.getInt(LAST_POS);
        }

        private void setLastRecordPosition(int pos)
        {
            mypage.setInt(LAST_POS, pos);
        }
   
    } 
}
