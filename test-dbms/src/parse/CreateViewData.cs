
namespace test_dbms.src.parse
{
    public class CreateViewData
    {// “create view”语句的数据：视图名称、视图定义（一个select语句）
        private string viewname;
        private QueryData qrydata;

        public CreateViewData(string viewname, QueryData qrydata)
        {//保存视图名称和视图定义
            this.viewname = viewname;
            this.qrydata = qrydata;
        }

        public string viewName()
        {//返回视图名称
            return viewname;
        }

        public string viewDef()
        {//返回视图的定义：一个Select语句
            return qrydata.ToString();
        }

    }
}
