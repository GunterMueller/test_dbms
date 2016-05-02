using System.Collections.Generic;
using test_dbms.src.file;
using test_dbms.src.buffer;
using test_dbms.src.server;

namespace test_dbms.src.tx.recovery
{
    class RecoveryMgr
    {// 恢复管理器，每个事务都由自己的恢复管理器
        private int txnum;

        public RecoveryMgr(int txnum)
        {//为指定事务创建一个恢复管理器
            this.txnum = txnum;
            new StartRecord(txnum).writeToLog();//写START日志
        }

        public void commit()
        {//写一个COMMIT记录到日志，然后将这个日志记录写回磁盘
            SimpleDB.bufferMgr().flushAll(txnum);//所有被当前该事务修改过的脏数据页，先写“修改日志”到磁盘，再将脏页写回磁盘
            int lsn = new CommitRecord(txnum).writeToLog();//写一个COMMIT记录到日志
            SimpleDB.logMgr().flush(lsn);//将该日志写回磁盘
        }

        public void rollback()
        {//写一个ROLLBACK日志，然后将这个日志记录写回磁盘
            SimpleDB.bufferMgr().flushAll(txnum);//所有被当前该事务修改过的脏数据页的“修改日志”写回磁盘，再将脏页写回磁盘
            doRollback();//回滚当前事务直到看到START日志记录
            int lsn = new RollbackRecord(txnum).writeToLog();
            SimpleDB.logMgr().flush(lsn);
        }

        public void recover()
        {
            doRecover();
            int lsn = new CheckpointRecord().writeToLog();
            SimpleDB.logMgr().flush(lsn);
        }

        public int setInt(Buffer buff, int offset, int newval)
        {//写一条SETINT日志并且返回LSN，更新到临时文件的值会被记录成假LSN，产生临时日志记录，不会被写回磁盘
            int oldval = buff.getInt(offset);
            Block blk = buff.block();
            if (isTempBlock(blk))
                return -1;
            else
                return new SetIntRecord(txnum, blk, offset, oldval).writeToLog();
        }

        public int setString(Buffer buff, int offset, string newval)
        {
            string oldval = buff.getString(offset);
            Block blk = buff.block();
            if (isTempBlock(blk))
                return -1;
            else
                return new SetStringRecord(txnum, blk, offset, oldval).writeToLog();
        }



        private bool isTempBlock(Block blk)
        {//判断当前块是否来源于临时文件
            return blk.fileName().StartsWith("temp");
        }

        private void doRollback()
        {//从后往前回滚所有和当前事务T相关的日志记录，在磁盘中恢复原状旧值（setint、setstring），直到找到日志记录<START>为止
            LogRecordIterator iter = new LogRecordIterator();
            while(iter.MoveNext())
            {
                LogRecord rec = iter.Current;
                if(rec.txNumber() == txnum)
                {
                    if (rec.op() == LogRecord.START)
                        return;
                    rec.undo(txnum);
                }
            }
        }

        private void doRecover()
        {//数据库的完全恢复操作：找到一个未完成事务的日志记录，就会撤销记录恢复旧值，一直到遇见检查点或者日志结束停止
            List<int> finishedTxs = new List<int>();
            LogRecordIterator iter = new LogRecordIterator();
            while(iter.MoveNext())
            {
                LogRecord rec = iter.Current;
                if (rec.op() == LogRecord.CHECKPOINT)
                    return;//检查点返回
                if (rec.op() == LogRecord.COMMIT || rec.op() == LogRecord.ROLLBACK)
                    finishedTxs.Add(rec.txNumber());//COMMIT、ROLLBACK表示此事务已提交，直接忽略
                else if (!finishedTxs.Contains(rec.txNumber()))
                    rec.undo(txnum);//若当前事务未完成则撤销
            }
        }

    }
}
