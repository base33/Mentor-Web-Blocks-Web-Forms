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

namespace WebBlocks.BusinessLogic.Helpers
{
    public class ValueConverter
    {
        /// <summary>
        /// Current supported types and methods to convert from string
        /// </summary>
        public static Dictionary<Type, Func<string, dynamic>> TypeDictionary = new Dictionary<Type, Func<string, object>>()
            {
                { typeof(int), (value) => int.Parse(value)},
                { typeof(float), (value) => float.Parse(value)},
                { typeof(decimal), (value) => decimal.Parse(value)},
                { typeof(double), (value) => double.Parse(value)},
                { typeof(string), (value) => value},
                { typeof(bool), (value) => bool.Parse(value)},
                { typeof(DateTime), (value) => DateTime.Parse(value) },
                { typeof(char), (value) => char.Parse(value) }
            };

        /// <summary>
        /// Converts string to requested type
        /// </summary>
        /// <typeparam name="T">Type to return</typeparam>
        /// <param name="property">string value to return</param>
        /// <returns>returns new converted value</returns>
        public static T ConvertType<T>(string property)
        {
            //convert the type from the given string
            return TypeDictionary[typeof (T)](property);
        }
    }
}
