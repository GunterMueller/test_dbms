using System;

namespace test_dbms.src.query
{
    public interface Constant : IComparable<Constant>
    {//接口：代表数据库中存储的值信息
        object asCsharpVal();

    }
}
