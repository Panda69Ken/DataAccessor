using System.Security.Cryptography;
using System.Text;

namespace My.Framework.Foundation.Encrypt
{
    /// <summary>
    /// Aes加密/解密类。
    /// </summary>
    public class AesTool
    {

        /// <summary>
        /// 使用AES加密字符串,按128位处理key
        /// </summary>
        /// <param name="content">加密内容</param>
        /// <param name="key">128位秘钥</param>
        /// <returns>Base64字符串结果</returns>
        public static string AesEncrypt(string content, string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(content);
            byte[] resultArray;

            using (SymmetricAlgorithm des = Rijndael.Create())
            {
                des.Key = keyArray;
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = des.CreateEncryptor();
                resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            }
            return Convert.ToBase64String(resultArray);
        }

        /// <summary>
        /// 使用AES解密字符串,按128位处理key
        /// </summary>
        /// <param name="content">内容</param>
        /// <param name="key">秘钥，需要128位、256位.....</param>
        /// <returns>UTF8解密结果</returns>
        public static string AesDecrypt(string content, string key)
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(content);
            string result = string.Empty;

            using (SymmetricAlgorithm des = Rijndael.Create())
            {

                des.Key = keyArray;
                des.Mode = CipherMode.ECB;
                des.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = des.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                result = Encoding.UTF8.GetString(resultArray);
            }
            return result;
        }

    }
}
