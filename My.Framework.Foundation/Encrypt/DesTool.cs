using System.Security.Cryptography;
using System.Text;

namespace My.Framework.Foundation.Encrypt
{
    /// <summary>
	/// DES加密/解密类。
    /// Copyright (C) Maticsoft
	/// </summary>
	public class DesTool
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="dataString"></param>
        /// <returns></returns>
        public static string Md5(string dataString)
        {
            if (string.IsNullOrEmpty(dataString))
            {
                return dataString;
            }

            MD5 md5 = MD5.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(dataString);

            var md5Bytes = md5.ComputeHash(bytes);

            return BitConverter.ToString(md5Bytes).Replace("-", string.Empty).ToLower();
        }


        #region 临时注释的内容
        /*
/// <summary>
/// 解密
/// </summary>
/// <param name="pToDecrypt">要解密的以Base64</param>
/// <param name="key"></param>
/// <param name="iv"></param>
/// <returns>已解密的字符串</returns>
public static string DesDecrypt(string pToDecrypt, byte[] key, byte[] iv)
{
    byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);

    using (DES des = DES.Create())
    {
        var crypto = des.CreateDecryptor(key, iv);

        var block = crypto.TransformFinalBlock(inputByteArray, 0, inputByteArray.Length);

        string str = Encoding.UTF8.GetString(block);

        return str;
    }
}

*/
        #endregion

        #region 对称加密/解密
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="encryptString">需要加密的内容</param>
        /// <param name="privateKey">加密密钥</param>
        /// <returns>加密后的内容</returns>
        public static string DesEncrypt(string encryptString, string privateKey = "com.caimao")
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(privateKey.Substring(0, 8));
            byte[] keyIv = keyBytes;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateEncryptor(keyBytes, keyIv), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="decryptString">需要解密的内容</param>
        /// <param name="privateKey">加密密钥</param>
        /// <returns>解密后的内容</returns>
        public static string DesDecrypt(string decryptString, string privateKey = "com.caimao")
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(privateKey.Substring(0, 8));
            byte[] keyIV = keyBytes;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, provider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        #endregion

    }
}
