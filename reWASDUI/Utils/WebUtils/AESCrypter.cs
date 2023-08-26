using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace reWASDUI.Utils.WebUtils
{
	public class AESCrypter
	{
		public static string EncryptString(string message, byte[] encryptKey = null)
		{
			if (encryptKey == null)
			{
				encryptKey = AESCrypter.EncryptKey;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(message);
			int num = 16 - message.Length % 16;
			if (num == 0)
			{
				num = 16;
			}
			char c = Convert.ToChar(97 + num);
			for (int i = 0; i < num; i++)
			{
				stringBuilder.Append(c);
			}
			Random random = new Random();
			byte[] array = new byte[16];
			random.NextBytes(array);
			string text = null;
			Aes aes = Aes.Create();
			try
			{
				aes.Key = encryptKey;
				aes.IV = array;
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.None;
				ICryptoTransform cryptoTransform = aes.CreateEncryptor(aes.Key, aes.IV);
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
					{
						byte[] bytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());
						cryptoStream.Write(bytes, 0, bytes.Length);
						byte[] array2 = memoryStream.ToArray();
						byte[] array3 = new byte[array.Length + array2.Length];
						Buffer.BlockCopy(array, 0, array3, 0, array.Length);
						Buffer.BlockCopy(array2, 0, array3, array.Length, array2.Length);
						text = Convert.ToBase64String(array3);
					}
				}
			}
			catch (CryptographicException)
			{
				return null;
			}
			finally
			{
				if (aes != null)
				{
					aes.Clear();
				}
			}
			return text;
		}

		public static string DecryptString(string cipherData, byte[] decryptKey = null)
		{
			if (decryptKey == null)
			{
				decryptKey = AESCrypter.DecryptKey;
			}
			byte[] array = Convert.FromBase64String(cipherData);
			byte[] array2 = new byte[16];
			Array.Copy(array, array2, 16);
			byte[] array3 = new byte[array.Length - 16];
			Array.Copy(array, 16, array3, 0, array.Length - 16);
			Aes aes = Aes.Create();
			string text2;
			try
			{
				aes.Key = decryptKey;
				aes.IV = array2;
				aes.Mode = CipherMode.CBC;
				aes.Padding = PaddingMode.None;
				using (MemoryStream memoryStream = new MemoryStream(array3))
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(decryptKey, array2), CryptoStreamMode.Read))
					{
						string text = new StreamReader(cryptoStream).ReadToEnd();
						if (text.Length > 1)
						{
							int num = (int)(text[text.Length - 1] - 'a');
							text = text.Substring(0, text.Length - num);
						}
						text2 = text;
					}
				}
			}
			catch (CryptographicException)
			{
				text2 = null;
			}
			catch (Exception)
			{
				text2 = null;
			}
			return text2;
		}

		public static byte[] StringToByteArray(string hex)
		{
			int length = hex.Length;
			byte[] array = new byte[length / 2];
			for (int i = 0; i < length; i += 2)
			{
				array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			}
			return array;
		}

		private const int ASCII_OFFSET = 97;

		private const int BLOCK_SIZE = 16;

		private static readonly byte[] EncryptKey = AESCrypter.StringToByteArray("a1c973b431aff5cef73294c351a973e6");

		private static readonly byte[] DecryptKey = AESCrypter.StringToByteArray("f17e687b3f26e53f7b1bde5239921192");
	}
}
