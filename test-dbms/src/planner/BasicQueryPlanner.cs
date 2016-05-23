using System.Collections.Generic;
using test_dbms.src.tx;
using test_dbms.src.query;
using test_dbms.src.parse;
using test_dbms.src.server;

namespace test_dbms.src.planner
{
    class BasicQueryPlanner : QueryPlanner
    {//最简单、原始的、可能的查询计划

        public Plan createPlan(QueryData data, Transaction tx)
        {
            //第一步：为每一个出现的表或者视图定义创建一个plan
            List<Plan> plans = new List<Plan>();
            foreach(string tblname in data.tables())
            {
                string viewdef = SimpleDB.mdMgr().getViewDef(tblname, tx);
                if (viewdef != null)
                    plans.Add(SimpleDB.planner().createQueryPlan(viewdef, tx));
                else
                    plans.Add(new TablePlan(tblname, tx));
            }

            //第二步：创建所有表计划的叉积运算
            Plan p = plans[0];
            plans.RemoveAt(0);
            foreach (Plan nextplan in plans)
                p = new ProductPlan(p, nextplan);

            //第三步：为predicate添加一个选择运算selection
            p = new SelectPlan(p, data.pred());

            //第四步：做字段名称上的投影运算
            p = new ProjectPlan(p, data.fields());
            return p;
        }
    }
}
