using System.Collections.Generic;
using HtmlAgilityPack;

namespace SwtyChina
{
    public class SwtyMp3
    {
        static public List<string> GetDirContents(string path)
        {
            string url = "http://mp3.swtychina.com/" + path;
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);
            // get inner text of all links
            List<string> contents_ = new List<string>();
            GetNodesByTagName(doc.DocumentNode, "a", ref contents_);
            // remove the first 5
            if (contents_.Count > 5)
            {
                for (int i = 0; i < 5; i++)
                    contents_.RemoveAt(0);
            }
            return contents_;
        }
        static void GetNodesByTagName(HtmlNode node, string tag, ref List<string> contents)
        {
            if (node.Name == tag)
                contents.Add(node.InnerText);
            if (node.HasChildNodes)
            {
                foreach (HtmlNode ele in node.ChildNodes)
                    GetNodesByTagName(ele, tag, ref contents);
            }
        }
    }
}