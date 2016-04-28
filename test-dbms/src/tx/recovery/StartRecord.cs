using test_dbms.src.log;

namespace test_dbms.src.tx.recovery
{
    class StartRecord : LogRecord
    {
        private int txnum;

        public StartRecord(int txnum)
        {
            this.txnum = txnum;
        }

        public StartRecord(BasicLogRecord rec)
        {
            txnum = rec.nextInt();
        }

        public override int writeToLog()
        {
            object[] rec = new object[] { START, txnum };
            return logMgr.append(rec);
        }

        public override int op()
        {
            return START;
        }

        public override int txNumber()
        {
            return txnum;
        }
        //不做任何操作，因为一个StartRecord没有undo信息
        public override void undo(int txnum) { }

        public override string ToString()
        {
            return "<START " + txnum + ">";
        }

    }
}
