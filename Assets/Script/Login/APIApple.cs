//将来给上appstore的时候就把91Manager的script改成这个，同时把在输出外服版本的时候需要打开VERSION_APPLE的定义 
#define	VERSION_APPLE 

using UnityEngine;
using System.Runtime.InteropServices;

// 将来给上apple appstore的时候就把91Manager的script改成这个 

public class APIApple : APIBase {
	/*#if VERSION_APPLE
		[DllImport("__Internal")]
		private static extern int init_apple(string obj, string method);
		[DllImport("__Internal")]
		private static extern int setcallback_apple(string obj, string method);
		[DllImport("__Internal")]
		private static extern int cmd_apple(string cmd, string param);

		protected override int do_init(string obj, string method)
		{
			return init_apple(obj, method);;
		}
		protected override int do_setcallback(string obj, string method)
		{
			return setcallback_apple(obj, method);
		}
		protected override int do_cmd(string cmd, string param)
		{
			return cmd_apple(cmd, param);
		}
	#endif*/
}
