﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SQLiteMapaTeste.Service
{
    class ImageService
    {
        public static byte[] ConvertImageToByte(string path, Assembly assembly)
        {
            var stream = GetImageFromStream(path, assembly);
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            var buffer = memoryStream.ToArray();
            return buffer;
        }

        public static byte[] ConvertImageToByte(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0;
            var buffer = memoryStream.ToArray();
            return buffer;
        }

        public static Stream GetImageFromStream(string path, Assembly assembly)
        {
            Stream stream = assembly.GetManifestResourceStream(path);
            return stream;
        }
    }
}
