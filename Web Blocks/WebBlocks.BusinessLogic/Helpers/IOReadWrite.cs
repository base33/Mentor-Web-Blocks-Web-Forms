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
using System.Web;
using WebBlocks.BusinessLogic.Helpers.Interfaces;

namespace WebBlocks.BusinessLogic.Helpers
{
    public static class IOReadWrite
    {
        /// <summary>
        /// Reads a file and returns the deserialised object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T Read<T>(string filePath, ISerialiser serialiser = null)
        {
            if (serialiser == null)
                serialiser = new JSONSerialiser();

            if (filePath.StartsWith("~"))
                filePath = HttpContext.Current.Server.MapPath(filePath);

            //Create a default object to pass back
            T returnObject = default(T);
            string serialisedData = string.Empty;

            //get the serialised data
            if (File.Exists(filePath))
            {
                serialisedData = File.ReadAllText(filePath);
            }

            if (!string.IsNullOrEmpty(serialisedData))
            {
                returnObject = serialiser.Deserialise<T>(serialisedData);
            }

            return returnObject;
        }

        /// <summary>
        /// Reads a file and returns the string contents
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string Read(string filePath)
        {
            if (filePath.StartsWith("~"))
                filePath = HttpContext.Current.Server.MapPath(filePath);

            //return file data
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return string.Empty;
        }

        /// <summary>
        /// Serialises and writes the object to a specified file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="o"></param>
        public static void Write<T>(string filePath, T o, ISerialiser serialiser = null)
        {
            if (serialiser == null)
                serialiser = new JSONSerialiser();

            if (filePath.StartsWith("~"))
                filePath = HttpContext.Current.Server.MapPath(filePath);

            string serialisedData = serialiser.Serialise<T>(o);

            File.WriteAllText(filePath, serialisedData);
        }

        /// <summary>
        /// writes the data to a specified file
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="o"></param>
        public static void Write(string filePath, string data)
        {
            if (filePath.StartsWith("~"))
                filePath = HttpContext.Current.Server.MapPath(filePath);

            File.WriteAllText(filePath, data);
        }

        /// <summary>
        /// Scans a directory and returns a string array of file names
        /// </summary>
        /// <param name="filePath">string array of file names</param>
        /// <returns></returns>
        public static string[] ScanFiles(string filePath)
        {
            if (filePath.StartsWith("~"))
                filePath = HttpContext.Current.Server.MapPath(filePath);

            return Directory.GetFiles(filePath);
        }
    }
}
