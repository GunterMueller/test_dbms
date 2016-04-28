
namespace test_dbms.src.file
{
    //磁盘块参照，包含两个属性:一个文件名和一个块号，不存储块的内容，块的内容存储在页上
    public class Block
    {
        private string filename;
        private int blknum;

        public Block(string filename, int blknum)
        {
            this.filename = filename;
            this.blknum = blknum;
        }

        public string fileName()
        {
            return filename;
        }
        
        public int number()
        {
            return blknum;
        }

        public override bool Equals(object obj)
        {
            Block blk = (Block)obj;
            return filename.Equals(blk.filename) && (blknum == blk.blknum);
        }

        public override string ToString()
        {
            return "[file: " + filename + " , block: " + blknum + "]";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
