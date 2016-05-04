using System.Collections.Generic;

namespace test_dbms.src.record
{
    public class Schema
    {//数据库表记录的schema信息：包含每个列的名称和类型，还有VARCHAR类型的字符长度,schema=database包含一套表、视图等
        private class FieldInfo
        {//列信息的一部分，定义了一个私有访问类：包含了类型、长度
            public int type, length;
            public FieldInfo(int type, int length) 
            {
                this.type = type;
                this.length = length;
            }
        }
        //字典包含了列名称和包装的私有类FieldInfo，构成了完整的列信息
        private Dictionary<string, FieldInfo> info = new Dictionary<string, FieldInfo>();

        //代表类型的值，来源于JDBC
        public const int INTEGER = 4, VARCHAR = 12;
        
        //创建一个空的约束，列的信息可以通过下述5种addXXX函数添加到一个约束中
        public Schema() { }

        public void addField(string fldname, int type, int length)
        {//添加一个列，包含列名称、类型和长度，若类型为整数，则长度的值无关紧要
            info.Add(fldname, new FieldInfo(type, length));
        }
        
        public void addIntField(string fldname)
        {//添加一个整数列
            addField(fldname, INTEGER, 0);
        }

        public void addStringField(string fldname, int length)
        {//添加一个字符串列
            addField(fldname, VARCHAR, length);
        }

        public void add(string fldname, Schema sch)
        {//在另一个schema中添加一个关联的列信息（类型、长度相同）
            int type = sch.type(fldname);
            int length = sch.length(fldname);
            addField(fldname, type, length);
        }

        public void addAll(Schema sch)
        {//为当前schema添加指定schema下所有的列信息
            foreach (KeyValuePair<string, FieldInfo> kvp in sch.info)
                info.Add(kvp.Key, kvp.Value);
        }

        public ICollection<string> fields()
        {//返回一个集合包含所有列名称
            return info.Keys;
        }

        public bool hasField(string fldname)
        {//若当前列名称中包含指定名称，返回true
            return info.ContainsKey(fldname);
        }

        public int type(string fldname)
        {
            return info[fldname].type;
        }
         
        public int length(string fldname)
        {
            return info[fldname].length;
        }
    }
}
