using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Security;

namespace My.Framework.Foundation.Encrypt
{
    /// <summary>
    /// 非对称加密工具类
    /// </summary>
    public class RsaUtils
    {
        /**
         * RSA最大加密明文大小
         */
        private static int MAX_ENCRYPT_BLOCK = 117;

        /**
         * RSA最大解密密文大小
         */
        private static int MAX_DECRYPT_BLOCK = 128;

        /// <summary>
        /// 公钥解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static byte[] DecryptByPublicKey(byte[] data, string publicKey)
        {
            // 对密钥解密
            byte[] keyBytes = Convert.FromBase64String(publicKey);
            AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);

            IAsymmetricBlockCipher signer = new Pkcs1Encoding(new RsaEngine());
            signer.Init(false, asymmetricKeyParameter);

            int inputLen = data.Length;
            MemoryStream memoryStream = new MemoryStream();
            int offSet = 0;
            int i = 0;
            // 对数据分段解密
            while (inputLen - offSet > 0)
            {
                byte[] cache;
                if (inputLen - offSet > MAX_DECRYPT_BLOCK)
                {
                    cache = signer.ProcessBlock(data, offSet, MAX_DECRYPT_BLOCK);
                }
                else
                {
                    cache = signer.ProcessBlock(data, offSet, inputLen - offSet);
                }
                memoryStream.Write(cache, 0, cache.Length);
                i++;
                offSet = i * MAX_DECRYPT_BLOCK;
            }
            byte[] decryptedData = memoryStream.ToArray();
            //解密
            //byte[] decryptedData = signer.ProcessBlock(data, 0, data.Length);

            return decryptedData;
        }

        /// <summary>
        /// 私钥加密
        /// </summary>
        /// <param name="data">data 源数据</param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static byte[] EncryptByPrivateKey(byte[] data, string privateKey)
        {
            byte[] privKeyBytes = Convert.FromBase64String(privateKey);
            AsymmetricKeyParameter asymmetricKeyParameter = PrivateKeyFactory.CreateKey(privKeyBytes);

            IAsymmetricBlockCipher signer = new Pkcs1Encoding(new RsaEngine());

            signer.Init(true, asymmetricKeyParameter);

            int inputLen = data.Length;
            using (MemoryStream stream = new MemoryStream())
            {
                int offSet = 0;
                byte[] cache;
                int i = 0;
                // 对数据分段加密
                while (inputLen - offSet > 0)
                {
                    if (inputLen - offSet > MAX_ENCRYPT_BLOCK)
                    {
                        cache = signer.ProcessBlock(data, offSet, MAX_ENCRYPT_BLOCK);
                    }
                    else
                    {
                        cache = signer.ProcessBlock(data, offSet, inputLen - offSet);
                    }
                    stream.Write(cache, 0, cache.Length);
                    i++;
                    offSet = i * MAX_ENCRYPT_BLOCK;
                }
                byte[] encryptedData = stream.ToArray();
                return encryptedData;
            }

        }

        public static byte[] HexStringToByteArray(string s)
        {
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;
        }
    }
}
