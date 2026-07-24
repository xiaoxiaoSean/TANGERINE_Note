using System.Security.Cryptography;
using System.Text;
//最先由ChatGPT编写
//Written by ChatGPT at first
public static class CryptoHelper
{
    public static byte[] Encrypt(string text, string password)
    {
        byte[] plaintext = Encoding.UTF8.GetBytes(text);

        // 随机盐
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        // 根据密码生成密钥
        byte[] key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256,
            32
        );

        // AES-GCM需要12字节Nonce
        byte[] nonce = RandomNumberGenerator.GetBytes(12);

        byte[] ciphertext = new byte[plaintext.Length];
        byte[] tag = new byte[16];


        using (AesGcm aes = new AesGcm(key))
        {
            aes.Encrypt(
                nonce,
                plaintext,
                ciphertext,
                tag
            );
        }


        // 保存：
        // salt + nonce + tag + 密文
        byte[] result = new byte[
            salt.Length +
            nonce.Length +
            tag.Length +
            ciphertext.Length
        ];

        int offset = 0;

        Buffer.BlockCopy(salt, 0, result, offset, salt.Length);
        offset += salt.Length;

        Buffer.BlockCopy(nonce, 0, result, offset, nonce.Length);
        offset += nonce.Length;

        Buffer.BlockCopy(tag, 0, result, offset, tag.Length);
        offset += tag.Length;

        Buffer.BlockCopy(ciphertext, 0, result, offset, ciphertext.Length);


        return result;
    }


    public static string Decrypt(byte[] data, string password)
    {
        int offset = 0;


        byte[] salt = data[offset..(offset + 16)];
        offset += 16;


        byte[] nonce = data[offset..(offset + 12)];
        offset += 12;


        byte[] tag = data[offset..(offset + 16)];
        offset += 16;


        byte[] ciphertext = data[offset..];


        byte[] key = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256,
            32
        );


        byte[] plaintext = new byte[ciphertext.Length];


        using (AesGcm aes = new AesGcm(key))
        {
            aes.Decrypt(
                nonce,
                ciphertext,
                tag,
                plaintext
            );
        }


        return Encoding.UTF8.GetString(plaintext);
    }
}