/****
    Mentor Web Blocks for Umbraco
    Copyright (C) 2013 Mentor Digital 
    (Mentor Communications Consultancy Ltd, 4 West End, Somerset Street, Bristol, BS2 8NE)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
****/

using System.IO;
using System.Text.RegularExpressions;
using umbraco.cms.businesslogic.template;

namespace WebBlocks.BusinessLogic.Extensions
{
    public static class TemplateExtensions
    {
        /// <summary>
        /// Retrieves layout content from a template for use within the builder.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="placeholder"></param>
        /// <returns></returns>
        public static string GetLayoutContent(this Template template, string placeholder)
        {
            // Read in the contents of the raw template file
            string templateText = File.ReadAllText(template.MasterPageFile);
            // Instantiate a variable for holding the parsed control text.
            string controlText = "";

            // Setup a layout pattern for matching with layout content.
            Regex layoutContentPattern = new Regex("<asp:content.*?contentplaceholderid=\"" + placeholder + "\".*?>(.*?)</asp:content>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match layoutContentMatch = layoutContentPattern.Match(templateText);

            if (layoutContentMatch.Success && layoutContentMatch.Groups.Count > 0)
            {
                controlText = layoutContentMatch.Groups[1].Value;
            }

            

            // Pull any register tags back in. They're fine to use.
            Regex masterTagPattern = new Regex("<%@ Register.*?>", RegexOptions.IgnoreCase);
            MatchCollection masterTagMatches = masterTagPattern.Matches(templateText);

            foreach (Match m in masterTagMatches)
            {
                if (m.Success)
                {
                    controlText = m.Groups[0] + controlText;
                }
            }

            return controlText;
        }
    }
}
