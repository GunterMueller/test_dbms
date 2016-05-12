using test_dbms.src.tx;
using test_dbms.src.query;
using test_dbms.src.parse;

namespace test_dbms.src.planner
{
    public interface QueryPlanner
    {//几口，被SQL select语句的计划实现

        //返回查询的一个计划
        Plan createPlan(QueryData data, Transaction tx);
    }
}
