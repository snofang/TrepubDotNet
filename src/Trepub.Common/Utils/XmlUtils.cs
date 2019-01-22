using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Trepub.Common.Utils
{
    public class XmlUtils
    {
        public static decimal GetMpdDuration(XmlDocument mpd)
        {
            var periodElement = mpd.GetElementsByTagName("Period")[0];
            var durationStr = periodElement.Attributes["duration"].Value;
            durationStr = durationStr.Substring(2).Replace("S", "");
            return decimal.Parse(durationStr);
        }

        public static long GetMpdAudioSize(XmlDocument mpd)
        {
            long result = 0;
            var adaptationSets = mpd.GetElementsByTagName("AdaptationSet");
            foreach (XmlElement adaptationSet in adaptationSets)
            {
                if (adaptationSet.GetAttribute("mimeType").Equals("audio/webm"))
                {
                    var indexRange = ((XmlElement)adaptationSet.GetElementsByTagName("SegmentBase")[0]).GetAttribute("indexRange");
                    result = long.Parse(indexRange.Split("-")[1]) + 1;
                    break;
                }
            }
            return result;
        }
    }
}
