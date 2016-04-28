using test_dbms.src.file;

namespace test_dbms.src.log
{
    //一条日志记录
    public class BasicLogRecord
    {
        private Page pg;
        private int pos;

        public BasicLogRecord(Page pg, int pos)
        {
            this.pg = pg;
            this.pos = pos;
        }

        public int nextInt()
        {
            int result = pg.getInt(pos);
            pos += Page.INT_SIZE;//位置后移
            return result;
        }

        public string nextString()
        {
            string result = pg.getString(pos);
            pos += Page.STR_SIZE(result.Length);
            return result;
        }
    }
}
