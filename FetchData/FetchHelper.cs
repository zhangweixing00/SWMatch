using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace FetchData
{
    public class FetchHelper
    {
        string mainAddr = "";


        public static void FetchCatalogue()
        {
            DAL.EFSource.HEEntities context = new DAL.EFSource.HEEntities();

            string address = "http://www.xiangha.com/shicai/";
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            string htmlCode = wb.DownloadString(address);
            string content = Regex.Match(htmlCode, "<div class=\"rec_classify_con\">(.|\n)*?<div class=\"main_act w960\">").Value;

            MatchCollection mc = Regex.Matches(content, "<div class=\"rec_classify_cell clearfix\">(.|\n)*?</div>");
            foreach (Match item in mc)
            {
                //创建一级分类
                string regString = "<h3 id=\"(?<id>.*?)\">(.|\n)*?href=\"(?<link>.*?)\">(?<name>.*?)</a>";
                Match fMatch = Regex.Match(item.Value, regString);
                context.AddToCatalogue(new DAL.EFSource.Catalogue()
                {
                    Id = "P_" + fMatch.Groups["id"].Value,
                    Name = fMatch.Groups["name"].Value,
                    Pid = "",
                    Link = fMatch.Groups["link"].Value,
                    CreateTime = DateTime.Now,
                    LastUpdateTime = DateTime.Now,
                    IsFinished = 0,
                    CurrentPageIndex = 1,
                    CurrentPageStatus = 0
                });
                MatchCollection subMc = Regex.Matches(item.Value, "<ul class=\"clearfix\">(.|\n)*?</ul>");
                foreach (Match subItem in subMc)
                {
                    //二级分类
                    regString = "<li id=\"(?<id>.*?)\">(.|\n)*?strong(.|\n)*?href=\"(?<link>.*?)\">(?<name>.*?)</a>";
                    Match sMatch = Regex.Match(subItem.Value, regString);
                    context.Catalogue.AddObject(new DAL.EFSource.Catalogue()
                      {
                          Id = sMatch.Groups["id"].Value,
                          Name = sMatch.Groups["name"].Value,
                          Pid = "P_" + fMatch.Groups["id"].Value,
                          Link = sMatch.Groups["link"].Value,
                          CreateTime = DateTime.Now,
                          LastUpdateTime = DateTime.Now,
                          IsFinished = 0,
                          CurrentPageIndex = 1,
                          CurrentPageStatus = 0
                      });
                    context.SaveChanges();
                }


            }

            // context.SaveChanges();
        }


        public static void FetchProduct()
        {
            DAL.EFSource.HEEntities context = new DAL.EFSource.HEEntities();
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            List<DAL.EFSource.Catalogue> catalogs = context.Catalogue.Where(x => !string.IsNullOrEmpty(x.Pid)).OrderBy(x => x.Id).ToList();
            foreach (DAL.EFSource.Catalogue item in catalogs)
            {
                Console.WriteLine(item.Name);
                if (item.IsFinished == 1)
                {
                    continue;
                }
                for (int pIndex = item.CurrentPageIndex.Value; pIndex < 30; pIndex++)
                {
                    Console.WriteLine("---------" + pIndex);
                    string address = item.Link + "-" + pIndex.ToString();
                    string htmlCode = "";
                    try
                    {
                        htmlCode = wb.DownloadString(address);
                    }
                    catch (WebException ex)
                    {
                        if (ex.Status == WebExceptionStatus.ProtocolError)
                        {
                            item.IsFinished = 1;
                            Console.WriteLine("--------- stop");
                            break;
                        }

                    }
                    if (htmlCode.Contains("<title>404"))
                    {
                        item.IsFinished = 1;
                        Console.WriteLine("--------- stop");
                        break;
                    }
                    //<div class="cla_ing_i_list"><div class="pagebar">
                    string content = Regex.Match(htmlCode, "<div class=\"cla_ing_i_list\">(.|\n)*?<div class=\"pagebar\">").Value;

                    MatchCollection mc = Regex.Matches(content, "<li>(.|\n)*?</li>");
                    foreach (Match subItem in mc)
                    {
                        //创建
                        string regString = "<p class=\"name\">(.|\n)*?href=\"(?<link>.*?)\"(.|\n)*?>(?<name>.*?)</a>";
                        Match fMatch = Regex.Match(subItem.Value, regString);

                        regString = "<p class=\"info\">(?<link>(.|\n)*?)</p>";

                        MatchCollection infoMatchs = Regex.Matches(subItem.Value, regString);
                        if (infoMatchs.Count > 0)
                        {
                            //infoMatchs[0].Value
                        }
                        string xkId = "-1";
                        if (infoMatchs.Count > 1)
                        {
                            regString = "<a (.|\n)*?/xiangke/(?<id>.*?)\"(.|\n)*?>";
                            Match xk = Regex.Match(infoMatchs[1].Value, regString);
                            if (xk.Success && !string.IsNullOrEmpty(xk.Groups["id"].Value))
                            {
                                xkId = xk.Groups["id"].Value;
                            }

                        }

                        context.Product.AddObject(new DAL.EFSource.Product()
                        {
                            Id = fMatch.Groups["link"].Value.Replace("http://www.xiangha.com/caipu/s-", ""),
                            Name = fMatch.Groups["name"].Value,
                            CataId = item.Id,
                            Link = fMatch.Groups["link"].Value,
                            CreateTime = DateTime.Now,
                            Description = infoMatchs[0].Groups["link"].Value,
                            IsSetMap = 0,
                            XiakeId = int.Parse(xkId)
                        });
                        Console.WriteLine("---------------------- - " + fMatch.Groups["name"].Value);
                    }

                    item.CurrentPageIndex = pIndex + 1;
                }
                context.SaveChanges();
            }
        }

        public static void FetchProductSimple()
        {
            DAL.EFSource.HEEntities context = new DAL.EFSource.HEEntities();
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            List<DAL.EFSource.Product> infos = context.Product.ToList();
            foreach (DAL.EFSource.Product item in infos)
            {
                string htmlCode = wb.DownloadString(item.Link);

                string content = Regex.Match(htmlCode, "<div class=\"ing_name clearfix\">(.|\n)*?<div class=\"list_tab clearfix\">").Value;

                string regString = "<img(.|\n)*?src=\"(?<link>.*?)\"";
                Match foMatch = Regex.Match(content, regString);
                string link = foMatch.Groups["link"].Value;

                regString = "<p>别名：(?<nick>.*?)</p>";
                Match fMatch = Regex.Match(content, regString);
                string nick = fMatch.Groups["nick"].Value;

                regString = "<p class=\"avoid\">(.|\n)*?：(?<avoid>.*?)</p>";
                Match sMatch = Regex.Match(content, regString);
                string avoid = sMatch.Groups["avoid"].Value;

                regString = "<p class=\"suit\">(.|\n)*?：(?<suit>.*?)</p>";
                Match tMatch = Regex.Match(content, regString);
                string suit = tMatch.Groups["suit"].Value;

                item.img = link;
                item.NickNames = nick;
                item.NoGoodFor = avoid;
                item.GoodFor = suit;
            }
            context.SaveChanges();
        }

        public static void FetchProductMap()
        {
            DAL.EFSource.HEEntities context = new DAL.EFSource.HEEntities();
            WebClient wb = new WebClient();
            wb.Encoding = Encoding.UTF8;
            List<DAL.EFSource.Product> infos = context.Product.ToList();
            foreach (DAL.EFSource.Product item in infos)
            {
                if (item.XiakeId.Value == -1)
                {
                    continue;
                }
                string address = "http://www.xiangha.com/xiangke/" + item.XiakeId.Value.ToString();
                string htmlCode = wb.DownloadString(address);

                string content = Regex.Match(htmlCode, "<div class=\"ing_tips tab_con\">(.|\n)*?</table>").Value;


                int xiangkeValue = 0;
                int yidaValue = 0;
                MatchCollection ths = Regex.Matches(content, "<th(.|\n)*?</th>");
                foreach (Match thItem in ths)
                {
                    if (thItem.Value.Contains("xiangke"))
                    {
                        Match xiangke = Regex.Match(thItem.Value, "<th(.|\n)*?rowspan=\"(?<value>.*?)\"(.|\n)*?id=\"xiangke\">");
                        if (xiangke.Success)
                        {
                            xiangkeValue = int.Parse(xiangke.Groups["value"].Value);
                        }
                    }
                    if (thItem.Value.Contains("yida"))
                    {
                        Match yida = Regex.Match(thItem.Value, "<th(.|\n)*?rowspan=\"(?<value>.*?)\"(.|\n)*?id=\"yida\">");
                        if (yida.Success)
                        {
                            yidaValue = int.Parse(yida.Groups["value"].Value);
                        }

                    }
                }

                Console.WriteLine(item.Name);

                MatchCollection list = Regex.Matches(content, "<tr>(.|\n)*?</tr>");
                if (list.Count != yidaValue + xiangkeValue)
                {

                }
                int index = 0;
                Console.WriteLine("---克：");
                for (; index < xiangkeValue; index++)
                {
                    ProcRS(context, item, list[index], 0);
                }
                Console.WriteLine("---宜：");
                for (; index < list.Count; index++)
                {
                    ProcRS(context, item, list[index], 1);
                }
            }
            context.SaveChanges();
        }

        private static void ProcRS(DAL.EFSource.HEEntities context, DAL.EFSource.Product item, Match match, int type)
        {
            if (match.Success)
            {
                MatchCollection mc = Regex.Matches(match.Value, "<td>(.|\n)*?</td>");
                foreach (Match subItem in mc)
                {
                    //<em>+</em>
                    string regString = "</em>(?<lon>.*?)</a>：(.|\n)*?<a(.|\n)*?>(?<des>.*?)</a>";
                    Match fMatch = Regex.Match(subItem.Value, regString);
                    if (fMatch.Success)
                    {
                        string name = "";
                        if (!fMatch.Groups["lon"].Value.Contains("<a "))
                        {
                            name = fMatch.Groups["lon"].Value;
                        }
                        else
                        {
                            regString = "<a(.|\n)*?>(?<name>.*)";
                            Match sMatch = Regex.Match(fMatch.Groups["lon"].Value, regString);
                            name = sMatch.Groups["name"].Value;
                        }

                        DAL.EFSource.Product fp = context.Product.FirstOrDefault(x => x.Name == name);
                        if (fp == null)
                        {
                            //fp = context.Product.First(x => x.NickNames.Split('、').ToList().Contains(name));
                            fp = context.Product.FirstOrDefault(x =>("、"+x.NickNames+"、").Contains("、"+name+"、"));

                        }
                        if (fp == null)
                        {
                            Console.Write(name+"（创建）、");
                            fp = new DAL.EFSource.Product()
                            {
                                Id = name,
                                Name = name,
                                NickNames = name,
                                CataId = "",
                                XiakeId = -1,
                                CreateTime = DateTime.Now,
                                Description = "AutoCreate"
                            };
                        }
                        else
                        {
                            Console.Write(name + "（找到）、");
                        }
                        int count = context.RelationShip.Count(x => x.RType == type && (x.PA == item.Id && x.PB == fp.Id || x.PB == item.Id && x.PA == fp.Id));
                        if (count == 0)
                        {
                            context.RelationShip.AddObject(new DAL.EFSource.RelationShip()
                            {
                                PA = item.Id,
                                PB = fp.Id,
                                CreateTime = DateTime.Now,
                                RType = type,
                                Description = fMatch.Groups["des"].Value
                            });
                            //Console.WriteLine("------------------done----------------");
                        }
                    }
                }
            }
            else
            {

            }
        }
    }
}
