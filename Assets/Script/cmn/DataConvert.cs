using UnityEngine;
using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using CMNCMD;
public class DataConvert : MonoBehaviour {

	// Use this for initialization 
	//格式化字符串后面的乱码(从服务器收到的字符串一般都需要调用此函数)   
	public static byte[] StrAdd0(byte[]strbuff)
	{
			int strlen = strbuff.Length;
			int i = 0;
			for(; i< strlen;i++)
			{
					if( strbuff[i] == 0)
					{
						//Array.Clear(strbuff,i,strlen - i);
						break;
					}
			}
			if(i==strbuff.Length)
				return strbuff;
			byte[] strbytes = new byte[i]; 
			Array.Copy(strbuff,0,strbytes,0,i);
			return strbytes;
	}
	//字符串转二进制 
	public static byte[] StrToBytes(string str)
	{
		byte[] bstring=System.Text.Encoding.UTF8.GetBytes(str);
			
		return bstring;
	}
	//utf8二进制转字符串 
	public static string BytesToStr(byte[] strbuff)
	{
		string str = "";
		if(strbuff != null)
			str = System.Text.Encoding.GetEncoding("UTF-8").GetString(StrAdd0(strbuff));
		return str;
	}
	//INT转时间 
	public static string TimeIntToStr(uint time_t)
	{
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(time_t);
			return  String.Format("{0:yyyy/MM/dd HH:mm:ss}", dt);
	}
	//INT转世界时间（时分秒） 
	public static string WorldTimeIntStr(uint time_t)
	{
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified).AddSeconds(time_t);
			//return  string.Format("{0:T}", dt);
			//return dt.GetDateTimeFormats('T')[0].ToString();
			return	dt.ToString("HH:mm:ss");
	}
	//时间转INT 
	public static int DateTimeToInt(System.DateTime time)
    {
            int intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (int)(time - startTime).TotalSeconds;
            return intResult;
    }
	//数据打包  
	public static byte[] PackageData(byte[] data,int CntrID,byte BaseCmd,CMN_MSG_SUBCMD_T SubCmd)
	{
			int datalen = data.Length;
			CMN_MSG_HEAD_T  Msg_Head = new CMN_MSG_HEAD_T();
			CMN_MSG_BASE_T  Msg_Base = new CMN_MSG_BASE_T();
			int headlen = U3dCmn.GetSizeofSimpleStructure<CMN_MSG_HEAD_T>();
			int msgbaselen = U3dCmn.GetSizeofSimpleStructure<CMN_MSG_BASE_T>();
			 int subcmdlen =  U3dCmn.GetSizeofSimpleStructure<CMN_MSG_SUBCMD_T>();
			
			Msg_Head.nSize1 =  msgbaselen + datalen;
			if ( BaseCmd == (byte)MSG_HEAD.MSG_DATA)
					Msg_Head.nSize1+= subcmdlen+1;
			Msg_Head.CntrID2 = CntrID;
			byte[] headbuff = StructToByte<CMN_MSG_HEAD_T>(Msg_Head);
		
			Msg_Base.Cmd1 = (byte)BaseCmd;
			byte[] msgbasebuff = StructToByte<CMN_MSG_BASE_T>(Msg_Base);
			byte[] subcmdbuff = StructToByte<CMN_MSG_SUBCMD_T>(SubCmd);
		
			int newlen = headlen+msgbaselen+ datalen;
			if ( BaseCmd == (byte)MSG_HEAD.MSG_DATA)
					newlen+= subcmdlen+1;
		
			byte[] buff = new byte[newlen];
			Array.Copy(headbuff,0,buff,0,headlen);
			Array.Copy(msgbasebuff,0,buff,headlen,msgbaselen);
			if ( BaseCmd == (byte)MSG_HEAD.MSG_DATA)
			{	
				byte[] encodebuf = new byte[subcmdlen+datalen];
				Array.Copy(subcmdbuff,0,encodebuf,0,subcmdlen);
				Array.Copy(data,0,encodebuf,subcmdlen,datalen);	
				buff[headlen+msgbaselen]= DataConvert.CRC8(encodebuf);
				Array.Copy(encodebuf,0,buff,headlen+msgbaselen+1,encodebuf.Length);
			}
			else
			{
				Array.Copy(data,0,buff,headlen+msgbaselen,datalen);
			}
			
			//by []intbuff = (int [])buff;
			return buff;
	}
	//结构体序列化 
	public static byte[] StructToByte<T>(T structure)
	{
			Type type=typeof(T);
	   	//	FieldInfo [] staticinfos=type.GetFields(BindingFlags.Public | BindingFlags.Static );
			
			FieldInfo [] infos=type.GetFields();
			int infonum = infos.Length;
			FieldInfo [] sortinfos = new FieldInfo[infonum];
			for (int i=0; i<infonum; i++)
			{
				string strinfo =  infos[i].ToString();
				int num = -1;
				char [] strchararray = new char[strinfo.Length];
				strchararray = strinfo.ToCharArray();
				for (num = 1; num <=strchararray.Length; num++)
				{
					if (!Char.IsNumber(strchararray[strchararray.Length-num]))
						break;
				}
				num--;
				int	index = Convert.ToInt32 (strinfo.Substring(strinfo.Length-num,num));
				index--;
				sortinfos[index] = infos[i];
				
			}
			MemoryStream stream = new MemoryStream(1024*1024);                         
			//int length = 0;
			//foreach(FieldInfo inf in infos)
			
			for (int i =0; i<infonum;i++)
			{
				if (!sortinfos[i].IsStatic)
				{
					if(sortinfos[i].FieldType ==typeof(byte))
					{
						byte[] buff = BitConverter.GetBytes((byte)sortinfos[i].GetValue(structure));
						stream.Write(buff,0,1);
					}
					else if(sortinfos[i].FieldType ==typeof(sbyte))
					{
						byte[] buff = BitConverter.GetBytes((sbyte)sortinfos[i].GetValue(structure));
						stream.Write(buff,0,1);
					}
					else if(sortinfos[i].FieldType ==typeof(short))
					{
						byte[] buff = BitConverter.GetBytes((short)sortinfos[i].GetValue(structure));
						stream.Write(buff,0,buff.Length);
					}
					else if(sortinfos[i].FieldType ==typeof(ushort))
					{
						byte[] buff = BitConverter.GetBytes((ushort)sortinfos[i].GetValue(structure));
						stream.Write(buff,0,buff.Length);
					}
					else if(sortinfos[i].FieldType ==typeof(int))
					{		
						byte[] buff = BitConverter.GetBytes((int)sortinfos[i].GetValue(structure));
						stream.Write(buff,0,buff.Length);
						//print(stream.Position);
					}
					else if(sortinfos[i].FieldType ==typeof(uint))
					{		
						byte[] buff = BitConverter.GetBytes((uint)sortinfos[i].GetValue(structure));
						 stream.Write(buff,0,buff.Length);
						//print(stream.Position);
					}
					else if(sortinfos[i].FieldType ==typeof(float))
					{
						byte[] buff = BitConverter.GetBytes((float)sortinfos[i].GetValue(structure));
						 stream.Write(buff,0,buff.Length);
					}
					else if(sortinfos[i].FieldType ==typeof(long))
					{
						byte[] buff = BitConverter.GetBytes((long)sortinfos[i].GetValue(structure));
						 stream.Write(buff,0,buff.Length);
					}
					else if(sortinfos[i].FieldType ==typeof(ulong))
					{
						byte[] buff = BitConverter.GetBytes((ulong)sortinfos[i].GetValue(structure));
						 stream.Write(buff,0,buff.Length);
					}
					else if(sortinfos[i].FieldType ==typeof(byte[]))
					{
					//	print(sortinfos[i].Name);
					//	string column ="L" + inf.Name.ToString();
						int len = (int) sortinfos[i-1].GetValue(structure);
						byte[] buff = (byte[])sortinfos[i].GetValue(structure);
						
						int size = buff.Length;
					    stream.Write(buff,0,size);
					    stream.Seek(len-size,SeekOrigin.Current);
					}
					else if(sortinfos[i].FieldType ==typeof(short[]))
					{
						int unit_len = 2;
						short[] array = (short[])sortinfos[i].GetValue(structure);
						int len = (int)sortinfos[i-1].GetValue(structure);
						int length = len*unit_len;
						byte[] buff = new byte[length];
						for (int n = 0; n < len; n++)  
				    	{  
							Array.Copy(BitConverter.GetBytes(array[n]),0,buff,n*unit_len,unit_len); 
				   	 	}  
						int size = buff.Length;
				    	stream.Write(buff,0,size);
					}
					else if(sortinfos[i].FieldType ==typeof(ushort[]))
					{
						int unit_len = 2;
						ushort[] array = (ushort[])sortinfos[i].GetValue(structure);
						int len = (int)sortinfos[i-1].GetValue(structure);
						int length = len*unit_len;
						byte[] buff = new byte[length];
						for (int n = 0; n < len; n++)  
				    	{  
							Array.Copy(BitConverter.GetBytes(array[n]),0,buff,n*unit_len,unit_len); 
				   	 	}  
						int size = buff.Length;
				    	stream.Write(buff,0,size);
					}
					else if(sortinfos[i].FieldType ==typeof(int[]))
					{
						int unit_len = 4;
						int[] array = (int[])sortinfos[i].GetValue(structure);
						int len = (int)sortinfos[i-1].GetValue(structure);
						int length = len*unit_len;
						byte[] buff = new byte[length];
						for (int n = 0; n < len; n++)  
				    	{  
							Array.Copy(BitConverter.GetBytes(array[n]),0,buff,n*unit_len,unit_len); 
				   	 	}  
						int size = buff.Length;
				    	stream.Write(buff,0,size);
					}
					else if(sortinfos[i].FieldType ==typeof(uint[]))
					{
						int unit_len = 4;
						uint[] array = (uint[])sortinfos[i].GetValue(structure);
						int len = (int)sortinfos[i-1].GetValue(structure);
						int length = len*unit_len;
						byte[] buff = new byte[length];
						for (int n = 0; n < len; n++)  
				    	{  
							Array.Copy(BitConverter.GetBytes(array[n]),0,buff,n*unit_len,unit_len); 
				   	 	}  
						int size = buff.Length;
				    	stream.Write(buff,0,size);
					}
					else if(sortinfos[i].FieldType ==typeof(long[]))
					{
						int unit_len = 8;
						long[] array = (long[])sortinfos[i].GetValue(structure);
						int len = (int)sortinfos[i-1].GetValue(structure);
						int length = len*unit_len;
						byte[] buff = new byte[length];
						for (int n = 0; n < len; n++)  
				    	{  
							Array.Copy(BitConverter.GetBytes(array[n]),0,buff,n*unit_len,unit_len); 
				   	 	}  
						int size = buff.Length;
				    	stream.Write(buff,0,size);
					}	
					else if(sortinfos[i].FieldType ==typeof(ulong[]))
					{
						int unit_len = 8;
						ulong[] array = (ulong[])sortinfos[i].GetValue(structure);
						int len = (int)sortinfos[i-1].GetValue(structure);
						int length = len*unit_len;
						byte[] buff = new byte[length];
						for (int n = 0; n < len; n++)  
				    	{  
							Array.Copy(BitConverter.GetBytes(array[n]),0,buff,n*unit_len,unit_len); 
				   	 	}  
						int size = buff.Length;
				    	stream.Write(buff,0,size);
					}
				}
			}
			int buffsize =  (int )stream.Position;
		   	byte[] buffer = new byte[buffsize];
			stream.Seek(0, SeekOrigin.Begin); 
			stream.Read(buffer,0,buffsize);
			
			return buffer;
	}
	//结构体反序列化 
	public static T  ByteToStruct<T>(byte[] dataBuffer)
	{
		 T structure= default(T);
		object obj = structure as object;
		Type type=typeof(T);
		FieldInfo [] infos=type.GetFields();
		int infonum = infos.Length;
		FieldInfo [] sortinfos = new FieldInfo[infonum];
		for (int i=0; i<infonum; i++)
		{
			string strinfo =  infos[i].ToString();
			int num = -1;
			char [] strchararray = new char[strinfo.Length];
			strchararray = strinfo.ToCharArray();
			for (num = 1; num <=strchararray.Length; num++)
			{
				if (!Char.IsNumber(strchararray[strchararray.Length-num]))
					break;
			}
			num--;
			int	index = Convert.ToInt32 (strinfo.Substring(strinfo.Length-num,num));
		
			index--;
			sortinfos[index] = infos[i];
		}
		int length = 0;
		for (int i =0; i<infonum;i++)
		{
			if (!sortinfos[i].IsStatic)
			{
				if (sortinfos[i].FieldType ==typeof(byte))
				{
					//这里特殊需要给BYTE两个字节的BUFF  
					byte[] bytebuff = new byte[2];
					Array.Copy(dataBuffer,length,bytebuff,0,1);
					byte val=(byte)BitConverter.ToChar(bytebuff,0);
					sortinfos[i].SetValue(obj,val);
					length+=1;
				}
				else if(sortinfos[i].FieldType ==typeof(sbyte))
				{
					//这里特殊需要给BYTE两个字节的BUFF  
					byte[] bytebuff = new byte[2];
					Array.Copy(dataBuffer,length,bytebuff,0,1);
					sbyte val=(sbyte)BitConverter.ToChar(bytebuff,0);
					sortinfos[i].SetValue(obj,val);
					length+=1;
				}
				else if (sortinfos[i].FieldType ==typeof(short))
				{
					 short val=BitConverter.ToInt16(dataBuffer,length);
					 sortinfos[i].SetValue(obj,val);
					 length+=2;
				}
				else if (sortinfos[i].FieldType ==typeof(ushort))
				{
					 ushort val=BitConverter.ToUInt16(dataBuffer,length);
					 sortinfos[i].SetValue(obj,val);
					 length+=2;
				}
				else if (sortinfos[i].FieldType ==typeof(int))
				{
					 int val=BitConverter.ToInt32(dataBuffer,length);
					 sortinfos[i].SetValue(obj,val);
					 length+=4;
				}
				else if (sortinfos[i].FieldType ==typeof(uint))
				{
					uint val=BitConverter.ToUInt32(dataBuffer,length);
					sortinfos[i].SetValue(obj,val);
					length+=4;
				}
				else if (sortinfos[i].FieldType ==typeof(long))
				{
					long val=BitConverter.ToInt64(dataBuffer,length);
					sortinfos[i].SetValue(obj,val);
					length+=8;
				}
				else if (sortinfos[i].FieldType ==typeof(ulong))
				{
					 ulong val=BitConverter.ToUInt64(dataBuffer,length);
					 sortinfos[i].SetValue(obj,val);
					 length+=8;
				}
				else if (sortinfos[i].FieldType ==typeof(float))
				{
					 float val=BitConverter.ToSingle(dataBuffer,length);
					 sortinfos[i].SetValue(obj,val);
					 length+=4;
				}
				else if(sortinfos[i].FieldType ==typeof(byte[]))
				{
					
					int len = (int) sortinfos[i-1].GetValue(obj);
					byte[] buff = new byte[len];
					Array.Copy(dataBuffer,length,buff,0,len);
					sortinfos[i].SetValue(obj,buff);
					length += len;
				}
				else if(sortinfos[i].FieldType ==typeof(short[]) || sortinfos[i].FieldType ==typeof(ushort[]) 
					|| sortinfos[i].FieldType ==typeof(int[]) || sortinfos[i].FieldType ==typeof(uint[]) 
					|| sortinfos[i].FieldType ==typeof(long[]) || sortinfos[i].FieldType ==typeof(ulong[]))//简单变量数组 
				{
					int unit_len = 0;
					if(sortinfos[i].FieldType ==typeof(short[]) || sortinfos[i].FieldType ==typeof(ushort[]))
					{
							unit_len =2;
							
					}
					else if(sortinfos[i].FieldType ==typeof(int[]) || sortinfos[i].FieldType ==typeof(uint[]))
					{
							unit_len =4;
							
					}
					else if(sortinfos[i].FieldType ==typeof(long[]) || sortinfos[i].FieldType ==typeof(ulong[]))
					{
							unit_len =8;
							
					}
					int len = (int) sortinfos[i-1].GetValue(obj);
					len *=unit_len;
					byte[] buff = new byte[len];
					Array.Copy(dataBuffer,length,buff,0,len);
					sortinfos[i].SetValue(obj,ArrayByteToVariable(buff,sortinfos[i].FieldType));
					length += len;
				}
			}
		}
		structure = (T)obj; 
		return  structure;
	}
	//结构体反序列化 另外获取到结构体SIZE大小 为结构体数组反序列化提供服务 
	public static T  ByteToStruct<T>(byte[] dataBuffer,ref int size)
	{
		 T structure= default(T);
		object obj = structure as object;
		Type type=typeof(T);
		FieldInfo [] infos=type.GetFields();
		int infonum = infos.Length;
		FieldInfo [] sortinfos = new FieldInfo[infonum];
		for (int i=0; i<infonum; i++)
		{
			string strinfo =  infos[i].ToString();
			int num = -1;
			char [] strchararray = new char[strinfo.Length];
			strchararray = strinfo.ToCharArray();
			for (num = 1; num <=strchararray.Length; num++)
			{
				if (!Char.IsNumber(strchararray[strchararray.Length-num]))
					break;
			}
			num--;
			int	index = Convert.ToInt32(strinfo.Substring(strinfo.Length-num,num));
			index--;
			sortinfos[index] = infos[i];
		}
		int length = 0;
		for (int i =0; i<infonum;i++)
		{
			if (!sortinfos[i].IsStatic)
			{
				if (sortinfos[i].FieldType ==typeof(byte))
				{
						//这里特殊需要给BYTE两个字节的BUFF 
						 byte[] bytebuff = new byte[2];
						Array.Copy(dataBuffer,length,bytebuff,0,1);
						 byte val=(byte)BitConverter.ToChar(bytebuff,0);
    					 sortinfos[i].SetValue(obj,val);
   						 length+=1;
				}
				else if(sortinfos[i].FieldType ==typeof(sbyte))
				{
					//这里特殊需要给BYTE两个字节的BUFF  
					byte[] bytebuff = new byte[2];
					Array.Copy(dataBuffer,length,bytebuff,0,1);
					sbyte val=(sbyte)BitConverter.ToChar(bytebuff,0);
					sortinfos[i].SetValue(obj,val);
					length+=1;
				}
				else if (sortinfos[i].FieldType ==typeof(short))
				{
						 short val=BitConverter.ToInt16(dataBuffer,length);
    					 sortinfos[i].SetValue(obj,val);
   						 length+=2;
				}
				else if (sortinfos[i].FieldType ==typeof(ushort))
				{
						 ushort val=BitConverter.ToUInt16(dataBuffer,length);
    					 sortinfos[i].SetValue(obj,val);
   						 length+=2;
				}
				else if (sortinfos[i].FieldType ==typeof(int))
				{
						 int val=BitConverter.ToInt32(dataBuffer,length);
    					 sortinfos[i].SetValue(obj,val);
   						 length+=4;
				}
				else if (sortinfos[i].FieldType ==typeof(uint))
				{
						uint val=BitConverter.ToUInt32(dataBuffer,length);
    					 sortinfos[i].SetValue(obj,val);
   						 length+=4;
				}
				else if (sortinfos[i].FieldType ==typeof(long))
				{
						 long val=BitConverter.ToInt64(dataBuffer,length);
    					 sortinfos[i].SetValue(obj,val);
   						 length+=8;
				}
				else if (sortinfos[i].FieldType ==typeof(ulong))
				{
						 ulong val=BitConverter.ToUInt64(dataBuffer,length);
    					 sortinfos[i].SetValue(obj,val);
   						 length+=8;
				}
				else if (sortinfos[i].FieldType ==typeof(float))
				{
						 float val=BitConverter.ToSingle(dataBuffer,length);
    					 sortinfos[i].SetValue(obj,val);
   						 length+=4;
				}
				else if(sortinfos[i].FieldType ==typeof(byte[]))
				{
						int len = (int) sortinfos[i-1].GetValue(obj);
						byte[] buff = new byte[len];
						Array.Copy(dataBuffer,length,buff,0,len);
						sortinfos[i].SetValue(obj,buff);
						length += len;
				}
				else if(sortinfos[i].FieldType ==typeof(short[]) || sortinfos[i].FieldType ==typeof(ushort[]) 
					|| sortinfos[i].FieldType ==typeof(int[]) || sortinfos[i].FieldType ==typeof(uint[]) 
					|| sortinfos[i].FieldType ==typeof(long[]) || sortinfos[i].FieldType ==typeof(ulong[]))//简单变量数组 
				{
					int unit_len = 0;
					if(sortinfos[i].FieldType ==typeof(short[]) || sortinfos[i].FieldType ==typeof(ushort[]))
					{
							unit_len =2;
							
					}
					else if(sortinfos[i].FieldType ==typeof(int[]) || sortinfos[i].FieldType ==typeof(uint[]))
					{
							unit_len =4;
							
					}
					else if(sortinfos[i].FieldType ==typeof(long[]) || sortinfos[i].FieldType ==typeof(ulong[]))
					{
							unit_len =8;
							
					}
					int len = (int) sortinfos[i-1].GetValue(obj);
					len *=unit_len;
					byte[] buff = new byte[len];
					Array.Copy(dataBuffer,length,buff,0,len);
					sortinfos[i].SetValue(obj,ArrayByteToVariable(buff,sortinfos[i].FieldType));
					length += len;
				}
			}
		}
		size =length;
		structure = (T)obj; 
		return  structure;
	}

	
	//简单变量数组反序列化  
	public static object ArrayByteToVariable(byte[] bytearray,Type type)
	{
		int unit_len = 0;
		if(type == typeof(short[]))
		{
			unit_len = 2;
			short [] shortarray = new short[bytearray.Length/unit_len];
			byte[] temparray = new byte[unit_len];
			for (int i = 0; i < shortarray.Length; i++)  
		    {  
				Array.Copy(bytearray,i*unit_len,temparray,0,unit_len);
		        shortarray[i] =(short)(BitConverter.ToInt16(temparray,0));  
		    }  
			return shortarray;
		}
		else if(type == typeof(ushort[]))
		{
			unit_len = 2;
			ushort [] shortarray = new ushort[bytearray.Length/unit_len];
			byte[] temparray = new byte[unit_len];
			for (int i = 0; i < shortarray.Length; i++)  
		    {  
				Array.Copy(bytearray,i*unit_len,temparray,0,unit_len);
		        shortarray[i] =(ushort)(BitConverter.ToInt16(temparray,0));  
		    }  
			return shortarray;
		}
		else if(type == typeof(int[]))
		{
			unit_len= 4;
			int [] intarray = new int[bytearray.Length/unit_len];
			byte[] temparray = new byte[unit_len];
			for (int i = 0; i < intarray.Length; i++)  
		    {  
				Array.Copy(bytearray,i*unit_len,temparray,0,unit_len);
		        intarray[i] =(int)( BitConverter.ToUInt32(temparray,0));  
		    }  
			return intarray;
		}
		else if(type == typeof(uint[]))
		{
			unit_len= 4;
			uint [] intarray = new uint[bytearray.Length/unit_len];
			byte[] temparray = new byte[unit_len];
			for (int i = 0; i < intarray.Length; i++)  
		    {  
				Array.Copy(bytearray,i*unit_len,temparray,0,unit_len);
		        intarray[i] =(uint)( BitConverter.ToUInt32(temparray,0));  
		    }  
			return intarray;
		}
		else if(type == typeof(long[]))
		{
			unit_len = 8;
			long[] longarray = new long[bytearray.Length/unit_len];
			byte[] temparray = new byte[unit_len];
			for (int i = 0; i < longarray.Length; i++)  
		    {  
				Array.Copy(bytearray,i*unit_len,temparray,0,unit_len);
				longarray[i] =(long)( BitConverter.ToInt64(temparray,0));
		    }  
			return longarray;
		}
		else if(type == typeof(ulong[]))
		{
			unit_len = 8;
			ulong[] longarray = new ulong[bytearray.Length/unit_len];
			byte[] temparray = new byte[unit_len];
			for (int i = 0; i < longarray.Length; i++)  
		    {  
				Array.Copy(bytearray,i*unit_len,temparray,0,unit_len);
				longarray[i] =(ulong)( BitConverter.ToUInt64(temparray,0));
		    }  
			return longarray;
		}
		return null;
	}
	//结构体数组序列化 
	public static byte[] StructArrayToByte<T>(T[] structarray)
	{
		MemoryStream stream = new MemoryStream(1024*1024); 
		for(int i=0,len=structarray.Length;i<len;i++)
		{
			byte[] tmp_buff = StructToByte<T>(structarray[i]);
			int size = tmp_buff.Length;
			stream.Write(tmp_buff,0,size);
		}
		int buffsize =  (int )stream.Position;
	   	byte[] buffer = new byte[buffsize];
		stream.Seek(0, SeekOrigin.Begin); 
		stream.Read(buffer,0,buffsize);
		
		return buffer;
	}
	//结构体数组反序列化 
	public static T[]  ByteToStructArray<T>(byte[] dataBuffer,int num)
	{
		int offset =0;// U3dCmn.GetSizeofSimpleStructure<T>();
		T[] structarray = new T[num];
		for(int i =0; i<num; i++)
		{
			int len = 0;
			byte[] SubBuff = new byte[dataBuffer.Length-offset];
			Array.Copy(dataBuffer,offset,SubBuff,0,dataBuffer.Length-offset);
			structarray[i] = ByteToStruct<T>(SubBuff,ref len);
			offset += len;
			
		}
		return structarray;
	}
	//根据内存偏移对结构体数组反序列化 
	public static T[] ByteToStructArrayByOffset<T>(byte[] buff,int num,int offset)
	{
		//int head_len = U3dCmn.GetSizeofSimpleStructure<T>();
		int data_len = buff.Length - offset;
		byte[] data_buff = new byte[data_len];
		Array.Copy(buff,offset,data_buff,0,data_len);
		return ByteToStructArray<T>(data_buff,num);
	}
	//int转ip地址 
	public static string IntToIP(int ip_int)
	{
		int seg1 = (int)(ip_int &0xff000000)>>24;
		if (seg1 < 0)
			 seg1 += 0x100;
		int seg2 =  (int)(ip_int & 0x00ff0000) >> 16;
		if (seg2 < 0)
			seg2 += 0x100;
		int seg3 = (int)(ip_int & 0x0000ff00) >> 8;
		if (seg3 < 0)
			seg2 += 0x100; 
		int seg4 = (int)(ip_int & 0x000000ff);
		if (seg4 < 0)
			seg2 += 0x100; 
		string ip_str = seg4.ToString()+"."+ seg3.ToString()+"."+ seg2.ToString()+"."+ seg1.ToString();
		return ip_str;
	}
	public static uint[] crc_32_tab = {	
		0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419,	
		0x706af48f, 0xe963a535, 0x9e6495a3, 0x0edb8832, 0x79dcb8a4,	
		0xe0d5e91e, 0x97d2d988, 0x09b64c2b, 0x7eb17cbd, 0xe7b82d07,	
		0x90bf1d91, 0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de,	
		0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7, 0x136c9856,	
		0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9,	
		0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4,	
		0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,	
		0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3,
		0x45df5c75, 0xdcd60dcf, 0xabd13d59, 0x26d930ac, 0x51de003a,	
		0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599,	
		0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,	
		0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 0x76dc4190,	
		0x01db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f,	
		0x9fbfe4a5, 0xe8b8d433, 0x7807c9a2, 0x0f00f934, 0x9609a88e,	
		0xe10e9818, 0x7f6a0dbb, 0x086d3d2d, 0x91646c97, 0xe6635c01,	
		0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed,	
		0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950,	
		0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3,	
		0xfbd44c65, 0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2,	
		0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a,	
		0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5,	
		0xaa0a4c5f, 0xdd0d7cc9, 0x5005713c, 0x270241aa, 0xbe0b1010,	
		0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,	
		0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17,	
		0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6,	
		0x03b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x04db2615,	
		0x73dc1683, 0xe3630b12, 0x94643b84, 0x0d6d6a3e, 0x7a6a5aa8,	
		0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1, 0xf00f9344,	
		0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb,
		0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a,	
		0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,	
		0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1,	
		0xa6bc5767, 0x3fb506dd, 0x48b2364b, 0xd80d2bda, 0xaf0a1b4c,	
		0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef,	
		0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,	
		0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe,	
		0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31,	
		0x2cd99e8b, 0x5bdeae1d, 0x9b64c2b0, 0xec63f226, 0x756aa39c,	
		0x026d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x05005713,	
		0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38, 0x92d28e9b,	
		0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242,	
		0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1,	
		0x18b74777, 0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c,	
		0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45, 0xa00ae278,	
		0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7,	
		0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc, 0x40df0b66,	
		0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,	
		0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605,	
		0xcdd70693, 0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8,	
		0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b,	
		0x2d02ef8d
	};
	public static int	CRC32(byte[] s,int len)
	{
		if( len<0 )
		{				
			return	0;	
		}	
		uint c; 	
		uint crc = (uint)0xffffffff; 
		if (s.Length == 0) 
		{		
			c = 0xffffffff;	
		} 
		else 
		{		
			c = crc;	
			int i =0;
			if (len>0) do 
			{			
				c = crc_32_tab[((int)c ^ (s[i])) & 0xff] ^ (c >> 8);	
				i++;
			} while (--len>0);	
		}	
		crc = c;	
		return (int)(c ^ 0xffffffff);
	}
	//生成crc8校验码 
	public static byte	CRC8(byte[] buff)
	{	
		int tmp	= CRC32(buff,buff.Length);	
		byte[] array = BitConverter.GetBytes(tmp);
		byte rst = (byte)(array[0] ^ array[1] ^ array[2] ^ array[3]);
		return	rst;
	}
	//public static int GetLenByName(object obj,FieldInfo [] infos,string name)
	//{
	//	foreach(FieldInfo inf in infos)
	//	{
	//			if (inf.Name.ToString() == name)
	//				return (int)inf.GetValue(obj);
	//	}
	//	return 0;
//	}
}
