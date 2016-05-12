using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace test_dbms.src.parse
{
    public class Lexer
    {//词法分析器
        private List<string> keywords = new List<string>();
        //关于C#正则表达式：http://www.cnblogs.com/w-y-f/archive/2012/05/14/2499963.html
        private string[] tokens;
        private int position;//当前指针位置
        private const string intPattern = @"^[-+]?(0|[1-9]\d*)$";//整数值
        private const string stringPattern = @"^'[^']*'$";//字符串
        private const string idPattern = @"^[a-zA-Z_]\w*$";//匹配ID名称（列名），\w表示包括下划线的任何字符，等价于'[A-Za-z0-9_]'
        private const string keywordPattern = @"^[a-zA-Z]*$";//关键词（Create、update等）
        private const string splitPattern = @"\s{1}|(,){1}|(=){1}|(\(){1}|(\)){1}|('[^']*'){1}";//分隔符

        private void nextToken()
        {
            position++;
            while (position < tokens.Length && tokens[position].Length == 0)
                position++;
        }

        private void initKeywords()
        {
            keywords.Add("select");
            keywords.Add("from");
            keywords.Add("where");
            keywords.Add("and");
            keywords.Add("insert");
            keywords.Add("into");
            keywords.Add("values");
            keywords.Add("delete");
            keywords.Add("update");
            keywords.Add("set");
            keywords.Add("create");
            keywords.Add("table");
            keywords.Add("int");
            keywords.Add("varchar");
            keywords.Add("view");
            keywords.Add("as");
            keywords.Add("index");
            keywords.Add("on");     
        }
//---------------------------------------------------------------------------   
        public Lexer(string s)
        {
            initKeywords();
            //利用捕获方式分割，会包含空格符号
            tokens = Regex.Split(s, splitPattern);//通过将分界符得到tokens
            position = -1;
            nextToken();//开始从头遍历
        }

        public bool matchDelim(char d)
        {//判断字符是否为指定的分界符
            if (position >= tokens.Length)
                return false;
            return d.ToString() == tokens[position];
        }

        public bool matchIntConstant()
        {//判断当前token是否为整数
            if (position >= tokens.Length)
                return false;
            return Regex.IsMatch(tokens[position], intPattern);
        }

        public bool matchstringConstant()
        {//判断当前token是否为字符串
            if (position >= tokens.Length)
                return false;
            return Regex.IsMatch(tokens[position], stringPattern);
        }

        public bool matchKeyword(string w)
        {//判断当前token是否为指定的关键词
            if (position >= tokens.Length)
                return false;
            return Regex.IsMatch(tokens[position], keywordPattern) && tokens[position].ToLower() == w;
        }

        public bool matchId()
        {//判断当前token是否为ID名称，且不能为关键词
            if (position >= tokens.Length)
                return false;
            return Regex.IsMatch(tokens[position], idPattern) && !keywords.Contains(tokens[position].ToLower());
        }

        public void eatDelim(char d)
        {//若当前token不是指定分界符，则抛出异常；若是则移向下一个token
            if (!matchDelim(d))
                throw new BadSyntaxException();
            nextToken();
        }

        public int eatIntConstant()
        {//若当前token是整数，返回整数值，移动到下一个token；否则抛异常
            if (!matchIntConstant())
                throw new BadSyntaxException();
            int i = int.Parse(tokens[position]);//将一个整数token的string形式转化为它等价的32位有符号数的整数表示
            nextToken();
            return i;
        }
           
        public string eatStringConstant()
        {
            if (!matchstringConstant())
                throw new BadSyntaxException();
            //字符串形式：'heheda'->length:8,从第1位开始读，读6位
            string s = tokens[position].Substring(1, tokens[position].Length - 2);
            nextToken();
            return s;
        }

        public void eatKeyword(string w)
        {
            if (!matchKeyword(w))
                throw new BadSyntaxException();
            nextToken();
        }

        public string eatId()
        {
            if (!matchId())
                throw new BadSyntaxException();
            string s = tokens[position].ToLower();
            nextToken();
            return s;
        }
    }
}
