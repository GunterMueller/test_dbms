using test_dbms.src.tx;
using test_dbms.src.record;
using test_dbms.src.query;

namespace test_dbms.src.index.hash
{
    public class HashIndex : Index
    {
        private const int NUM_BUCKETS = 100;//假定100个桶
        private string idxname;
        private Schema sch;
        private Transaction tx;
        private Constant searchkey = null;
        private TableScan ts = null;

        public HashIndex(string idxname, Schema sch, Transaction tx)
        {//构造函数，为指定索引名称的 index 打开一个哈希索引
            this.idxname = idxname;
            this.sch = sch;
            this.tx = tx;
        }

        public void beforeFirst(Constant searchkey)
        {//将索引定位在包含指定搜索键的第一条索引记录前
            close();
            this.searchkey = searchkey;
            int bucket = searchkey.GetHashCode() % NUM_BUCKETS;//根据searchkey的大小判断当前在哪一个桶
            string tblname = idxname + bucket;
            TableInfo ri = new TableInfo(tblname, sch);
            ts = new TableScan(ri, tx);
        }

        public bool next()
        {//移动到包含搜索键的下一条记录，这个方法循环检查桶，来找到符合要求的记录
            while(ts.next())
            {
                if (ts.getVal("dataval").Equals(searchkey))
                    return true;
            }
            return false;
        }

        public RID getDataRid()
        {
            int blknum = ts.getInt("block");
            int id = ts.getInt("id");
            return new RID(blknum, id);
        }

        public void insert(Constant val, RID rid)
        {//向TableScan相应桶中插入一条新纪录
            beforeFirst(val);
            ts.insert();
            ts.setInt("block", rid.blockNumber());
            ts.setInt("id", rid.Id());
            ts.setVal("dataval", val);
        }

        public void delete(Constant val, RID rid)
        {//删除 TableScan 中相应桶中指定的记录
            beforeFirst(val);
            while(next())
            {
                if(getDataRid().Equals(rid))
                {
                    ts.delete();
                    return;
                }
            }
        }

        public void close()
        {//通过关闭表扫描，来关闭索引
            if (ts != null)
                ts.close();
        }

        public static int searchCost(int numblocks, int rpb)
        {//返回搜索一个指定块数的index文件的代价，假设所有的桶都是一样大小，所以代价就是桶的大小
            return numblocks / NUM_BUCKETS;
        }/* numblocks：the number of blocks of index records
          * rpb：the number of records per block
          */

    }
}
