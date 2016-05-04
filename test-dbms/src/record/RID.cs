
namespace test_dbms.src.record
{
    public class RID
    {//文件中一条表记录的ID：包含在当前文件中的块号，还有在那个块中的记录ID
        private int blknum;
        private int id;

        public RID(int blknum, int id)
        {//构造函数，为指定block,block中指定ID的记录创建一个RID
            this.blknum = blknum;
            this.id = id;
        }

        public int blockNumber()
        {
            return blknum;
        }

        public int Id()
        {
            return id;
        }

        public override bool Equals(object obj)
        {//判断指定obj对象与当前RID对象是否相等
            RID r = (RID)obj;
            return blknum == r.blknum && id == r.id;
        }

        public override string ToString()
        {
            return "[" + blknum + "," + id + "]";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

    }
}
