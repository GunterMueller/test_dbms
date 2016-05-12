using System.Collections.Generic;
using test_dbms.src.query;

namespace test_dbms.src.parse
{
    public class QueryData
    {
        private List<string> i_fields;
        private List<string> i_tables;
        private Predicate i_pred;

        public QueryData(List<string> fields, List<string> tables, Predicate pred)
        {
            this.i_fields = fields;
            this.i_tables = tables;
            this.i_pred = pred;
        }

        public List<string> fields()
        {//返回所有在“select”后提到的字段名称
            return i_fields;
        }

        public List<string> tables()
        {//返回所有在“from”词后提到的表名称
            return i_tables;
        }

        public Predicate pred()
        {//返回predicate所有的Terms对象（描述哪些记录应该输出）
            return i_pred;
        }

        public override string ToString()
        {
            string result = "select ";
            foreach (string fldname in i_fields)
                result += fldname + ", ";
            result = result.Substring(0, result.Length - 2);//去掉结尾的", "
            foreach (string tblname in i_tables)
                result += tblname + ", ";
            result = result.Substring(0, result.Length - 2);
            string predstring = i_pred.ToString();
            if (!predstring.Equals(""))
                result += " where " + predstring;
            return result;
        }
        
    }
}
