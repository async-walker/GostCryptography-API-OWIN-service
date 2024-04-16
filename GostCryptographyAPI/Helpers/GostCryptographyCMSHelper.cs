﻿using GostCryptography.Pkcs;
using Serilog;
using System;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;

namespace GostCryptographyAPI.Helpers
{
    public static class GostCryptographyCMSHelper
    {
        public static byte[] SignMessage(X509Certificate2 certificate, byte[] message)
        {
            try
            {
                var signedCms = new GostSignedCms(new ContentInfo(message));

                var signer = new CmsSigner(certificate)
                {
                    IncludeOption = X509IncludeOption.EndCertOnly
                };

                signedCms.ComputeSignature(signer);

                return signedCms.Encode();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Исключение при подписи сообщения");

                throw;
            }
        }
    }
}
