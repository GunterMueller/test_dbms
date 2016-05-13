using System.Collections.Generic;
using test_dbms.src.file;

namespace test_dbms.src.record
{
    public class TableInfo
    {//记录数据表和其中记录的元数据（metadata）
        private Schema sch;
        private Dictionary<string, int> offsets;
        private int recordlen;
        private string tblname;

        private int lengthInBytes(string fldname)
        {//以字节数形式返回指定列类型的底层存储实际长度，整数返回INT_SIZE，字符串返回STR_SIZE
            int fldtype = sch.type(fldname);
            if (fldtype == Schema.INTEGER)
                return Page.INT_SIZE;
            else
                return Page.STR_SIZE(sch.length(fldname));//为fldname的字符串留出Page.STR_SIZE的长度
        }

        public TableInfo(string tblname, Schema schema)
        {//创建一个数据表对象，通过表名称和schema，构造函数会计算每个列的偏移量
            this.sch = schema;
            this.tblname = tblname;
            offsets = new Dictionary<string, int>();
            int pos = 0;
            foreach (string fldname in schema.fields())
            {//表被创建时，在偏移量表中记录列名称和相应的偏移量
                offsets.Add(fldname, pos);
                pos += lengthInBytes(fldname);
            }
            recordlen = pos;
        }

        public TableInfo(string tblname, Schema schema, Dictionary<string ,int> offsets, int recordlen)
        {//构造函数，通过指定元数据创建一个数据表对象
            this.tblname = tblname;
            this.sch = schema;
            this.offsets = offsets;
            this.recordlen = recordlen;
        }

        public string fileName()
        {//返回文件名，当前文件名就是“表名+.tbl”
            return tblname + ".tbl";
        }

        public Schema schema()
        {//返回表记录的schema信息
            return sch;
        }

        public int offset(string fldname)
        {//返回指定列的偏移量
            return offsets[fldname];
        }

        public int recordLength()
        {
            return recordlen;
        }

    }
}
