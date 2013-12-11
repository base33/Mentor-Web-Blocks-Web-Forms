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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using WebBlocks.BusinessLogic.Model;

namespace WebBlocks.BusinessLogic.Helpers
{
    public static class EmbeddedResources
    {
        public const string EMBEDDED_RAZOR_CACHE_NAME = "WebBlocksEmbeddedRazor";

        public static bool IsEmbeddedBlock(string alias)
        {
            var embeddedBlocks = GetEmbeddedResources();

            return embeddedBlocks.FirstOrDefault(d => d.BlockAlias == alias) != null;
        }

        public static List<EmbeddedDeclaration> GetEmbeddedResources()
        {
            return (List<EmbeddedDeclaration>)HttpContext.Current.Cache[EMBEDDED_RAZOR_CACHE_NAME] ?? new List<EmbeddedDeclaration>();
        }

        public static string GetEmbeddedScript(string alias)
        {
            var embeddedBlocks = GetEmbeddedResources();

            EmbeddedDeclaration declaration = embeddedBlocks.FirstOrDefault(d => d.BlockAlias == alias);

            Assembly typeAssembly = Assembly.Load(declaration.DLLName);

            string script = "";

            using(StreamReader reader  = new StreamReader(typeAssembly.GetManifestResourceStream(declaration.Namespace)))
            {
                script = reader.ReadToEnd();
            }

            return script;
        }
    }
}
