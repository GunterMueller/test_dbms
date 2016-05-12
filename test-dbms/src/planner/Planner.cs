using test_dbms.src.tx;
using test_dbms.src.query;
using test_dbms.src.parse;

namespace test_dbms.src.planner
{
    public class Planner
    {
        private QueryPlanner qplanner;
        private UpdatePlanner uplanner;

        public Planner(QueryPlanner qplanner, UpdatePlanner uplanner)
        {
            this.qplanner = qplanner;
            this.uplanner = uplanner;
        }

        public Plan createQueryPlan(string qry, Transaction tx)
        {//为select语句创建一个计划，根据语法分析器得到需要的数据，然后生成一个查询计划
            Parser parser = new Parser(qry);
            QueryData data = parser.query();
            return qplanner.createPlan(data, tx);
        }

        public int executeUpdate(string cmd, Transaction tx)
        {//执行一个SQL insert、delete、modify 或者 create 语句，此函数根据语法分析器返回的结果，使用相应的更新计划器返回计算结果
            Parser parser = new Parser(cmd);
            object obj = parser.updateCmd();

            if (obj is InsertData)
                return uplanner.executeInsert((InsertData)obj, tx);
            else if (obj is ModifyData)
                return uplanner.executeModify((ModifyData)obj, tx);
            else if (obj is DeleteData)
                return uplanner.executeDelete((DeleteData)obj, tx);
            else if (obj is CreateTableData)
                return uplanner.executeCreateTable((CreateTableData)obj, tx);
            else if (obj is CreateIndexData)
                return uplanner.executeCreateIndex((CreateIndexData)obj, tx);
            else if (obj is CreateViewData)
                return uplanner.executeCreateView((CreateViewData)obj, tx);
            else
                return 0;
        }

    }
}
