using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.NewApp
{
    public class PositionBind
    {
        /// <summary>
        /// 出入库位置绑定
        /// </summary>
        public int wMSPositionBind;
        /// <summary>
        /// 出入库位置绑定
        /// </summary>
        public int wMSPositionBindLock;
        /// <summary>
        /// AGV绑定
        /// </summary>
        public int aGVBind;
        /// <summary>
        /// 加工单元1位置1绑定
        /// </summary>
        public int process1Position1Bind;
        /// <summary>
        /// 加工单元1位置1锁定
        /// </summary>
        public int process1Position1BindLock;
        /// <summary>
        /// 加工单元1位置2绑定
        /// </summary>
        public int process1Position2Bind;
        /// <summary>
        /// 加工单元1位置2锁定
        /// </summary>
        public int process1Position2BindLock;
        /// <summary>
        /// 加工单元2位置1绑定
        /// </summary>
        public int process2Position1Bind;
        /// <summary>
        /// 加工单元2位置1锁定
        /// </summary>
        public int process2Position1BindLock;
        /// <summary>
        /// 加工单元2位置2绑定
        /// </summary>
        public int process2Position2Bind;
        /// <summary>
        /// 加工单元2位置2锁定
        /// </summary>
        public int process2Position2BindLock;
        /// <summary>
        /// 加工单元3位置1绑定
        /// </summary>
        public int process3Position1Bind;
        /// <summary>
        /// 加工单元3位置1锁定
        /// </summary>
        public int process3Position1BindLock;
        /// <summary>
        /// 加工单元3位置2绑定
        /// </summary>
        public int process3Position2Bind;
        /// <summary>
        /// 加工单元3位置2锁定
        /// </summary>
        public int process3Position2BindLock;
        /// <summary>
        /// 加工单元3位置1绑定
        /// </summary>
        public int process4Position1Bind;
        /// <summary>
        /// 加工单元4位置1锁定
        /// </summary>
        public int process4Position1BindLock;
        /// <summary>
        /// 加工单元4位置2绑定
        /// </summary>
        public int process4Position2Bind;
        /// <summary>
        /// 加工单元4位置2锁定
        /// </summary>
        public int process4Position2BindLock;
        /// <summary>
        /// 清洗单元位置1绑定
        /// </summary>
        public int cleanPosition1Bind;

        /// <summary>
        /// 清洗单元位置1绑定
        /// </summary>
        public int cleanPosition1BindLock;
        /// <summary>
        /// 清洗单元位置2绑定
        /// </summary
        public int cleanPosition2Bind;
        /// <summary>
        /// 清洗单元位置2绑定
        /// </summary
        public int cleanPosition2BindLock;
        /// <summary>
        /// 检测1单元位置1绑定
        /// </summary>
        public int check1Position1Bind;
        /// <summary>
        /// 检测1单元位置2绑定
        /// </summary
        public int check1Position2Bind;
        /// <summary>
        /// 检测2单元位置1绑定
        /// </summary>
        public int check2Position1Bind;
        /// <summary>
        /// 检测2单元位置2绑定
        /// </summary
        public int check2Position2Bind;
        /// <summary>
        /// 检测1单元位置1绑定
        /// </summary>
        public int fit1Position1Bind;
        /// <summary>
        /// 检测1单元位置1绑定
        /// </summary>
        public int fit1Position1BindLock;
        /// <summary>
        /// 装配1单元位置2绑定
        /// </summary
        public int fit1Position2Bind;
        /// <summary>
        /// 装配1单元位置2绑定
        /// </summary
        public int fit1Position2BindLock;
        /// <summary>
        /// 装配2单元位置1绑定
        /// </summary>
        public int fit2Position1Bind;
        /// <summary>
        /// 装配2单元位置1绑定
        /// </summary>
        public int fit2Position1BindLock;
        /// <summary>
        /// 装配2单元位置2绑定
        /// </summary
        public int fit2Position2Bind;
        /// <summary>
        /// 装配2单元位置2绑定
        /// </summary
        public int fit2Position2BindLock;

        public PositionBind()
        {
            wMSPositionBind = 0;
            aGVBind = 0;
            process1Position1Bind = 0;
            process1Position2Bind = 0;
            process2Position1Bind = 0;
            process2Position2Bind = 0;
            process3Position1Bind = 0;
            process3Position2Bind = 0;
            process4Position1Bind = 0;
            process4Position2Bind = 0;
            cleanPosition1Bind = 0;
            cleanPosition1Bind = 0;
            check1Position1Bind = 0;
            check1Position2Bind = 0;
            check2Position1Bind = 0;
            check2Position2Bind = 0;
            fit1Position1Bind = 0;
            fit1Position2Bind = 0;
            fit2Position1Bind = 0;
            fit2Position2Bind = 0;
            process1Position1BindLock = 0;
            process1Position2BindLock = 0;
            process2Position1BindLock = 0;
            process2Position2BindLock = 0;
            process3Position1BindLock = 0;
            process3Position2BindLock = 0;
            process4Position1BindLock = 0;
            process4Position2BindLock = 0;
           
        }
    }
}
