using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterfacePlatform.Core
{
    public class RecogniteInfo
    {
        public ProcessType PType { get; set; }
        public Dictionary<string,DAL.EFSource.Product> ProductList { get; set; }
    }
}