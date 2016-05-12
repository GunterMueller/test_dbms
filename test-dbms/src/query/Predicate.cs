using System.Collections.Generic;
using test_dbms.src.record;

namespace test_dbms.src.query
{
    public class Predicate
    {//一个predicate就是一组 term 的布尔值
        private List<Term> terms = new List<Term>();

        public Predicate() { }

        public Predicate(Term T)
        {//构造函数，包含一个Term对象
            terms.Add(T);
        }

        public void conjoinWith(Predicate pred)
        {//修改当前predicate为它自己和指定pred的连接
            terms.AddRange(pred.terms);
        }

        public bool isSatisfied(Scan s)
        {
            foreach (Term t in terms)
                if (!t.isSatisfied(s))
                    return false;
            return true;
        }

        public int reductionFactor(Plan p)
        {
            int factor = 1;
            foreach (Term t in terms)
                factor *= t.reductionFactor(p);
            return factor;
        }

        public Predicate selectPred(Schema sch)
        {//返回符合指定schema的子predicate
            Predicate result = new Predicate();
            foreach (Term t in terms)
                if (t.appliesTo(sch))
                    result.terms.Add(t);
            if (result.terms.Count == 0)
                return null;
            else
                return result;
        }

        public Predicate joinPred(Schema sch1, Schema sch2)
        {//返回符合两个指定schema交集的子predicate
            Predicate result = new Predicate();
            Schema newsch = new Schema();
            newsch.addAll(sch1);
            newsch.addAll(sch2);
            foreach (Term t in terms)
                if (!t.appliesTo(sch1) && !t.appliesTo(sch2) && t.appliesTo(newsch))
                    result.terms.Add(t);
            if (result.terms.Count == 0)
                return null;
            else
                return result;
        }

        public Constant equatesWithConstant(string fldname)
        {//判断term是否形如“F=c”，其中F如果是指定的fldname，而c是另外一个值，就返回c常量
            foreach(Term t in terms)
            {
                Constant c = t.equatesWithConstant(fldname);
                if (c != null)
                    return c;
            }
            return null;
        }

        public string equatesWihField(string fldname)
        {//判断term是否形如“F1=F2”，其中F1如果是指定的fldname，而F2是另外一个值，就返回F2字段名称
            foreach(Term t in terms)
            {
                string s = t.equatesWithField(fldname);
                if (s != null)
                    return s;
            }
            return null;
        }

        public override string ToString()
        {
            List<Term>.Enumerator iter = terms.GetEnumerator();
            if (!iter.MoveNext())
                return "";
            string result = iter.Current.ToString();
            while (iter.MoveNext())
                result += " and " + iter.Current.ToString();
            return result;
        }

    }
}
