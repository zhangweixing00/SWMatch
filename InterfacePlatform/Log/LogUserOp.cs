using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InterfacePlatform.Log
{
    public class LogUserOp
    {
        public static bool Log(string uid, string opdes, int type)
        {
            try
            {
                DAL.EFSource.HEEntities context = new DAL.EFSource.HEEntities();
                context.AddToLog_Operation(new DAL.EFSource.Log_Operation()
                {
                    UserName = uid,
                    OpDes = opdes,
                    OpTime = DateTime.Now,
                    OpType = type
                });
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Logger.logger.DebugFormat("Log.error:{0}\r\n{1}", ex.Message, ex.StackTrace);
                return false;
            }
        }
    }
}