//		PLU发送DLL库
//
//	通过一组Dhset.exe设置好的ini文件
//	下载其PLU到秤
//				Roya	07.4.9
#ifdef	DHSPN_EXPORTS
#define DHSPN __declspec(dllexport)
#else
#define DHSPN __declspec(dllimport)
#endif


//	返回值说明
//	0	成功
//	-1	dhip.ini为空或者设置错误
//	-2	dhpluversion.int为空或设置错误
//	-3	dhplupathname.ini为空或设置错误
//	-4	dhplupathname.ini中读取的Plu路径不对,所指向的Plu文件不存在
//	-5	Plu文本为空或者设置错误(如:Plu版本与Plu文本内容不一致等)
//	-6	通信中存在异常,查看具体信息请读取Return.txt;

//	Return.txt中具体异常情况说明
//	0	成功
//	-1	连接失败
//	-2	发送后,未收到返回信息
//	-3	读缓冲区失败
//	-4	ip地址转换失败
//	-10	发送后未收到正确返回信息(0d,0a,03结束标志)

DHSPN	int WINAPI	dhSendPluDefault();