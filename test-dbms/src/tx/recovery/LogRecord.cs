using test_dbms.src.server;
using test_dbms.src.log;

namespace test_dbms.src.tx.recovery
{
    abstract class LogRecord
    {//接口，由六种不同类型的日志记录来实现
        //6种不同的日志记录 —— 检查点，开始，提交，回滚，设置整数，设置字符串
        public const int CHECKPOINT = 0, START = 1, COMMIT = 2, ROLLBACK = 3, SETINT = 4, SETSTRING = 5;
        //test_dbms创建的日志管理器
        public static readonly LogMgr logMgr = SimpleDB.logMgr();
        //将记录写到日志中，并将日志号（即块号）返回
        public abstract int writeToLog();
        //返回日志记录的类型（0-5）
        public abstract int op();
        //返回与日志记录一起被存储的事务的ID
        public abstract int txNumber();
        //撤销undo被日志记录所编码的操作，只有SETINT和SETSTRING
        public abstract void undo(int txnum);

    }
}
