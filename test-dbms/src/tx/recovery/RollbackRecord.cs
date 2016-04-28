using test_dbms.src.log;

namespace test_dbms.src.tx.recovery
{
    class RollbackRecord : LogRecord
    {
        private int txnum;

        public RollbackRecord(int txnum)
        {
            this.txnum = txnum;
        }

        public RollbackRecord(BasicLogRecord rec)
        {
            txnum = rec.nextInt();
        }

        public override int writeToLog()
        {
            object[] rec = new object[] { ROLLBACK, txnum };
            return logMgr.append(rec);
        }

        public override int op()
        {
            return ROLLBACK;
        }

        public override int txNumber()
        {
            return txnum;
        }
        //不做任何操作，因为一个StartRecord没有undo信息
        public override void undo(int txnum) { }

        public override string ToString()
        {
            return "<ROLLBACK " + txnum + ">";
        }

    }
}
