using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace URID
{
	public static class Alphabet
	{
		public const int LettersCount = 26;
		private const string CodeToLetterLower = "abcdefghijklmnopqrstuvwxyz";
		private const string CodeToLetterUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		// https://cs.wellesley.edu/~fturbak/codman/letterfreq.html
		public const string MajorEndingLetterLower = "denrst";
		public const string MajorEndingLetterUpper = "DENRST";

		private const int LetterToCodeCount = 123;
		private readonly static int[] LetterToCode = new int[LetterToCodeCount]{
			99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, // 00..15
			99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, // 16..31
			99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, // 32..47
			~0, ~1, ~2, ~3, ~4, ~5, ~6, ~7, ~8, ~9, 99, 99, 99, 99, 99, 99, // '0'..'9', 58..63
			99,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, // 64, 'A'..'O'
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 99, 99, 99, 99, 99, // 'P'..'Z', 91..95
			99,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, // 96, 'a'..'o'
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25                      // 'p'..'z'
		};

		// LettersCountToBits[i] = ceiling(log2(26) * i)
		private readonly static int[] LettersCountToBitsCount = new int[]
		{
			  0,   5,  10,  15,
			 19,  24,  29,  33,
			 38,  43,  48,  52,
			 57,  62,  66,  71,
			 76,  80,  85,  90,
			 95,  99, 104, 109,
			113, 118, 123, 127,
		};

		// CodeToLetterCount[i] = floor(i / log2(26))
		private readonly static int[] BitsCountToLettersCount = new int[128]
		{
			 0,  0,  0,  0,  0,
			 1,  1,  1,  1,  1,
			 2,  2,  2,  2,  2,
			 3,  3,  3,  3,
			 4,  4,  4,  4,  4,
			 5,  5,  5,  5,  5,
			 6,  6,  6,  6,
			 7,  7,  7,  7,  7,
			 8,  8,  8,  8,  8,
			 9,  9,  9,  9,  9,
			10, 10, 10, 10,
			11, 11, 11, 11, 11,
			12, 12, 12, 12, 12,
			13, 13, 13, 13,
			14, 14, 14, 14, 14,
			15, 15, 15, 15, 15,
			16, 16, 16, 16,
			17, 17, 17, 17, 17,
			18, 18, 18, 18, 18,
			19, 19, 19, 19, 19,
			20, 20, 20, 20,
			21, 21, 21, 21, 21,
			22, 22, 22, 22, 22,
			23, 23, 23, 23,
			24, 24, 24, 24, 24,
			25, 25, 25, 25, 25,
			26, 26, 26, 26,
			27, 
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TokenType Encode(char letter, ref int code)
		{
			int index = letter;
			if (index >= LetterToCodeCount)
				return TokenType.Separator;

			code = LetterToCode[index];
			if (code >= LettersCount)
				return TokenType.Separator;

			if (code >= 0)
				return TokenType.Letter;

			code = ~code;
			return TokenType.Digit;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char DecodeLowerUnsafe(int code)
			=> CodeToLetterLower[code];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char DecodeUpperUnsafe(int code)
			=> CodeToLetterUpper[code];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetBitsCountUnsafe(int lettersCount)
			=> LettersCountToBitsCount[lettersCount];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetWordLengthPrefixInfo(int bitsRemain, out int prefixMask, out int prefixSize)
		{
			int lettersRemain = BitsCountToLettersCount[bitsRemain];
			prefixSize = (int)math.ceil(math.log2(lettersRemain));
			if (prefixSize <= bitsRemain)
			{
				prefixSize = 0;
				prefixMask = 0;
			}
			else
			{
				lettersRemain = BitsCountToLettersCount[bitsRemain - prefixSize];
				prefixSize = (int)math.ceil(math.log2(lettersRemain));
				prefixMask = (1 << prefixSize) - 1;
			}
		}
	}
}