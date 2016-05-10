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

        public StatMgr(TableMgr tblMgr, Transaction tx)
        {
            this.tblMgr = tblMgr;
            refreshStatistics(tx);
        }

        public StatInfo getStatInfo(string tblname, TableInfo ti, Transaction tx)
        {
            lock(threadLock)
            {
                numcalls++;
                if (numcalls > 100)//100次就更新一下<表名称,统计信息>的tablestats
                    refreshStatistics(tx);

                StatInfo si;
                if(!tablestats.ContainsKey(tblname))
                {
                    si = calcTableStats(ti, tx);
                    tablestats.Add(tblname, si);
                }
                else
                {
                    si = tablestats[tblname];
                }
                return si;
            }
        }
        private StatInfo calcTableStats(TableInfo ti, Transaction tx)
        {
            lock(threadLock)
            {
                int numRecs = 0;
                RecordFile rf = new RecordFile(ti, tx);
                int numblocks = 0;
                while(rf.next())
                {
                    numRecs++;
                    numblocks = rf.currentRid().blockNumber() + 1;
                }
                rf.close();
                return new StatInfo(numblocks, numRecs);
            }
        }

        private void refreshStatistics(Transaction tx)
        {
            lock(threadLock)
            {
                tablestats = new Dictionary<string, StatInfo>();
                numcalls = 0;
                TableInfo tcatmd = tblMgr.getTableInfo("tblcat", tx);//获取tblcat表中的信息
                RecordFile tcatfile = new RecordFile(tcatmd, tx);//利用这些信息新建一个RecordFile读取类
                while(tcatfile.next())
                {//获取每一张表的字段信息，计算出块数和记录数，添加到统计信息管理器中去
                    string tblname = tcatfile.getString("tblname");
                    TableInfo md = tblMgr.getTableInfo(tblname, tx);
                    StatInfo si = calcTableStats(md, tx);
                    tablestats.Add(tblname, si);
                }
                tcatfile.close();
            }
        }

    }
}
