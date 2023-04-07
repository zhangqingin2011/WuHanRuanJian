using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public class EVENTDEF
    {
       public const Int32 MAX_EVENT_NUM =  512 ;
       public const Int32 MAX_RESERVE_DATA_LEN =  128 ;
       public const Int32 EV_SRC_SYS =  0x010  ;//  系统事件 
       public const Int32 EV_SRC_CH0 =  0x100  ;//  通道0事件  0x100~0x10f 
       public const Int32 EV_SRC_MDI =  0x110  ;//  MDI的事件 
       public const Int32 EV_SRC_KBD =  0x200  ;//  键盘事件 
       public const Int32 EV_SRC_AX0 =  0x300  ;//  轴事件 
       public const Int32 EV_SRC_NET =  0x400  ;//网络事件 
       public const Int32 ncEvtPrgStart =  0xa001  ;//  程序启动 
       public const Int32 ncEvtPrgEnd =  0xa002  ;//  程序结束 
       public const Int32 ncEvtPrgHold =  0xa003  ;//  Hold完成 
       public const Int32 ncEvtPrgBreak =  0xa004  ;//  break完成 
       public const Int32 ncEvtG92Fin =  0xa005  ;//  G92完成 
       public const Int32 ncEvtRstFin =  0xa006  ;//  上电复位完成 
       public const Int32 ncEvtRwndFin =  0xa007  ;//  重运行完成 
       public const Int32 ncEvtMdiRdy =  0xa008  ;//  MDI准备好 
       public const Int32 ncEvtMdiExit =  0xa009  ;//  MDI退出 
       public const Int32 ncEvtMdiAck =  0xa00a  ;//  MDI行解释完成 
       public const Int32 ncEvtRunStart =  0xa00b  ;//  程序运行 
       public const Int32 ncEvtRunRowAck =  0xa00d  ;//  任意行请求应答 
       public const Int32 ncEvtRunRowRdy =  0xa00e  ;//  任意行准备好 
       public const Int32 ncEvtBpSaved =  0xa011  ;//  断点保存完成 
       public const Int32 ncEvtBpResumed =  0xa012  ;//  断点恢复完成 
       public const Int32 ncEvtIntvHold =  0xa013  ;//  执行到M92等待用户干预 
       public const Int32 ncEvtEstop =  0xa014  ;//  外部急停 
       public const Int32 ncEvtLoadOK =  0xa015  ;//  程序加载完成 
       public const Int32 ncEvtSyntax1 =  0xa016  ;//  第一类语法错【修改后可接着运行】 
       public const Int32 ncEvtSyntax2 =  0xa017  ;//  第二类语法错【修改后从头运行】 
       public const Int32 ncEvtGcodeSave =  0xa018  ;//  程序中的数据保存指令 
       public const Int32 ncEvtLoadData =  0xa019  ;//  程序中的数据加载指令 
       public const Int32 ncEvtChgTool =  0xa01a  ;//  G代码修改了刀具数据 
       public const Int32 ncEvtChgCrds =  0xa01b  ;//  G代码修改了坐标系数据 
       public const Int32 ncEvtChgAxes =  0xa01c  ;//  通道轴组发生了改变 
       public const Int32 ncEvtChgVar =  0xa01d  ;//  G代码修改变量 
       public const Int32 ncEvtNckNote =  0xa01e  ;//  通道提示 
       public const Int32 ncEvtNckAlarm =  0xa01f  ;//  通道报警 
       public const Int32 ncEvtStopAck =  0xa020  ;//  sys_stop_prog完成 
       public const Int32 ncEvtRandomRowFinish =  0xa027  ;//任意行扫描完成 
       public const Int32 ncEVtRandomRowRequestProg =  0xa029  ;//任意行扫描模式请求程序管理器准备好程序 
       public const Int32 ncEvtFaultIrq =  0xa030  ;//  故障中断 
       public const Int32 ncEvtPackFin =  0xa040  ;//  数据打包完成 
       public const Int32 ncEvtAlarmChg =  0xa055  ;//  报警产生或消除 
       public const Int32 ncEvtFileChg =  0xa056  ;//  文件修改 
       public const Int32 ncEvtConnect =  0xa060  ;//  nc连接 
       public const Int32 ncEvtDisConnect =  0xa061  ;//  nc断开连接 
       public const Int32 ncEvtFileSend =  0xa062  ;//  传送文件完毕 
       public const Int32 ncEvtMaxEncPos =  0xa201  ;//  轴编码器初始位置过大 
       public const Int32 ncEvtMaxACC =  0xa202  ;//  轴加速度过大 
       public const Int32 ncEvtPoweroff =  0xa800  ;//  系统断电 
       public const Int32 ncEvtSaveData =  0xa801  ;//  保存系统数据 
       public const Int32 ncEvtSysExit =  0xa802  ;//  系统退出 
       public const Int32 ncEvtUserStart =  0xb000  ;//  用户自定义事件  保留100个 
       public const Int32 ncEvtUserFunc1 =  ncEvtUserStart +  100  ;//  event  100  对应用户按键调用指定程序 
       public const Int32 ncEvtUserFunc2 =  ncEvtUserStart +  101  ;//  event  100  对应用户按键调用指定程序 
       public const Int32 ncEvtHardRstFin =  ncEvtUserStart +  102  ;//  硬复位完成 
       public const Int32 ncEvtSaveRegB =  ncEvtUserStart +  103  ;//  保存B寄存器 
       public const Int32 ncEvtUserReqChn =  ncEvtUserStart +  104  ;//  请求切换通道 
       public const Int32 ncEvtUserReqMsk =  ncEvtUserStart +  105  ;//  请求屏蔽通道 
       public const Int32 ncEvtReserve0 =  ncEvtUserStart +  106  ;//  用户保留PLC事件 
       public const Int32 ncEvtReserve1 =  ncEvtUserStart +  107  ;//  用户保留PLC事件 
       public const Int32 ncEvtReserve2 =  ncEvtUserStart +  108  ;//  用户保留PLC事件 
       public const Int32 ncEvtReserve3 =  ncEvtUserStart +  109  ;//  用户保留PLC事件 
       public const Int32 ncEvtChFin0 =  ncEvtUserStart +  110  ;//  通道0加工完成 
       public const Int32 ncEvtChFin1 =  ncEvtUserStart +  111  ;//  通道1加工完成 
       public const Int32 ncEvtChFin2 =  ncEvtUserStart +  112  ;//  通道2加工完成 
       public const Int32 ncEvtChFin3 =  ncEvtUserStart +  113  ;//  通道3加工完成 
       public const Int32 ncEvtChFin =  ncEvtUserStart +  114  ;//  加工完成事件 
       public const Int32 ncEvtRstMsg =  ncEvtUserStart +  120  ;//  复位消息 
       public const Int32 ncEvtRigChg =  ncEvtUserStart +  121  ;//  钥匙锁切换权限消息 
       public const Int32 ncEvtRfidReadTag =  ncEvtUserStart +  122  ;//  RFID读电子标签数据到CNC 
       public const Int32 ncEvtRfidWriteTag =  ncEvtUserStart +  123  ;//  RFID写CNC数据到电子标签 
    }
}
