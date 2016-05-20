using System;
using System.Security.Cryptography;
using System.Text;

//
// From http://stackoverflow.com/questions/5243237/c-sharp-password-generator
//
namespace skky.Security
{
	public class PasswordGenerator
	{
		private const int DefaultMinimum = 8;
		private const int DefaultMaximum = 10;
		private const int UBoundDigit = 61;
		private const int UBoundLettersOnly = 51;
		private const int UBoundLowercaseOnly = 25;

		private int minSize = DefaultMinimum;
		private int maxSize = DefaultMaximum;
		private bool hasRepeatingCharacters = true;
		private bool hasConsecutiveCharacters = true;
		private bool excludeSymbols = true;
		private string exclusionSet;
		private char[] pwdCharArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`~!@#$%^&*()-_=+[]{}\\|;:'\",<.>/?".ToCharArray();

		public PasswordGenerator()
		{ }

		protected int GetCryptographicRandomNumber(int lBound, int uBound)
		{
			// Assumes lBound >= 0 && lBound < uBound
			// returns an int >= lBound and < uBound
			uint urndnum;
			byte[] rndnum = new Byte[4];
			if (lBound == uBound - 1)
			{
				// test for degenerate case where only lBound can be returned   
				return lBound;
			}

			uint xcludeRndBase = (uint.MaxValue - (uint.MaxValue % (uint)(uBound - lBound)));

			do
			{
				using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
				{
					rng.GetBytes(rndnum);
				}
				urndnum = System.BitConverter.ToUInt32(rndnum, 0);
			} while (urndnum >= xcludeRndBase);

			return (int)(urndnum % (uBound - lBound)) + lBound;
		}

		protected char GetRandomCharacter()
		{
			int upperBound = pwdCharArray.GetUpperBound(0);

			if (true == this.ExcludeSymbols)
			{
				upperBound = PasswordGenerator.UBoundLowercaseOnly;
			}

			int randomCharPosition = GetCryptographicRandomNumber(pwdCharArray.GetLowerBound(0), upperBound);

			char randomChar = pwdCharArray[randomCharPosition];

			return randomChar;
		}

		public static string GeneratePassword()
		{
			var pwdGenerator = new PasswordGenerator();

			return pwdGenerator.Generate();
		}

		public string Generate()
		{
			// Pick random length between minimum and maximum   
			int pwdLength = GetCryptographicRandomNumber(this.Minimum, this.Maximum);

			StringBuilder pwdBuffer = new StringBuilder();
			pwdBuffer.Capacity = this.Maximum;

			// Generate random characters
			char lastCharacter, nextCharacter;

			// Initial dummy character flag
			lastCharacter = nextCharacter = '\n';

			for (int i = 0; i < pwdLength; i++)
			{
				nextCharacter = GetRandomCharacter();

				if (false == this.ConsecutiveCharacters)
				{
					while (lastCharacter == nextCharacter)
					{
						nextCharacter = GetRandomCharacter();
					}
				}

				if (false == this.RepeatCharacters)
				{
					string temp = pwdBuffer.ToString();
					int duplicateIndex = temp.IndexOf(nextCharacter);
					while (-1 != duplicateIndex)
					{
						nextCharacter = GetRandomCharacter();
						duplicateIndex = temp.IndexOf(nextCharacter);
					}
				}

				if (!string.IsNullOrEmpty(this.Exclusions))
				{
					while (-1 != this.Exclusions.IndexOf(nextCharacter))
					{
						nextCharacter = GetRandomCharacter();
					}
				}

				pwdBuffer.Append(nextCharacter);
				lastCharacter = nextCharacter;
			}

			if (null != pwdBuffer)
			{
				return pwdBuffer.ToString();
			}
			else
			{
				return String.Empty;
			}
		}

		public string Exclusions
		{
			get { return this.exclusionSet; }
			set { this.exclusionSet = value; }
		}

		public int Minimum
		{
			get { return this.minSize; }
			set
			{
				this.minSize = value;
				if (PasswordGenerator.DefaultMinimum > this.minSize)
				{
					this.minSize = PasswordGenerator.DefaultMinimum;
				}
			}
		}

		public int Maximum
		{
			get { return this.maxSize; }
			set
			{
				this.maxSize = value;
				if (this.minSize >= this.maxSize)
				{
					this.maxSize = PasswordGenerator.DefaultMaximum;
				}
			}
		}

		public bool ExcludeSymbols
		{
			get { return this.excludeSymbols; }
			set { this.excludeSymbols = value; }
		}

		public bool RepeatCharacters
		{
			get { return this.hasRepeatingCharacters; }
			set { this.hasRepeatingCharacters = value; }
		}

		public bool ConsecutiveCharacters
		{
			get { return this.hasConsecutiveCharacters; }
			set { this.hasConsecutiveCharacters = value; }
		}
	}
}