using test_dbms.src.tx;
using test_dbms.src.query;
using test_dbms.src.parse;
using test_dbms.src.server;

namespace test_dbms.src.planner
{
    class BasicUpdatePlanner : UpdatePlanner
    {
        public int executeDelete(DeleteData data, Transaction tx)
        {
            Plan p = new TablePlan(data.tableName(), tx);
            p = new SelectPlan(p, data.pred());
            UpdateScan us = (UpdateScan)p.open();
            int count = 0;
            while(us.next())
            {
                us.delete();
                count++;
            }
            us.close();
            return count;
        }

        public int executeModify(ModifyData data, Transaction tx)
        {
            Plan p = new TablePlan(data.tableName(), tx);
            p = new SelectPlan(p, data.pred());
            UpdateScan us = (UpdateScan)p.open();
            int count = 0;
            while (us.next())
            {
                Constant val = data.newValue().evaluate(us);//返回此处的表达式的常量值
                us.setVal(data.targetField(), val);//设置要更改字段处的值
                count++;
            }
            us.close();
            return count;
        }

        public int executeInsert(InsertData data, Transaction tx)
        {
            Plan p = new TablePlan(data.tableName(), tx);
            UpdateScan us = (UpdateScan)p.open();
            us.insert();
            string fldname;
            Constant val;
            for (int i = 0; i < data.fields().Count; i++)
            {
                fldname = data.fields()[i];
                val = data.vals()[i];
                us.setVal(fldname, val);//一对一，对应的设置
            }
            us.close();
            return 1;
        }

        public int executeCreateTable(CreateTableData data, Transaction tx)
        {
            SimpleDB.mdMgr().createTable(data.tableName(), data.newSchema(), tx);
            return 0;
        }

        public int executeCreateView(CreateViewData data, Transaction tx)
        {
            SimpleDB.mdMgr().createView(data.viewName(), data.viewDef(), tx);
            return 0;
        }

        public int executeCreateIndex(CreateIndexData data, Transaction tx)
        {
            SimpleDB.mdMgr().createIndex(data.indexName(), data.tableName(), data.fieldName(), tx);
            return 0;
        }

    }
}
