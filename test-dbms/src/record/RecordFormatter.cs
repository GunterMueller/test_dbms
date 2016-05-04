using test_dbms.src.file;
using test_dbms.src.buffer;

namespace test_dbms.src.record
{
    /*前面描述了test_dbms的存储实现，在buffer文件夹下面有一个PageFormatter的接口，
     * 当初提到说，整个系统只有两个接口实现，其中有一个就是RecordPageFormatter。
     * 实际上说，数据库中的表，不是简单地按照顺序的形式写在文件中，而是有其自身的格式限制。
     * 表文件有着自己的存储格式，新插入记录按照指定的格式写入到表文件中。
     */
    class RecordFormatter : PageFormatter
    {/* 这个类中有一个TableInfo，利用这个TableInfo对象包含的表中的字段信息
      * 在表文件被写入真实数据之前，对其进行格式化*/
        private TableInfo ti;
        
        private void makeDefaultRecord(Page page, int pos)
        {
            foreach(string fldname in ti.schema().fields())
            {//为每一条记录初始化，类型为整数的置为0，为字符串的置为“”
                int offset = ti.offset(fldname);
                if (ti.schema().type(fldname) == Schema.INTEGER)
                    page.setInt(pos + Page.INT_SIZE + offset, 0);
                else
                    page.setString(pos + Page.INT_SIZE + offset, "");
            }
        }
//---------------------------------------------------------------------------        
        public RecordFormatter(TableInfo ti)
        {//为表文件创建一个新的格式化器
            this.ti = ti;
        }

        public void format(Page page)
        {//以定长记录的形式保存记录
            int recsize = ti.recordLength() + Page.INT_SIZE;
            for (int pos = 0; pos + recsize <= Page.BLOCK_SIZE; pos += recsize)
            {
                page.setInt(pos, RecordPage.EMPTY);
                makeDefaultRecord(page, pos);
            }
        }

    }
}
