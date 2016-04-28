using test_dbms.src.log;

namespace test_dbms.src.tx.recovery
{
    class CheckpointRecord : LogRecord
    {
        //创建一个静态检查点日志
        public CheckpointRecord() { }

        public CheckpointRecord(BasicLogRecord rec) { }

        public override int writeToLog()
        {
            object[] rec = new object[] { CHECKPOINT };
            return logMgr.append(rec);
        }

        public override int op()
        {
            return CHECKPOINT;
        }

        public override int txNumber()
        {
            return -1;
        }
        //不做任何操作，因为一个StartRecord没有undo信息
        public override void undo(int txnum) { }

        public override string ToString()
        {
            return "<CHECKPOINT>";
        }

    }
}
