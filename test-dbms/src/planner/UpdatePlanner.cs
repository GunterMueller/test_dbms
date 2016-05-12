using test_dbms.src.tx;
using test_dbms.src.parse;

namespace test_dbms.src.planner
{
    public interface UpdatePlanner
    {//接口，被SQL insert，delete 和 modify(update) 语句的计划实现

        //执行 insert 语句，返回被影响记录的数目
        int executeInsert(InsertData data, Transaction tx);

        //执行 delete 语句，返回被影响记录的数目
        int executeDelete(DeleteData data, Transaction tx);

        //执行 modify 语句，返回被影响记录的数目
        int executeModify(ModifyData data, Transaction tx);

        //执行 create table 语句，返回被影响记录的数目
        int executeCreateTable(CreateTableData data, Transaction tx);

        //执行 create view 语句，返回被影响记录的数目
        int executeCreateView(CreateViewData data, Transaction tx);

        //执行 create index 语句，返回被影响记录的数目
        int executeCreateIndex(CreateIndexData data, Transaction tx);
    }
}
