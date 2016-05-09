using System.Collections.Generic;
using test_dbms.src.record;
using test_dbms.src.tx;

namespace test_dbms.src.metadata
{
    class StatMgr
    {/* 统计信息管理器：主要为每个表管理统计信息，这些统计信息不是持久化到磁盘上，
      * 而是在系统启动，新建StatMgr对象的时候就自动计算得到，并且阶段性的刷新它
      */
        private TableMgr tblMgr;
        private Dictionary<string, StatInfo> tablestats;
        private int numcalls;
        private object threadLock = new object();


    }
}
