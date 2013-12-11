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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using umbraco.BusinessLogic;
using WebBlocks.BusinessLogic.Helpers;
using WebBlocks.BusinessLogic.Interface;
using WebBlocks.BusinessLogic.Model;

namespace WebBlocks.BusinessLogic.Events
{
    public class RegisterEmbeddedResources : ApplicationBase
    {
        public RegisterEmbeddedResources()
        {
            Register();
        }

        public void Register()
        {
            
            Type iDeclareRazor = typeof (IDeclareRazor);
            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.GetInterfaces().Contains(iDeclareRazor));

            List<EmbeddedDeclaration> declarations = new List<EmbeddedDeclaration>();
            foreach (var type in types)
            {
                IDeclareRazor instance = (IDeclareRazor)Activator.CreateInstance(type);
                var declaration = instance.GetEmbeddedResources();

                declaration.ToList().ForEach(d => declarations.Add(new EmbeddedDeclaration()
                    {
                        BlockAlias = d.Key,
                        Namespace = d.Value,
                        DLLName = type.Assembly.GetName().Name
                    }));
            }

            HttpContext.Current.Cache.Add(EmbeddedResources.EMBEDDED_RAZOR_CACHE_NAME, declarations, null, 
                DateTime.Now.AddYears(5), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }
    }
}
