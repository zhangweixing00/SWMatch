using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW.MiddlePlugin
{
    public interface IMiddlePlugin
    {
        MPResultInfo Execute(string content);
    }
}
