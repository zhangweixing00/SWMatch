using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SW.MiddlePlugin;

namespace SWMP.FilterSymbol
{
    public class FilterSymbol : SW.MiddlePlugin.IMiddlePlugin
    {
        public MPResultInfo Execute(string content)
        {
            char[] symbolList = new char[] { ',', '.', '，', '。' };

            content = content.Trim(symbolList);

            return new MPResultInfo()
            {
                Contents = content,
                Status = ExecuteStatus.继续
            };
        }
    }
}
