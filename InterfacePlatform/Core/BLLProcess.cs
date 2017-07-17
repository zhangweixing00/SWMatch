using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace InterfacePlatform.Core
{
    public class BLLProcess
    {
        /// <summary>
        /// 识别用户传入文本
        /// </summary>
        /// <param name="userString"></param>
        /// <returns></returns>
        public static RecogniteInfo Recognite(string userString)
        {
            DAL.EFSource.HEEntities context = new DAL.EFSource.HEEntities();

            ProcessType ret = ProcessType.未知;
            Dictionary<string, DAL.EFSource.Product> finallyParams = new Dictionary<string, DAL.EFSource.Product>();
            if (!string.IsNullOrEmpty(userString) && !string.IsNullOrEmpty(userString.Trim()))
            {
                userString = userString.Replace("，", ",");
                char[] spiltChars = { ',', ' ' };//分隔符优先顺序
                string[] params_Items = { userString };
                foreach (var item in spiltChars)
                {
                    if (userString.Contains(item))
                    {
                        params_Items = userString.Split(item);
                        break;
                    }
                }
                Dictionary<string, DAL.EFSource.Product> tmpParams = new Dictionary<string, DAL.EFSource.Product>();

                foreach (var item in params_Items)
                {
                    ///去空去重关键字
                    if (!string.IsNullOrEmpty(item) && !tmpParams.Keys.Contains(item))
                    {
                        DAL.EFSource.Product pInfo = context.Product.FirstOrDefault(x => x.Name == item || ("、" + x.NickNames + "、").Contains("、" + item + "、"));
                        if (pInfo == null)
                        {
                            //模糊查找
                            pInfo = context.Product.FirstOrDefault(x => x.Name.StartsWith(item));

                            if (pInfo == null)
                            {
                                pInfo = context.Product.FirstOrDefault(x => x.Name.Contains(item));
                            }
                        }
                        tmpParams.Add(item, pInfo);
                    }
                }
                List<string> currentInfos = new List<string>();
                foreach (KeyValuePair<string, DAL.EFSource.Product> item in tmpParams)
                {
                    if (item.Value != null && currentInfos.Contains(item.Value.Id))
                    {
                        continue;
                    }
                    finallyParams.Add(item.Key, item.Value);
                }

                //-------------------------

                if (finallyParams.Count == 1)
                {
                    ret = ProcessType.单类;
                }
                else
                {
                    if (tmpParams.Count == 2)
                    {
                        ret = ProcessType.双类;
                    }
                    else
                    {
                        if (tmpParams.Count >= 3)
                        {
                            ret = ProcessType.混合;
                        }
                    }
                }

            }
            return new RecogniteInfo()
            {
                PType = ret,
                ProductList = finallyParams
            };
        }

        public static string Entry(string userString)
        {
            if (userString=="?"||userString=="？")
            {
                return "";
            }
            Log.Logger.logger.DebugFormat("输入文本：{0}", userString);

            if(Global.MPList.Count>0)
            {
                foreach (var item in Global.MPList)
                {
                    var ret = item.Execute(userString);
                    if (ret.Status== SW.MiddlePlugin.ExecuteStatus.截断)
                    {
                        Log.Logger.logger.DebugFormat("直接返回文本：{0}", ret.Contents);
                        return ret.Contents;
                    }
                    else if (ret.Status == SW.MiddlePlugin.ExecuteStatus.继续)
                    {
                        userString = ret.Contents;
                        Log.Logger.logger.DebugFormat("转换文本：{0}", userString);
                    }
                }
            }

            string backString = "";
            RecogniteInfo info = Recognite(userString);
            switch (info.PType)
            {
                case ProcessType.单类:
                    backString = SingleP(info.ProductList.First());
                    break;
                case ProcessType.双类:
                    backString = SingleT(info.ProductList);
                    break;
                case ProcessType.混合:
                    backString = SingleH(info.ProductList);
                    break;
                case ProcessType.未知:
                    break;
                default:
                    break;
            }
            Log.Logger.logger.DebugFormat("输出：{0}", backString);

            return backString;
        }

        private static string SingleH(Dictionary<string, DAL.EFSource.Product> dictionary)
        {
            DAL.EFSource.HEEntities context = new DAL.EFSource.HEEntities();

            StringBuilder content = new StringBuilder();

            List<string> noInfos = new List<string>();
            Dictionary<string, DAL.EFSource.Product> list = new Dictionary<string, DAL.EFSource.Product>();
            foreach (KeyValuePair<string, DAL.EFSource.Product> item in dictionary)
            {
                if (item.Value == null)
                {
                    noInfos.Add(item.Key);
                }
                else
                {
                    list.Add(item.Key, item.Value);
                }
            }
            if (list.Count <= 1)
            {
                //不符合比较规范
                content.Append("没有找到名称为");
                foreach (var item in noInfos)
                {
                    content.AppendFormat("{0}、", item);
                }
                content.Remove(content.Length - 1, 1);
                content.Append("相关的食材，换个名称试试吧^_^\r\n获取查询方法请输入问号");
                //content.Append(SingleP(list.First()));
            }
            else
            {
                if (list.Count == 2)
                {
                    content.Append(SingleT(list));
                }
                else
                {
                    //真正的混合模式
                    List<DAL.EFSource.RelationShip> rsList = new List<DAL.EFSource.RelationShip>();
                    List<DAL.EFSource.Product> pInfos = dictionary.Select(x => x.Value).ToList();
                    for (int index = 0; index < pInfos.Count; index++)
                    {
                        for (int suIndex = index + 1; suIndex < pInfos.Count; suIndex++)
                        {
                            DAL.EFSource.Product itemF = pInfos[index];
                            DAL.EFSource.Product itemS = pInfos[suIndex];
                            DAL.EFSource.RelationShip rs = context.RelationShip.FirstOrDefault(x => x.PA == itemF.Id && x.PB == itemS.Id
    || x.PB == itemF.Id && x.PA == itemS.Id);
                            if (rs != null)
                            {
                                rsList.Add(rs);
                                content.AppendFormat("{0}与{1}{2}宜搭配，{3}", itemF.Name, itemS.Name, rs.RType == 0 ? "不" : "", rs.Description);
                            }

                        }
                    }
                    if (rsList.Count(x => x.RType == 0) == 0)
                    {
                        if (content.ToString()!="")
                        {
                            content.Append("\r\n");
                        }
                        content.Append("所有食材均不相互冲突，可一块食用");
                    }

                }
            }
            return content.ToString();

        }

        private static string SingleT(Dictionary<string, DAL.EFSource.Product> dictionary)
        {
            DAL.EFSource.HEEntities context = new DAL.EFSource.HEEntities();

            StringBuilder content = new StringBuilder();
            string fKey = dictionary.Keys.ToList()[0];
            DAL.EFSource.Product itemF = dictionary[fKey];
            string sKey = dictionary.Keys.ToList()[1];
            DAL.EFSource.Product itemS = dictionary[sKey];
            if (itemF == null && itemS == null)
            {
                content.AppendFormat("没有找到与{0}、{1}相关的食材，换个名称试试^_^，获取查询方法请输入问号", fKey, sKey);
            }
            else
            {
                if (itemF != null && itemS != null)
                {
                    DAL.EFSource.RelationShip rs = context.RelationShip.FirstOrDefault(x => x.PA == itemF.Id && x.PB == itemS.Id
                        || x.PB == itemF.Id && x.PA == itemS.Id);
                    if (rs != null)
                    {
                        content.AppendFormat("{0}与{1}{2}宜搭配，{3}", itemF.Name, itemS.Name, rs.RType == 0 ? "不" : "", rs.Description);
                    }
                    else
                    {
                        content.AppendFormat("{0}与{1}不相克，可以一块食用", itemF.Name, itemS.Name);
                    }
                }
                else
                {
                    if (itemS != null)
                    {
                        content.Append(SingleP(new KeyValuePair<string, DAL.EFSource.Product>(sKey, itemS)));
                    }
                    else
                    {
                        content.Append(SingleP(new KeyValuePair<string, DAL.EFSource.Product>(fKey, itemF)));
                    }
                    content.AppendFormat("\r\n没有找到与{0}相关的食材,换个名称试试^_^", itemS != null?fKey:sKey);
                }
            }

            return content.ToString();
        }

        private static string SingleP(KeyValuePair<string, DAL.EFSource.Product> product)
        {

            DAL.EFSource.HEEntities context = new DAL.EFSource.HEEntities();

            if (product.Value == null)
            {
                //
                return string.Format("没有找到与{0}相关的食材，换个名称试试^_^\r\n获取查询方法请输入问号", product.Key);
            }
            else
            {
                StringBuilder content = new StringBuilder();
                content.AppendFormat("{0}", product.Value.Name);
                if (!string.IsNullOrEmpty(product.Value.NickNames))
                {
                    // content.AppendFormat(",又叫{0}",product.Value.NickNames); 
                }
                if (!string.IsNullOrEmpty(product.Value.Description))
                {
                    content.AppendFormat("，{0}", product.Value.Description);
                }
                if (!string.IsNullOrEmpty(product.Value.GoodFor))
                {
                    content.AppendFormat("\r\n适宜：{0}", product.Value.GoodFor);
                }
                if (!string.IsNullOrEmpty(product.Value.NoGoodFor))
                {
                    content.AppendFormat("\r\n禁忌：{0}", product.Value.NoGoodFor);
                }

                List<DAL.EFSource.RelationShip> xlist = context.RelationShip.Where(x => x.PA == product.Value.Id
                 && x.RType == 0).ToList();

                if (xlist.Count > 0)
                {
                    content.Append("\r\n相克：");
                    foreach (var item in xlist)
                    {
                        DAL.EFSource.Product xkProduct = context.Product.FirstOrDefault(x => x.Id == item.PB);
                        if (xkProduct!=null)
                        {
                            content.AppendFormat("{0}、", xkProduct.Name);
                        }
                        else
                        {
                             content.AppendFormat("{0}、",item.PB);
                        }
                    }
                    content.Remove(content.Length - 1, 1);
                    content.Append("。");
                }

                List<DAL.EFSource.RelationShip> ylist = context.RelationShip.Where(x => x.PA == product.Value.Id
                 && x.RType == 1).ToList();

                if (ylist.Count > 0)
                {
                    content.Append("\r\n相宜：");
                    foreach (var item in ylist)
                    {
                        DAL.EFSource.Product xyProduct = context.Product.FirstOrDefault(x => x.Id == item.PB);
                        if (xyProduct!=null)
                        {
                            content.AppendFormat("{0}、", xyProduct.Name);
                        }
                        else
                        {
                            content.AppendFormat("{0}、", item.PB);
                        }
                    }
                    content.Remove(content.Length - 1, 1);
                    content.Append("。");
                }
                return content.ToString();
            }
        }

    }
}