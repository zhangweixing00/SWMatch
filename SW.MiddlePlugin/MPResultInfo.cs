using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW.MiddlePlugin
{
    public class MPResultInfo
    {
        public ExecuteStatus Status { get; set; }
        public string Contents { get; set; }
    }
    public enum ExecuteStatus
    {
        截断,
        继续,
        忽略
    }
}
