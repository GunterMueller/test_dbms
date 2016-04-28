using System.Collections;
using System.Collections.Generic;
using test_dbms.src.log;
using test_dbms.src.server;

namespace test_dbms.src.tx.recovery
{
    class LogRecordIterator : IEnumerator<LogRecord>
    {/* 一个提供了从日志文件中逆序读取记录的方法的类，不同于log.LogIterator类只关注混合序列，
        这个类可以解析日志记录的语义信息*/
        private IEnumerator<BasicLogRecord> iter = SimpleDB.logMgr().GetEnumerator();

        public LogRecord Current
        {
            get
            {
                BasicLogRecord rec = iter.Current;

            }
        }


    }
}
