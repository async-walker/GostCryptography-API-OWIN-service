﻿using GostCryptographyAPI.Types;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GostCryptographyAPI.Formatters
{
    public class BinaryMediaTypeFormatter : MediaTypeFormatter
    {

        private static Type _supportedType = typeof(byte[]);
        private bool _isAsync = false;

        public BinaryMediaTypeFormatter() : this(false)
        {
        }

        public BinaryMediaTypeFormatter(bool isAsync)
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue(ContentType.Binary));
            IsAsync = isAsync;
        }

        public bool IsAsync
        {
            get { return _isAsync; }
            set { _isAsync = value; }
        }

        public override bool CanReadType(Type type)
        {
            return type == _supportedType;
        }

        public override bool CanWriteType(Type type)
        {
            return type == _supportedType;
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream,
            HttpContent content, IFormatterLogger formatterLogger)
        {
            Task<object> readTask = GetReadTask(stream);
            if (_isAsync)
            {
                readTask.Start();
            }
            else
            {
                readTask.RunSynchronously();
            }
            return readTask;

        }

        private Task<object> GetReadTask(Stream stream)
        {
            return new Task<object>(() =>
            {
                var ms = new MemoryStream();
                stream.CopyTo(ms);
                return ms.ToArray();
            });
        }

        private Task GetWriteTask(Stream stream, byte[] data)
        {
            return new Task(() =>
            {
                var ms = new MemoryStream(data);
                ms.CopyTo(stream);
            });
        }


        public override Task WriteToStreamAsync(Type type, object value, Stream stream,
            HttpContent content, TransportContext transportContext)
        {

            if (value == null)
                value = new byte[0];
            Task writeTask = GetWriteTask(stream, (byte[])value);
            if (_isAsync)
            {
                writeTask.Start();
            }
            else
            {
                writeTask.RunSynchronously();
            }
            return writeTask;
        }
    }
}
