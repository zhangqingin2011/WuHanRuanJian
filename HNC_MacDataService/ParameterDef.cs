﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNC_MacDataService
{
    /// <summary>
    /// CNC参数的相关定义和公共方法。ps:当前只存一份参数映射，当发现dbNo更换或dbNo未初始化时更新
    /// </summary>
    public class ParameterDef
    {
        public static String[] names = { "NC参数", "机床用户参数", "通道参数", "坐标轴参数", "误差补偿参数", "设备接口参数", "数据表参数" };
        public static int[] subClass = { 1, 1, 4, 32, 32, 80, 1 };
        public static String[] subClassName = { "", "", "通道", "逻辑轴", "补偿轴", "设备", "" };
        public static String[] keyType = { "NC", "MAC", "CH", "Axis", "ACMP", "CFG", "TABLE" };
        public static String[][][] subClassRows = new String[7][][];
        public static int[] mainClassStart = { 0, 10000, 40000, 100000, 300000, 500000, 700000 };
        public static int[] subClassInterval = { 0, 0, 1000, 1000, 1000, 1000, 0 };
        public static int[] isDbInited = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int lastDbNo = -1;
        /// <summary>
        /// 根据dbNo，建立大类、小类以及第n行的与redis参数id的映射表
        /// </summary>
        /// <param name="dbNo"></param>
        /// <returns></returns>
        public static int InitParams(int dbNo)
        {
            int ret = -1;
            if (dbNo >= 0 && dbNo < 24)//编号0~23
            {
                if (lastDbNo != dbNo || (isDbInited[dbNo] == 0 && checkParameterContent(dbNo)))// 如果未初始化
                {
                    for (int i = 0; i < keyType.Length; i++)
                    {
                        String[] fields = MacDataService.GetInstance().GetHashFields(dbNo, "Parameter:" + keyType[i]);
                        if (fields != null)
                        {
                            int subClassLength = subClass[i];
                            int sumLength = fields.Length;
                            subClassRows[i] = new string[subClassLength][];
                            Array.Sort(fields);
                            int subRowLength = sumLength / subClassLength;
                            for (int j = 0; j < subClassLength; j++)
                            {
                                subClassRows[i][j] = new String[subRowLength];
                                Array.Copy(fields, subRowLength * j, subClassRows[i][j], 0, subRowLength);
                            }
                            ret = 0;
                        }
                        else
                        {
                            ret = -1;
                        }
                        ret += ret;
                    }
                    if(ret == 0)
                    {
                        isDbInited[dbNo] = 1;
                        lastDbNo = dbNo;
                    }
                }
                else if(isDbInited[dbNo] == 1)//如果已初始化
                {
                    ret = 0;
                }
            }
            return ret;
        }

        /// <summary>
        /// 检查参数是否已完整采集
        /// </summary>
        /// <param name="dbNo"></param>
        /// <returns></returns>
        public static bool checkParameterContent(int dbNo)
        {
            bool isRight = true;
            for (int i = 0; i < keyType.Length; i++)
            {
                String[] fields = MacDataService.GetInstance().GetHashFields(dbNo, "Parameter:" + keyType[i]);
                if (fields == null)
                {
                    isRight = false;
                }
            }
            return isRight;
        }

        /// <summary>
        /// 通过大类、小类以及第row行，获得redis中的field值
        /// </summary>
        /// <param name="fileNo"></param>
        /// <param name="recNo"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static int GetParamanId(int fileNo, int recNo, int row, ref String field)
        {
            int ret = -1;
            if (subClassRows[fileNo][recNo][row] != null)
            {
                field = subClassRows[fileNo][recNo][row];
                ret = 0;
            }
            return ret;
        }

        /// <summary>
        /// 根据大类、小类以及小类里的id获得redis中的field值
        /// </summary>
        /// <param name="filenum"></param>
        /// <param name="subclass"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static String GetParamanIdField(int filenum, int subclass, int id)
        {
            int paramanId = mainClassStart[filenum] + subClassInterval[filenum] * subclass + id;
            return paramanId.ToString("D6");
        }
    }
}
