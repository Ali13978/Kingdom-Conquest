using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
public class MBDeal: MonoBehaviour {
	const byte _t = (byte)'\t';
    const byte _n = (byte)'\n';
	
    // 从dat的start位置开始读取，如果遇到\t或者\n就停下并返回（根据返回的最后一个字符来判断是\t还是\n。如果返回null则表示没有可读的了） 
    static public byte[] readPart(byte[] dat, int start)
    {
        if (start >= dat.Length)
        {
            return null;
        }
        byte[] rst = null;
        int i = start;
        while (i < dat.Length)
        {
            if (dat[i] == _t || dat[i] == _n)
            {
                int l   = i-start+1;
                rst = new byte[l];
               	Array.Copy(dat, start, rst, 0, l);
                return rst;
            }
            i++;
        }
        // 剩下的都是最后一个元素 
        rst = new byte[dat.Length - start];
        Array.Copy(dat, start, rst, 0, rst.Length);
        return rst;
    }
	
    // 读取mbbyte数组到多列数组，只处理字串，分隔符为tab（因为可能要读取加密文件，所以这里实现从字串读取的接口即可）
    // 参数：
    // dat          字节数组（可能是文件内容，或者是解密之后的文件内容。编码应该为utf8格式）。
    // readTitle    如果为true，则读入第一行的title信息（一般不用）
    // 返回：
    // 字串数组的数组。第一维为列。第二维为每列的数据（如果没有内容的，string为null）。
	
    static public string[][] readMBFromBytes(byte[] dat, bool readTitle, bool completeTable)
    {
        if (dat==null)
        {
            return null;
        }
        // 先读取第一行，获得总共的列数，从而创建列数组  
        List<string> names = new List<string>();
        int idx = 0;
        bool eol = false;
        byte[] part;
        while (!eol)
        {
            part = readPart(dat, idx);
            if (part == null)
            {
                // 说明是空文件（因为即便是只有一行的也会返回字串的）  
                return null;
            }
			
            // 如果不是\t结尾就说明是最后一个元素  
            if (part[part.Length - 1] != _t)
            {
                eol = true;
            }
            idx += part.Length;
            string s = Encoding.UTF8.GetString(part);
            s   = s.Trim();

            names.Add(s);
        }
        // 读取后面的数据行  
        int i,j;
		
        // 创建每行的list  
        List<string>[] columnList = new List<string>[names.Count];
        for (i = 0; i < names.Count; i++)
        {
            columnList[i]   = new List<string>();
        }
  
        int line=0;
        bool eof=false;
        i   = 0;    
        while(!eof)
        {
            List<string>    curList = columnList[i];    
            part = readPart(dat, idx);
            if (part == null)
            {
                // 如果需要完整表，则判断一下所有列的最大元素个数是多少 
                if (completeTable)
                {
                    bool needComplete = false;
                    int nMax = columnList[0].Count;
                    for (i = 1; i < names.Count; i++)
                    {
                        if (columnList[i].Count > nMax)
                        {
                            nMax = columnList[i].Count;
                            needComplete = true;
                        }
                        else if( columnList[i].Count != columnList[i-1].Count )
                        {
                            needComplete = true;
                        }
                    }
                    if (needComplete)
                    {
                        for (i = 0; i < names.Count; i++)
                        {
                            curList = columnList[i];
                            while (curList.Count < nMax)
                            {
                                curList.Add(null);
                            }
                        }
                    }
                }
                break;
            }
            idx += part.Length;
            string s = Encoding.UTF8.GetString(part);
            s   = s.Trim();
            if (s.Length > 0)
            {			
				
                while (curList.Count < line)
                {
                    curList.Add(null);
                }
                curList.Add(s);
            }
            // 如果不是\t结尾就说明是最后一个元素   
            if (part[part.Length - 1] != _t)
            {
                i = 0;
				
                //行号增加 
				
                line++;

            }
            else
            {
                // 下一列  
                i++;
            }
        }

        // 根据columnList生成最终的每列的array  
        string[][] rst  = new string[columnList.Length][];
        for (i = 0; i < columnList.Length; i++)
        {
            List<string> sl = columnList[i];
            string[] sa;
       
            if( readTitle )
            {
                sa = new string[sl.Count + 1];
                sa[0] = names[i];
                idx = 1;
            }
            else
            {
                sa = new string[sl.Count];
                idx = 0;
            }
  
            for(j=0;j<sl.Count;j++)
            {
                sa[idx++] = sl[j];
            }
            rst[i] = sa;
        }
        
        return rst;
    }
	
    // 把普通文件读入到byte数组  
    static public byte[] readFile(string path)
    {
        FileInfo fi = new FileInfo(path);
        using (FileStream fs = fi.OpenRead())
        {
            byte[] dat = new byte[fi.Length];
            if (fs.Read(dat, 0, dat.Length) != dat.Length)
            {
              	Console.WriteLine("data read error");
                return null;
            }
            fs.Close();
            return dat;
        }
    }
}
