using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterfacePlatform.Core
{
    public enum ProcessType
    {
        //单个关键字，如果不存在给出用户相近，否则显示相关信息
        单类,
        //两查询
        双类,
        //对各种实体进行两两组合，查询是否合适，最终出xy,xk表
        混合,
        未知
    }
}