using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WebBlocks.Model;

namespace WebBlocks.BusinessLogic.Helpers
{
    /// <summary>
    /// Helper class which contains any methods required to work on the html of wysiwygs
    /// </summary>
    public class HtmlHelper
    {
        /// <summary>
        /// Adds a querystring to all image tags with width and height so that MImageResizer resizes them on request
        /// </summary>
        /// <param name="json">Layout builder json</param>
        /// <returns>Layout builder json</returns>
        public static string ResizeImages(string json)
        {
            List<Container> containers = json.FromJson<List<Container>>();
            List<Block> wysiwygBlocks = (from c in containers
                                         from b in c.Blocks
                                         where b.Content.Length > 0
                                         select b).ToList();
            

            foreach (Block block in wysiwygBlocks)
            {
                int offset = 0;
                //match 1 = src, match 2 = width, match 3 = height - explaination eof
                string imgRegex =
                    @"(?i)<img\s(?=[^>]*src=""([^""]*)"")(?=[^>]*width=""([^""]*)"")(?=[^>]*height=""([^""]*)"")[^>]*>";

                MatchCollection mc = Regex.Matches(block.Content, imgRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);

                foreach (Match match in mc)
                {
                    string newUrl = string.Format("{0}?width={1}&height={2}&crop=true",
                        match.Groups[1].Value.Split('?').First(), match.Groups[2].Value, match.Groups[3].Value);
                    
                    string newSrc = block.Content.Substring(0, match.Groups[1].Index + offset);
                    newSrc += newUrl;
                    int urlEndIndex = (match.Groups[1].Index + offset) + match.Groups[1].Length;
                    newSrc += block.Content.Substring(urlEndIndex, block.Content.Length - urlEndIndex);
                    block.Content = newSrc;
                    offset += newUrl.Length - match.Groups[1].Value.Length;
                }
            }

            return containers.ToJson();
        }
    }
}


//(?i)            // start case insensiteve matching
//<img            // match '<img'
//\s              // match a whitespace
//(?=             // start look ahead
//  [^>]*         // match zero or more characters of any type except '>'
//  alt=          // match 'alt='
//  "([^"]*)"     // match text between double quotes and group it (group 1)
//)               // stop look ahead
//(?=             // start look ahead
//  [^>]*         // match zero or more characters of any type except '>
//  title=        // match 'title='
//  "([^"]+)"     // match text between double quotes and group it (group 2)
//)               // stop look ahead
//[^>]*           // match zero or more characters of any type except '>'
//>               // match '>' 