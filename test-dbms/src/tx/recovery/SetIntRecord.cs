using test_dbms.src.file;
using test_dbms.src.log;
using test_dbms.src.buffer;
using test_dbms.src.server;

namespace test_dbms.src.tx.recovery
{
    class SetIntRecord : LogRecord
    {//class A:B 表示B是基类，类A继承自父类B
        private int txnum, offset, val;
        private Block blk;

        public SetIntRecord(int txnum,Block blk,int offset,int val)
        {//构造函数，创建一条新的SetIntRecord记录
            this.txnum = txnum;
            this.blk = blk;
            this.offset = offset;
            this.val = val;
        }

        public SetIntRecord(BasicLogRecord rec)
        {//通过一条不带语义的字节记录来创建一条带语义的日志记录
            txnum = rec.nextInt();
            string filename = rec.nextString();
            int blknum = rec.nextInt();
            blk = new Block(filename, blknum);
            offset = rec.nextInt();
            val = rec.nextInt();
        }

        public override int writeToLog()
        {
            object[] rec = new object[] { SETINT, txnum, blk.fileName(), blk.number(), offset, val };
            return logMgr.append(rec);
        }

        public override int op()
        {
            return SETINT;
        }

        public override int txNumber()
        {
            return txnum;
        }

        public override string ToString()
        {
            return "<SETINT " + txnum + " " + blk + " " + offset + " " + val + ">";
        }

        public override void undo(int txnum)
        {
            BufferMgr buffMgr = SimpleDB.bufferMgr();
            Buffer buff = buffMgr.pin(blk);//将块绑定到（读入）缓冲区
            buff.setInt(offset, val, txnum, -1);//LSN为负数，生成一条临时日志记录，不保存到磁盘
            buffMgr.unpin(buff);
        }

    }
}
