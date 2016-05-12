using test_dbms.src.file;
using test_dbms.src.log;
using test_dbms.src.buffer;
using test_dbms.src.tx;
using test_dbms.src.metadata;
using test_dbms.src.planner;

namespace test_dbms.src.server
{
    //这个类提供一些全系统的静态全局变量值，这些值在使用前被“init”方法初始化，还有一些限制性的初始化可以便于调试
    public class SimpleDB
    {
        public static int BUFFER_SIZE = 8;//默认缓冲区大小
        public static string LOG_FILE = "test-dbms.log";//默认日志文件名

        private static FileMgr fm;
        private static LogMgr logm;
        private static BufferMgr bm;
        private static MetadataMgr mdm;

        public static void init(string dirname)
        {
            initFileLogAndBufferMgr(dirname);
            Transaction tx = new Transaction();
            bool isnew = fm.isNew();
            if (isnew)
                System.Console.WriteLine("creating new database...");
            else
            {
                System.Console.WriteLine("recovering existing database...");
                tx.recover();
            }
            initMetadataMgr(isnew, tx);
            tx.commit();         
        }

        public static void initFileMgr(string dirname)
        {
            fm = new FileMgr(dirname);
        }

        public static void initFileAndLogMgr(string dirname)
        {
            initFileMgr(dirname);
            logm = new LogMgr(LOG_FILE);
        }
        
        public static void initFileLogAndBufferMgr(string dirname)
        {
            initFileAndLogMgr(dirname);
            bm = new BufferMgr(BUFFER_SIZE);
        }

        public static void initMetadataMgr(bool isnew, Transaction tx)
        {
            mdm = new MetadataMgr(isnew, tx);
        }

        public static FileMgr fileMgr() { return fm; }

        public static LogMgr logMgr() { return logm; }

        public static BufferMgr bufferMgr() { return bm; }

        public static MetadataMgr mdMgr() { return mdm; }

        public static Planner planner()
        {
            QueryPlanner qplanner = new BasicQueryPlanner();
            UpdatePlanner uplanner = new BasicUpdatePlanner();
            return new Planner(qplanner, uplanner);
        }

    }
}
