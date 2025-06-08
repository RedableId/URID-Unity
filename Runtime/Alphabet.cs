using System.Runtime.CompilerServices;
using Unity.Mathematics;

namespace URID
{
	public static class Alphabet
	{
		public const int LettersCount = 26;

		// https://cs.wellesley.edu/~fturbak/codman/letterfreq.html
		public const string MajorEndingLetterLower = "denrst";
		public const string MajorEndingLetterUpper = "DENRST";

		private const int LetterToCodeCount = 123;
		private static readonly int[] LetterToCode = new int[LetterToCodeCount]{
			99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, // 00..15
			99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, // 16..31
			99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, 99, // 32..47
			~0, ~1, ~2, ~3, ~4, ~5, ~6, ~7, ~8, ~9, 99, 99, 99, 99, 99, 99, // '0'..'9', 58..63
			99, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, // 64, 'A'..'O'
			41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 99, 99, 99, 99, 99, // 'P'..'Z', 91..95
			99,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14, // 96, 'a'..'o'
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25                      // 'p'..'z'
		};

		// LettersCountToBits[i] = ceiling(log2(26) * i)
		private static readonly int[] LettersCountToBitsCount = new int[]
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
		private static readonly int[] BitsCountToLettersCount = new int[128]
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
		public static TokenType Encode(char letter, ref ulong code)
		{
			int index = letter;
			if (index >= LetterToCodeCount)
				return TokenType.Separator;

			switch (LetterToCode[index])
			{
				case < 0:
					code = (ulong)(~LetterToCode[index]);
					return TokenType.Digit;
				case < LettersCount:
					code = (ulong)LetterToCode[index];
					return TokenType.LetterLower;
				case < LettersCount * 2:
					code = (ulong)(LetterToCode[index] - LettersCount);
					return TokenType.LetterUpper;
				default:
					return TokenType.Separator;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char DecodeLower(int code)
			=> (char)('a' + code);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static char DecodeUpper(int code)
			=> (char)('A' + code);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetWordPrefixBitsCount(int bitsRemain)
		{
			int lettersRemain = BitsCountToLettersCount[bitsRemain];
			var prefixBitsCount = (int)math.ceil(math.log2(lettersRemain));
			if (bitsRemain <= prefixBitsCount)
				return 0;

			lettersRemain = BitsCountToLettersCount[bitsRemain - prefixBitsCount];
			prefixBitsCount = (int)math.ceil(math.log2(lettersRemain));
			return prefixBitsCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetWordLettersBitsCount(int wordLettersCount)
			=> LettersCountToBitsCount[wordLettersCount];
	}
}