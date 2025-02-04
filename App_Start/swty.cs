
using System.Collections.Generic;
using System.Xml;
using Microsoft.VisualBasic;
using HtmlAgilityPack;
using System.Text;
using System;

namespace SwtyChina
{
    public class SwtyItem
    {
        public SwtyItem()
        {
            date = title = content = countid = "";
        }
        public SwtyItem(string date_, string title_, string content_, string id_)
        {
            date = date_;
            title = title_;
            content = content_;
            countid = id_;
        }
        public string date { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string countid { get; set; }
    }
    public class Swty
    {
        static string _XmlUrl = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/swty.xml");
        static int _XmlEndYear = 2024;
        static int _XmlEndMonth = 12;
        static string _PortalUrl = "https://swtychina.com/gb/portal/";
        static public List<SwtyItem> GetNewItems()
        {
            string Url = _PortalUrl + "new.aspx?";
            List<SwtyItem> newItems = GetItems(Url);
            string[] dates = newItems[newItems.Count - 1].date.Split('-');
            List<SwtyItem> nextItems = GetMonthItems(dates[0], dates[1], true);
            foreach (SwtyItem nextItem in nextItems)
            {
                if (ItemExists(newItems, nextItem))
                    continue;

                newItems.Add(nextItem);
            }
            return newItems;
        }
        static bool ItemExists(List<SwtyItem> items, SwtyItem item)
        {
            foreach (SwtyItem newItem in items)
            {
                if (item.date == newItem.date)
                    return true;
            }
            return false;
        }
        static bool UseXml(string year, string month)
        {
            return ( int.Parse(year) < _XmlEndYear
                || (int.Parse(year) == _XmlEndYear && int.Parse(month) <= _XmlEndMonth) );
        }
        static public List<SwtyItem> GetMonthItems(string year, string month, bool bReverse)
        {
            List<SwtyItem> items= new List<SwtyItem>();
            if ("**" == month)
            {   // whole year's items
                for (int mon = 1; mon < 13; mon++)
                {
                    string Url = _PortalUrl + year + mon.ToString("D2") + ".aspx";
                    List<SwtyItem> mItems = UseXml(year, mon.ToString()) ? GetMonthContentItemsXml(year, mon.ToString("D2")) : GetItems(Url);
                    foreach (SwtyItem item in mItems)
                        items.Add(item);
                }
            }
            else
            {
                string Url = _PortalUrl + year + month + ".aspx";
                items = UseXml(year, month) ? GetMonthContentItemsXml(year, month) : GetItems(Url);
                if (bReverse)
                    items.Reverse();
            }
            return items;
        }
        static List<SwtyItem> GetItems(string url)
        {
            List<SwtyItem> items = new List<SwtyItem>();
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);
            HtmlNode table = doc.GetElementbyId("ctl00_ContentPlaceHolder1_GridView1");
            if (null == table)
                return items;

            int th_index = 0;
            foreach (HtmlNode th in table.ChildNodes)
            {
                if (th_index++ <= 1)
                    continue;

                int index = 0;
                string date = "", title = "", content = "", id = "";
                foreach (HtmlNode tr in th.ChildNodes)
                {
                    if (index == 1)
                    { 
                        date = tr.InnerText;
                        var strPair = GetContentAndID(url, tr);
                        content = strPair.Item1;
                        id = strPair.Item2;
                    }
                    if (index == 5)
                    {
                        title = tr.InnerText;
                        break;
                    }
                    ++index;
                }
                if (date != "" && title != "")
                    items.Add(new SwtyItem(date, title, content, id));
            }
            return items;
        }
        static public List<SwtyItem> Search(string str, bool bContent = false)
        {
            string[] arr = str.Split(' ');
            bool includeContent = arr.Length > 1 && str.ToLower().Contains("content");
            bool xmlOnly = arr.Length > 1 && str.ToLower().Contains("xmlonly");
            // both simplified and traditional Chinese
            string trd = Strings.StrConv(arr[0], VbStrConv.TraditionalChinese);
            string smp = Strings.StrConv(arr[0], VbStrConv.SimplifiedChinese);
            List<SwtyItem> items = new List<SwtyItem>();
            // search in XML
            XmlDocument doc = new XmlDocument();
            doc.Load(_XmlUrl);
            foreach (XmlNode item in doc.DocumentElement.ChildNodes)
            {
                if (item.ChildNodes[1].InnerText.Contains(trd) || item.ChildNodes[1].InnerText.Contains(smp) ||                         // title
                    (includeContent && (item.ChildNodes[2].InnerText.Contains(trd) || item.ChildNodes[2].InnerText.Contains(smp))))    // content
                    items.Add(new SwtyItem(item.ChildNodes[0].InnerText, item.ChildNodes[1].InnerText, bContent ? item.ChildNodes[2].InnerText : "", item.ChildNodes[3].InnerText));
            }
            if (xmlOnly)
                return items;
            // search the latest from swtychina.com
            DateTime now = DateTime.Now;
            for (int year = _XmlEndYear + 1; year <= now.Year; ++year)
            {
                var yearItems = GetMonthItems(year.ToString(), "**", false);
                foreach (SwtyItem webItem in yearItems)
                {
                    if (webItem.title.Contains(trd) || webItem.title.Contains(smp) ||                           // title
                        (includeContent && (webItem.content.Contains(trd) || webItem.content.Contains(smp))))  // content
                    {
                        if (ItemExists(items, webItem))
                            continue;

                        items.Add(webItem);
                    }
                }
            }
            return items;
        }
        static List<SwtyItem> GetMonthContentItemsXml(string year, string month, bool bContent = false)
        {
            List<SwtyItem> items = new List<SwtyItem>();
            XmlDocument doc = new XmlDocument();
            doc.Load(_XmlUrl);
            foreach (XmlNode item in doc.DocumentElement.ChildNodes)
            {
                if (item.ChildNodes[0].InnerText.Substring(0, 4) == year && item.ChildNodes[0].InnerText.Substring(5, 2) == month)
                    items.Add(new SwtyItem(item.ChildNodes[0].InnerText, item.ChildNodes[1].InnerText, bContent ? item.ChildNodes[2].InnerText : "", item.ChildNodes[3].InnerText));
            }
            return items;
        }
        static string GetCountID(string aspx)
        {
            var id = new StringBuilder();
            foreach (var ch in aspx)
            {
                if (char.IsDigit(ch))
                    id.Append(ch);
            }
            return id.ToString();
        }
        static Tuple<string, string> GetContentAndID(string url, HtmlNode tr, bool getContent = false)
        {
            string content = "", id = "";
            string[] arr = tr.InnerHtml.Split(' ');
            foreach (string attr in arr)
            {
                if (attr.Contains("href="))
                {
                    string[] aspx = attr.Split('"');
                    if(getContent)
                        content = GetContent(url, aspx[1]);
                    id = GetCountID(aspx[1]);
                    break;
                }
            }
            return Tuple.Create(content, id);
        }
        static string GetContent(string url, string aspx)
        {
            url = url.Substring(0, url.LastIndexOf("/") + 1) + aspx;
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);
            HtmlNode contentNode = doc.GetElementbyId("ctl00_ContentPlaceHolder1_FormView1_contentLabel");
            return contentNode != null ? contentNode.InnerText : "";
        }
    }
}
