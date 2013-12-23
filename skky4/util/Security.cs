using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace skky.util
{
	public static class Security
	{
		#region Extension Methods
		public static string HashString(this string s)
		{
			byte[] data1ToHash = s.EncodeUnicode();
			byte[] hashvalue = (new MD5CryptoServiceProvider()).ComputeHash(data1ToHash);

			return BitConverter.ToString(hashvalue);
		}

		public static bool CompareHashString(this string str1, string str2)
		{
			byte[] data1ToHash = str1.EncodeUnicode();
			byte[] data2ToHash = str2.EncodeUnicode();
			byte[] hashvalue1 = (new MD5CryptoServiceProvider()).ComputeHash(data1ToHash);
			byte[] hashvalue2 = (new MD5CryptoServiceProvider()).ComputeHash(data2ToHash);
			int i = 0;
			bool bval = true;
			do
			{
				if (hashvalue1[i] != hashvalue2[i])
				{
					bval = false;
					break;
				}

				i++;
			} while (i < hashvalue1.Length);

			return bval;
		}
		#endregion

		public static string HashPassword(string password)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(password);
			byte[] inArray = HashAlgorithm.Create("SHA1").ComputeHash(bytes);

			return Convert.ToBase64String(inArray);
		}
		// Use this one if you are looking for an MD5 hashing. All ASCII.
		public static string HashPasswordMD5(string password)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(password);
			byte[] inArray = HashAlgorithm.Create("MD5").ComputeHash(bytes);

			return Convert.ToBase64String(inArray);
		}
		// This one returns binary characters.
		public static string EncryptPasswordMD5(string data)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(data));

			return Encoding.UTF8.GetString(result);
		}
		// This one returns all ASCII, but with 2 char and dash separators.
		public static string EncryptPasswordMD5Ascii(string originalPassword)
		{
			string enc = EncryptPasswordMD5AsciiWithDashes(originalPassword);
			if (!string.IsNullOrEmpty(enc))
				enc = enc.ToLower().Replace("-", "");

			return enc;
		}
		// This one returns all ASCII, but with 2 char and dash separators.
		public static string EncryptPasswordMD5AsciiWithDashes(string originalPassword)
		{
			//Declarations
			Byte[] originalBytes;
			Byte[] encodedBytes;
			MD5 md5;

			//Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
			md5 = new MD5CryptoServiceProvider();
			originalBytes = ASCIIEncoding.Default.GetBytes(originalPassword);
			encodedBytes = md5.ComputeHash(originalBytes);

			//Convert encoded bytes back to a 'readable' string
			return BitConverter.ToString(encodedBytes);
		}

		public static string EncryptString(string Message, string Passphrase)
		{
			if (Passphrase == null)
				throw new Exception("Invalid Passphrase in string encryption.");

			byte[] Results;
			System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

			// Step 1. We hash the passphrase using MD5
			// We use the MD5 hash generator as the result is a 128 bit byte array

			// which is a valid length for the TripleDES encoder we use below

			MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
			byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

			// Step 2. Create a new TripleDESCryptoServiceProvider object
			TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
			// Step 3. Setup the encoder
			TDESAlgorithm.Key = TDESKey;
			TDESAlgorithm.Mode = CipherMode.ECB;
			TDESAlgorithm.Padding = PaddingMode.PKCS7;

			// Step 4. Convert the input string to a byte[]
			byte[] DataToEncrypt = UTF8.GetBytes(Message);
			// Step 5. Attempt to encrypt the string
			try
			{
				ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
				Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
			}
			finally
			{
				// Clear the TripleDes and Hashprovider services of any sensitive information
				TDESAlgorithm.Clear();
				HashProvider.Clear();
			}
			// Step 6. Return the encrypted string as a base64 encoded string
			return Convert.ToBase64String(Results);
		}

		public static string DecryptString(string Message, string Passphrase)
		{
			byte[] Results;

			System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
			// Step 1. We hash the passphrase using MD5
			// We use the MD5 hash generator as the result is a 128 bit byte array
			// which is a valid length for the TripleDES encoder we use below

			MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
			byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

			// Step 2. Create a new TripleDESCryptoServiceProvider object
			TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

			// Step 3. Setup the decoder
			TDESAlgorithm.Key = TDESKey;
			TDESAlgorithm.Mode = CipherMode.ECB;
			TDESAlgorithm.Padding = PaddingMode.PKCS7;

			// Step 4. Convert the input string to a byte[]
			byte[] DataToDecrypt = Convert.FromBase64String(Message);
			// Step 5. Attempt to decrypt the string
			try
			{
				ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
				Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
			}
			finally
			{
				// Clear the TripleDes and Hashprovider services of any sensitive information
				TDESAlgorithm.Clear();
				HashProvider.Clear();
			}
			// Step 6. Return the decrypted string in UTF8 format
			return UTF8.GetString(Results);
		}
	}
}
