using System.Collections.Generic;
using test_dbms.src.tx;
using test_dbms.src.record;

namespace test_dbms.src.metadata
{
    class TableMgr
    {//表信息管理器，有两个功能：创建表时保存表的元数据（表基本信息、字段信息）、获取一个已创建表的元数据
        
        //无论是表名称、字段名称的最大字符长度，当前设置为16
        public const int MAX_NAME = 16;

        private TableInfo tcatInfo, fcatInfo;

        public TableMgr(bool isNew, Transaction tx)
        {/* 为数据库创建目录表，用来维护表的信息，若数据库是新建的，那么两张表也会被创建*/
            //创建表基本模式信息：表名称、每条记录的长度
            Schema tcatSchema = new Schema();
            tcatSchema.addStringField("tblname", MAX_NAME);
            tcatSchema.addIntField("reclength");
            tcatInfo = new TableInfo("tblcat", tcatSchema);
            //创建字段基本模式信息：表名称、字段名称、类型、长度、偏移量
            Schema fcatSchema = new Schema();
            fcatSchema.addStringField("tblname", MAX_NAME);
            fcatSchema.addStringField("fldname", MAX_NAME);
            fcatSchema.addIntField("type");
            fcatSchema.addIntField("length");
            fcatSchema.addIntField("offset");
            fcatInfo = new TableInfo("fldcat", fcatSchema);

            if(isNew)
            {
                createTable("tblcat", tcatSchema, tx);
            }
        }

        public void createTable(string tblname, Schema sch, Transaction tx)
        {//创建一个新表，有指定的表名称、模式信息
            TableInfo ti = new TableInfo(tblname, sch);
            //往tblcat表中插入一条记录
            RecordFile tcatfile = new RecordFile(tcatInfo, tx);
            tcatfile.insert();
            tcatfile.setString("tblname", tblname);
            tcatfile.setInt("reclength", ti.recordLength());
            tcatfile.close();

            //往fldcat表中插入记录：每个字段的信息
            RecordFile fcatfile = new RecordFile(fcatInfo, tx);
            foreach(string fldname in sch.fields())
            {
                fcatfile.insert();
                fcatfile.setString("tblname", tblname);
                fcatfile.setString("fldname", fldname);
                fcatfile.setInt("type", sch.type(fldname));
                fcatfile.setInt("length", sch.length(fldname));
                fcatfile.setInt("offset", ti.offset(fldname));
            }
            fcatfile.close();
        }

        public TableInfo getTableInfo(string tblname, Transaction tx)
        {
            RecordFile tcatfile = new RecordFile(tcatInfo, tx);
            int reclen = -1;
            while(tcatfile.next())
            {
                if(tcatfile.getString("tblname").Equals(tblname))
                {//获取到指定表名称的一条记录长度
                    reclen = tcatfile.getInt("reclength");
                    break;
                }
            }
            tcatfile.close();

            RecordFile fcatfile = new RecordFile(fcatInfo, tx);
            Schema sch = new Schema();
            Dictionary<string, int> offsets = new Dictionary<string, int>();
            while(fcatfile.next())
            {//每次取fcatfile中的一条记录
                if(fcatfile.getString("tblname").Equals(tblname))
                {
                    string fldname = fcatfile.getString("fldname");
                    int fldtype = fcatfile.getInt("type");
                    int fldlen = fcatfile.getInt("length");
                    int offset = fcatfile.getInt("offset");
                    offsets.Add(fldname, offset);//获取一张表中每个字段名称的偏移量
                    sch.addField(fldname, fldtype, fldlen);//获取一张表的模式信息
                }
            }
            fcatfile.close();
            return new TableInfo(tblname, sch, offsets, reclen);
        }

    }
}
