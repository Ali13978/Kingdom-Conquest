//#define PLATFORM_ANDROID
#define	PLATFORM_IOS

// if it is inner version, comment the define below
//#define	VERSION_91

using UnityEngine;
using System.Runtime.InteropServices;

public class API91 : APIBase {
#if VERSION_91
#if PLATFORM_IOS
	[DllImport("__Internal")]
	private static extern int init91(string obj, string method);
	[DllImport("__Internal")]
	private static extern int setcallback91(string obj, string method);
	[DllImport("__Internal")]
	private static extern int cmd91(string cmd, string param);
	
	protected override int do_init(string obj, string method)
	{
		return init91(obj, method);;
	}
	protected override int do_setcallback(string obj, string method)
	{
		return setcallback91(obj, method);
	}
	protected override int do_cmd(string cmd, string param)
	{
		return cmd91(cmd, param);
	}
#endif
#if PLATFORM_ANDROID
	[DllImport("_API91")]
	private static extern int init91(string obj, string method);
	[DllImport("_API91")]
	private static extern int setcallback91(string obj, string method);
	[DllImport("_API91")]
	private static extern int cmd91(string cmd, string param);
#endif
#endif
}
