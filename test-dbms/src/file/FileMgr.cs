using System;
using System.IO;
using System.Collections.Generic;

namespace test_dbms.src.file
{
    public class FileMgr
    {
        private string dbDirectory;
        private bool isnew;
        //用字典对文件名和文件流做了一个映射:openFiles
        private Dictionary<string, FileStream> openFiles = new Dictionary<string, FileStream>();
        private object threadLock = new object();//应该使用不影响其他操作的私有对象作为threadLock

        private FileStream getFile(string filename)
        {
            FileStream fs;
            if(openFiles.ContainsKey(filename))
            {//若当前openFiles集合中包含这个传参的filename，则为true
                fs = openFiles[filename];//令fs等于openFiles映射的值即文件流
            }
            else
            {
                string dbTable = Path.Combine(dbDirectory, filename);
                //FileStream的常用属性： 表示使用fs流对象对文件进行操作，允许的操作有FileMode.OpenOrCreate“指示操作系统应打开文件，如果文件不存在则创建新文件”等等
                fs = new FileStream(dbTable, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 8, FileOptions.WriteThrough);
                //添加映射
                openFiles.Add(filename, fs);
            }
             return fs;
        }

        public FileMgr(string dbname)
        {//构造函数,创建一个新的文件
            string homedir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//将主目录定位到“我的文档”文件夹下
            dbDirectory = Path.Combine(homedir, dbname);//通过指向路径和存储数据库的文件夹名 -> 路径:dbDirectory
            DirectoryInfo di = new DirectoryInfo(dbDirectory);//通过路径新建一个文件夹信息对象
            isnew = !di.Exists;

            //如果是新数据库(名)就创建这个文件夹
            if (isnew)
                di.Create();
            //删除所有剩余的暂时性的数据表
            foreach (FileInfo fi in di.GetFiles())
                if (fi.Name.StartsWith("temp"))
                    fi.Delete();
        }

        internal void read(Block blk, byte[] bb)
        {//internal 只能在当前程序集下访问（多个DLL同时引用时，只能在当前DLL中的类中访问），类似于private
            lock(threadLock)
            {//lock()括号中为引用类型，不能为值类型
                try
                {
                    bb.Initialize();
                    FileStream fs = getFile(blk.fileName());
                    //将文件指针以字节为单位移动到number*512（假设为3*512，实际上是第4个块的开始）
                    fs.Seek(blk.number() * Page.BLOCK_SIZE, SeekOrigin.Begin);
                    //将接下来的512字节读取到bb数组中
                    fs.Read(bb, 0, Page.BLOCK_SIZE);
                }catch(IOException)
                {
                    throw new Exception("cannot read block " + blk);
                }
            }
        }

        internal void write(Block blk, byte[] bb)
        {
            lock(threadLock)
            {
                try
                {
                    FileStream fs = getFile(blk.fileName());
                    fs.Seek(blk.number() * Page.BLOCK_SIZE, SeekOrigin.Begin);
                    fs.Write(bb, 0, Page.BLOCK_SIZE);
                }catch(IOException)
                {
                    throw new Exception("cannot write block " + blk);
                }
            }
        }

        internal Block append(string filename, byte[] bb)
        {
            lock(threadLock)
            {
                int newblknum = size(filename);
                Block blk = new Block(filename, newblknum);
                write(blk, bb);
                return blk;
            }
        }

        public int size(string filename)
        {
            lock(threadLock)
            {
                try
                {
                    FileStream fs = getFile(filename);
                    return (int)(fs.Length / Page.BLOCK_SIZE);
                }catch(IOException)
                {
                    throw new Exception("cannot access " + filename);
                }
            }
        }

        public bool isNew()
        {
            return isnew;//数据库是新更新过的，则置为true
        }

    }
}
