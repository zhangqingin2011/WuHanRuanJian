using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum TimePeriod
    {
    	HEARTBEAT_TIME = 0,// ��������
    	HEARTBEAT_TIMEOUT_COUNT,	   // ������ʱ����
    	API_TIMEOUT_TIME,              // Ĭ�Ͻӿڳ�ʱʱ��
    	PARMSAVE_TIMEOUT_TIME,         // �������泬ʱʱ��
    	PARMSAVEAS_TIMEOUT_TIME,       // �������Ϊ��ʱʱ��
    	BACKUP_TIMEOUT_TIME,           // ���ݳ�ʱʱ��
    	UPDATE_TIMEOUT_TIME,           // ������ʱʱ��
    	FILE_CHECK_TIMEOUT_TIME,       // �ļ�У�鳬ʱʱ��
    };

    public class HNCNET
    {
       public const Int32 VERSION_LEN =  32 ;
    }
}
