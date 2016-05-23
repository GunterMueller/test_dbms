using System.Collections.Generic;
using test_dbms.src.record;

namespace test_dbms.src.query
{
    public interface Expression
    {//与 SQL <Expression> 关联的接口
        
        //如果表达式是一个常量（Constant），返回true
        bool isConstant();

        //如果表达式是一个字段名称（FieldName），返回true
        bool isFieldName();

        //返回与常量表达式相关联的常量值，若此表达式不表示常量，则抛异常
        Constant asConstant();

        //返回字段名称，若此表达式不表示一个字段，则抛异常
        string asFieldName();

        //计算当前记录、指定scan处的表达式的值
        Constant evaluate(Scan s);

        //判断当前所有在表达式中提到的字段名称，是否都包含在schema中
        bool appliesTo(Schema sch);

    }

    public interface fExpression
    {//与 SQL <fExpression> 关联的接口

        //如果表达式是一个常量（Constant），返回true
        bool isConstant();

        //如果表达式是一个字段名称（FieldName），返回true
        bool isFieldName();

        //返回与常量表达式相关联的常量值，若此表达式不表示常量，则抛异常
        Constant asConstant();

        //返回字段名称，若此表达式不表示一个字段，则抛异常
        string asFieldName();

        //计算当前记录、指定scan处的表达式的值
        Constant evaluate(Scan s);

        //判断当前所有在表达式中提到的字段名称，是否都包含在schema中
        bool appliesTo(Schema sch);

    }
}
