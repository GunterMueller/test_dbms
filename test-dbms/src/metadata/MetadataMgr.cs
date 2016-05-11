using System.Collections.Generic;
using test_dbms.src.tx;
using test_dbms.src.record;

namespace test_dbms.src.metadata
{
    public class MetadataMgr
    {//元数据管理：管理表信息、视图信息、统计信息、索引信息
        private static TableMgr tblmgr;
        private static ViewMgr viewmgr;
        private static StatMgr statmgr;
        private static IndexMgr idxmgr;

        public MetadataMgr(bool isnew, Transaction tx)
        {//初始化元数据管理器
            tblmgr = new TableMgr(isnew, tx);//维护了两张表，表信息（名称，记录长度），字段信息；创建表和获取表信息
            viewmgr = new ViewMgr(isnew, tblmgr, tx);//用tblMgr创建视图表，存储视图信息
            statmgr = new StatMgr(tblmgr, tx);//使用tblMgr获取到每张表的信息
            idxmgr = new IndexMgr(isnew, tblmgr, tx);//使用tblMgr创建索引信息表
        }

        public void createTable(string tblname, Schema sch, Transaction tx)
        {//向表信息中添加信息，如果有表的话（表名称，模式[字段名称，字段类型，字段长度]）
            tblmgr.createTable(tblname, sch, tx);
        }

        public TableInfo getTableInfo(string tblname, Transaction tx)
        {//获取指定表名称的表信息（一条记录的长度，字段名称，字段类型，字段长度，每个字段的偏移量）
            return tblmgr.getTableInfo(tblname, tx);
        }

//------------------------------------------------------------------------------------------------
        public void createView(string viewname, string viewdef, Transaction tx)
        {//向视图信息表添加信息，如果有视图表的话（视图名称、视图定义）
            viewmgr.createView(viewname, viewdef, tx);
        }

        public string getViewDef(string viewname, Transaction tx)
        {//获取指定视图上对应的视图定义
            return viewmgr.getViewDef(viewname, tx);
        }

//------------------------------------------------------------------------------------------------
        public void createIndex(string idxname, string tblname, string fldname, Transaction tx)
        {
            idxmgr.createIndex(idxname, tblname, fldname, tx);
        }

        public Dictionary<string, IndexInfo> getIndexInfo(string tblname, Transaction tx)
        {//获取到指定表上的索引信息（<字段名称，IndexInfo>的一个映射，其中IndexInfo包含idxname, tblname, fldname, tx）
            return idxmgr.getIndexInfo(tblname, tx);
        }

//------------------------------------------------------------------------------------------------
        public StatInfo getStatInfo(string tblname, TableInfo ti, Transaction tx)
        {//获取指定表的统计信息（包含块数、记录数）
            return statmgr.getStatInfo(tblname, ti, tx);
        }
    }
}