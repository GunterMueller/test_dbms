
namespace test_dbms.src.metadata
{
    public class StatInfo
    {//保存表数据的三种信息：块数、记录数、每个字段的非重复记录数
        private int numBlocks;
        private int numRecs;

        public StatInfo(int numblocks, int numrecs)
        {//字段的区别值不传入此构造函数
            this.numBlocks = numblocks;
            this.numRecs = numrecs;
        }

        public int blockAccessed()
        {//表中块数的估计值
            return numBlocks;
        }

        public int recordsOutput()
        {//表中记录的估计值
            return numRecs;
        }

        public int distinctValues(string fldname)
        {//对指定字段名称返回非重复记录数的估计值，这是一个经验估计值
            return 1 + (numRecs / 3);
        }

    }
}
