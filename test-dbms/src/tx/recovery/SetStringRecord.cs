using test_dbms.src.file;
using test_dbms.src.log;
using test_dbms.src.buffer;
using test_dbms.src.server;

namespace test_dbms.src.tx.recovery
{
    class SetStringRecord : LogRecord
    {
        private int txnum, offset;
        private string val;
        private Block blk;

        public SetStringRecord(int txnum, Block blk, int offset, string val)
        {
            this.txnum = txnum;
            this.blk = blk;
            this.offset = offset;
            this.val = val;
        }

        public SetStringRecord(BasicLogRecord rec)
        {
            txnum = rec.nextInt();
            string filename = rec.nextString();
            int blknum = rec.nextInt();
            blk = new Block(filename, blknum);
            offset = rec.nextInt();
            val = rec.nextString();
        }

        public override int writeToLog()
        {
            object[] rec = new object[] { SETSTRING, txnum, blk.fileName(), blk.number(), offset, val };
            return logMgr.append(rec);
        }

        public override int op()
        {
            return SETSTRING;
        }

        public override int txNumber()
        {
            return txnum;
        }

        public override string ToString()
        {
            return "<SETSTRING " + txnum + " " + blk + " " + offset + " " + val + ">";
        }

        public override void undo(int txnum)
        {
            BufferMgr buffMgr = SimpleDB.bufferMgr();
            Buffer buff = buffMgr.pin(blk);
            buff.setString(offset, val, txnum, -1);
            buffMgr.unpin(buff);
        }

    }
}
