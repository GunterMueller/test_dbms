using System.Collections.Generic;

namespace test_dbms.src.tx.recovery
{
    class RecoveryMgr
    {// 恢复管理器，每个事务都由自己的恢复管理器
        private int txnum;

        private void doRollback()
        {
            LogRecordIterator iter = new LogRecordIterator();
        }
    }
}
