using System;
using System.IO;
using test_dbms.src.tx;
using test_dbms.src.record;
using test_dbms.src.query;
using test_dbms.src.parse;
using test_dbms.src.planner;
using test_dbms.src.server;

namespace run_dbms
{
    class Program
    {
        static void Main(string[] args)
        {
            string cmd, cmdstr;

            help();
            string dbname = "test-db";
            Console.WriteLine("Current database is " + dbname);
            string homedir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string dbDirectory = Path.Combine(homedir, dbname);
            Console.WriteLine("Home directory is " + dbDirectory);
            Console.WriteLine("Use the following command to get metadata information;");
            Console.WriteLine("    select tblname,reclength from tblcat;");
            Console.WriteLine("    select tblname,fldname,type,length,offset from fldcat;");
            SimpleDB.init(dbname);

            while (true)
            {
                Console.Write(dbname + ">");
                cmd = Console.ReadLine().Trim();
                if (cmd.EndsWith(";"))
                    cmd = cmd.Substring(0, cmd.Length - 1);
                if (cmd.Length == 0) continue;
                cmdstr = cmd.ToLower();

                if (cmdstr == "exit")
                    break;
                else if (cmdstr == "help")
                    help();
                else if (cmdstr.StartsWith("select"))
                {
                    try
                    {
                        doQuery(cmd);
                    }
                    catch (BadSyntaxException)
                    {
                        Console.WriteLine("Error SQL command!");
                    }
                }
                else if (cmdstr.StartsWith("insert") ||
                    cmdstr.StartsWith("delete") ||
                    cmdstr.StartsWith("update") ||
                    cmdstr.StartsWith("create"))
                {
                    try
                    {
                        doUpdate(cmd);
                    }
                    catch (BadSyntaxException)
                    {
                        Console.WriteLine("Error SQL command!");
                    }
                }
                else
                    Console.WriteLine("Unknown command!");
            }
        }

        static void doQuery(string cmd)
        {
            int length;
            string format;
            Planner planner = SimpleDB.planner();
            Transaction tx = new Transaction();

            Plan p = planner.createQueryPlan(cmd, tx);
            Schema sch = p.schema();
            foreach (string fldname in sch.fields())
            {
                if (sch.type(fldname) == Schema.INTEGER)
                {
                    length = Math.Max(5, fldname.Length) + 2;
                    format = "{0," + length + "}";
                    Console.Write(format, fldname);
                }
                else
                {
                    length = Math.Max(sch.length(fldname), fldname.Length) + 2;
                    format = "{0," + length + "}";
                    Console.Write(format, fldname);
                }
            }
            Console.WriteLine();

            Scan s = p.open();
            while (s.next())
            {
                foreach (string fldname in sch.fields())
                {
                    if (sch.type(fldname) == Schema.INTEGER)
                    {
                        length = Math.Max(5, fldname.Length) + 2;
                        format = "{0," + length + "}";
                        Console.Write(format, s.getInt(fldname));
                    }
                    else
                    {
                        length = Math.Max(sch.length(fldname), fldname.Length) + 2;
                        format = "{0," + length + "}";
                        Console.Write(format, s.getString(fldname));
                    }
                }
                Console.WriteLine();
            }
            s.close();

            tx.commit();
        }

        static void doUpdate(string cmd)
        {
            Planner planner = SimpleDB.planner();
            Transaction tx = new Transaction();

            int num = planner.executeUpdate(cmd, tx);
            tx.commit();

            System.Console.WriteLine(num + " records affected");
        }

        static void help()
        {
            Console.WriteLine("Commands include HELP, EXIT, and sql-senetences.");
        }
    }
}