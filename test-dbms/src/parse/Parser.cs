using System.Collections.Generic;
using test_dbms.src.record;
using test_dbms.src.query;

namespace test_dbms.src.parse
{
    public class Parser
    {// test_dbms的语法分析器
        private Lexer lex;

        public Parser(string s)
        {
            lex = new Lexer(s);
        }

/// some methods for parsing predicates,terms,expressions, constants,and fields.
        private string field()
        {//切出field，符合字段名称pattern的字符串
            return lex.eatId();
        }

        private Constant constant()
        {//切出Constant，若符合字符串常量pattern的字符串，返回不含''的字符串，指向下一位；否则按照整型常量切词
            if (lex.matchstringConstant())
                return new StringConstant(lex.eatStringConstant());
            else
                return new IntConstant(lex.eatIntConstant());
        }

        private Expression expression()
        {//切出Expression，若满足字段名称的pattern，则切出一个field，否则切出一个Constant
            if (lex.matchId())
                return new FieldNameExpression(field());
            else
                return new ConstantExpression(constant());
        }

        private Term term()
        {//切出Term，形如“F1 = F2”或“F = c”
            Expression lhs = expression();
            lex.eatDelim('=');
            Expression rhs = expression();
            return new Term(lhs, rhs);
        }

        private Predicate predicate()
        {
            Predicate pred = new Predicate(term());
            if (lex.matchKeyword("and"))
            {//遇不到“and”则返回
                lex.eatKeyword("and");
                pred.conjoinWith(predicate());//循环调用
            }
            return pred;
        }

/// Methods for parsing queries
        public QueryData query()
        {
            lex.eatKeyword("select");
            List<string> fields = selectList();
            lex.eatKeyword("from");
            List<string> tables = tableList();
            Predicate pred = new Predicate();
            if (lex.matchKeyword("where"))
            {
                lex.eatKeyword("where");
                pred = predicate();
            }
            return new QueryData(fields, tables, pred);//封装成一个QueryData传给query模块完成查询
        }

        private List<string> selectList()
        {
            List<string> l = new List<string>();
            l.Add(field());
            if (lex.matchDelim(','))
            {
                lex.eatDelim(',');
                l.AddRange(selectList());
            }
            return l;
        }

        private List<string> tableList()
        {
            List<string> l = new List<string>();
            l.Add(lex.eatId());
            if (lex.matchDelim(','))
            {
                lex.eatDelim(',');
                l.AddRange(tableList());
            }
            return l;
        }

// these are methods for parsing the various update commands
        public object updateCmd()
        {//所有更改操作的语句公共入口
            if (lex.matchKeyword("insert"))
                return insert();
            else if (lex.matchKeyword("delete"))
                return delete();
            else if (lex.matchKeyword("update"))
                return modify();
            else
                return create();
        }

        private List<string> fieldList()
        {
            List<string> l = new List<string>();
            l.Add(field());
            if (lex.matchDelim(','))
            {
                lex.eatDelim(',');
                l.AddRange(fieldList());
            }
            return l;
        }

        private List<Constant> constList()
        {
            List<Constant> l = new List<Constant>();
            l.Add(constant());
            if (lex.matchDelim(','))
            {
                lex.eatDelim(',');
                List<Constant> l2 = constList();
                l.AddRange(l2);
            }
            return l;
        }

        private InsertData insert()
        {//插入语句
            lex.eatKeyword("insert");
            lex.eatKeyword("into");
            string tblname = lex.eatId();
            lex.eatDelim('(');
            List<string> flds = fieldList();
            lex.eatDelim(')');
            lex.eatKeyword("values");
            lex.eatDelim('(');
            List<Constant> vals = constList();
            lex.eatDelim(')');
            return new InsertData(tblname, flds, vals);//封装成一个InsertData传给query模块完成查询
        }

        private DeleteData delete()
        {//删除语句
            lex.eatKeyword("delete");
            lex.eatKeyword("from");
            string tblname = lex.eatId();
            Predicate pred = new Predicate();
            if (lex.matchKeyword("where"))
            {
                lex.eatKeyword("where");
                pred = predicate();
            }
            return new DeleteData(tblname, pred);//封装成一个DeleteData传给query模块完成查询
        }

        private ModifyData modify()
        {//更新语句
            lex.eatKeyword("update");
            string tblname = lex.eatId();
            lex.eatKeyword("set");
            string fldname = field();
            lex.eatDelim('=');
            Expression newal = expression();
            Predicate pred = new Predicate();
            if (lex.matchKeyword("where"))
            {
                lex.eatKeyword("where");
                pred = predicate();
            }
            return new ModifyData(tblname, fldname, newal, pred);//封装成一个ModifyData传给query模块完成查询
        }

        private object create()
        {//创建语句：分成新建表、新建视图、新建索引
            lex.eatKeyword("create");
            if (lex.matchKeyword("table"))
                return createTable();
            else if (lex.matchKeyword("view"))
                return createView();
            else
                return createIndex();
        }
        //--------------新建表-----------------
        private CreateTableData createTable()
        {
            lex.eatKeyword("table");
            string tblname = lex.eatId();
            lex.eatDelim('(');
            Schema sch = fieldDefs();
            lex.eatDelim(')');
            return new CreateTableData(tblname, sch);//封装成一个CreateTableData传给query模块完成查询
        }

        private Schema fieldDefs()
        {
            Schema schema = fieldDef();
            if (lex.matchDelim(','))
            {
                lex.eatDelim(',');
                Schema schema2 = fieldDefs();
                schema.addAll(schema2);//将每个模式信息都记录下来
            }
            return schema;
        }

        private Schema fieldDef()
        {
            string fldname = field();
            return fieldType(fldname);//返回了一个模式信息
        }

        private Schema fieldType(string fldname)
        {
            Schema schema = new Schema();
            if (lex.matchKeyword("int"))
            {
                lex.eatKeyword("int");
                schema.addIntField(fldname);
            }
            else
            {
                lex.eatKeyword("varchar");
                lex.eatDelim('(');
                int strLen = lex.eatIntConstant();
                lex.eatDelim(')');
                schema.addStringField(fldname, strLen);//添加字段名称和长度
            }
            return schema;
        }
        //--------------新建视图-----------------
        private CreateViewData createView()
        {
            lex.eatKeyword("view");
            string viewname = lex.eatId();
            lex.eatKeyword("as");
            QueryData qd = query();
            return new CreateViewData(viewname, qd);//封装成一个CreateViewData传给query模块完成查询
        }
        //--------------新建索引-----------------
        private CreateIndexData createIndex()
        {
            lex.eatKeyword("index");
            string idxname = lex.eatId();
            lex.eatKeyword("on");
            string tblname = lex.eatId();
            lex.eatDelim('(');
            string fldname = field();
            lex.eatDelim(')');
            return new CreateIndexData(idxname, tblname, fldname);
        }
    }
}
