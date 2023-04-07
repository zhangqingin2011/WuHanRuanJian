using System;
using System.Collections.Generic;
using System.Linq;

namespace SCADA
{
    public static class MMCSDbInitializer
    {
        /// <summary>
        /// 初始化数据库（系统基础数据，不包含业务数据）
        /// </summary>
        /// <param name="force">是否强制重新创建数据库</param>
        public static void Initialize(bool force = false, MMCSDbContext context = null)
        {
            context = context ?? new MMCSDbContext();
            if (force)
            {
                context.Database.EnsureDeleted();
            }
            context.Database.EnsureCreated();            
        } 

    }
}