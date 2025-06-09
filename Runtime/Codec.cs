using System;
using System.Runtime.CompilerServices;

namespace URID
{
	public static class Codec
	{
		private static readonly IFormatProvider FormatProvider = System.Globalization.CultureInfo.InvariantCulture;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Decode(
			ulong encodedId,
			Span<char> decodedId,
			out int decodedCharsCount,
			char wordsSeparator = Defaults.WordsSeparator,
			char indexSeparator = Defaults.IndexSeparator
		)
		{
			if (wordsSeparator == '\0')
				DecodePascalCase(encodedId, decodedId, out decodedCharsCount, indexSeparator);
			else
				DecodeSeparated(encodedId, decodedId, out decodedCharsCount, wordsSeparator, indexSeparator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void DecodePascalCase(
			ulong encodedId,
			Span<char> decodedId,
			out int decodedCharsCount,
			char indexSeparator
		)
		{
			int encodedIdBitsRemain = 64;
			ulong encodedLetters = default;
			int encodedLettersCount = default;
			decodedCharsCount = 0;
			do
				if (DecodePrefix(encodedId, ref encodedIdBitsRemain, ref encodedLetters, ref encodedLettersCount))
					DecodeLettersPascalCase(encodedLetters, encodedLettersCount, decodedId, ref decodedCharsCount);
				else
					break;
			while (encodedIdBitsRemain > 0);

			DecodeIndex(encodedId, encodedIdBitsRemain, decodedId, ref decodedCharsCount, indexSeparator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void DecodeSeparated(ulong encodedId, Span<char> decodedId, out int decodedCharsCount, char wordsSeparator, char indexSeparator)
		{
			int encodedIdBitsRemain = 64;
			ulong encodedLetters = default;
			int encodedLettersCount = default;
			decodedCharsCount = 0;
			do
				if (DecodePrefix(encodedId, ref encodedIdBitsRemain, ref encodedLetters, ref encodedLettersCount))
					DecodeLettersSeparated(encodedLetters, encodedLettersCount, decodedId, ref decodedCharsCount, wordsSeparator);
				else
					break;
			while (encodedIdBitsRemain > 0);

			DecodeIndex(encodedId, encodedIdBitsRemain, decodedId, ref decodedCharsCount, indexSeparator);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool DecodePrefix(ulong encodedId, ref int encodedIdBitsRemain, ref ulong encodedLetters, ref int encodedLettersCount)
		{
			int encodedPrefixBitsCount = Alphabet.GetWordPrefixBitsCount(encodedIdBitsRemain);
			if (encodedPrefixBitsCount <= 0)
				return false;

			ulong encodedPrefixMask = (1ul << encodedPrefixBitsCount) - 1;
			encodedLettersCount = (int)((encodedId >> (encodedIdBitsRemain - encodedPrefixBitsCount)) & encodedPrefixMask);
			encodedIdBitsRemain -= encodedPrefixBitsCount;

			if (encodedLettersCount == 0)
				return false;

			int encodedLettersBitsCount = Alphabet.GetWordLettersBitsCount(encodedLettersCount);
			ulong encodedLettersMask = (1ul << encodedLettersBitsCount) - 1;
			encodedLetters = (encodedId >> (encodedIdBitsRemain - encodedLettersBitsCount)) & encodedLettersMask;
			encodedIdBitsRemain -= encodedLettersBitsCount;
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void DecodeLettersPascalCase(ulong encodedLetters, int encodedLettersCount, Span<char> decodedId, ref int decodedCharsCount)
		{
			ulong letterCode;
			int decodedWordBegin = decodedCharsCount;
			for (int decodedCharIndex = decodedWordBegin + encodedLettersCount - 1; decodedCharIndex > decodedWordBegin; --decodedCharIndex)
			{
				letterCode = encodedLetters % Alphabet.LettersCount;
				encodedLetters /= Alphabet.LettersCount;
				decodedId[decodedCharIndex] = Alphabet.DecodeLower((int)letterCode);
			}

			letterCode = encodedLetters % Alphabet.LettersCount;
			decodedId[decodedWordBegin] = Alphabet.DecodeUpper((int)letterCode);

			decodedCharsCount += encodedLettersCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void DecodeLettersSeparated(ulong encodedLetters, int encodedLettersCount, Span<char> decodedId, ref int decodedCharsCount, char wordsSeparator)
		{
			if (decodedCharsCount != 0)
				decodedId[decodedCharsCount++] = wordsSeparator;

			int decodedWordBegin = decodedCharsCount;
			for (int decodedCharIndex = decodedWordBegin + encodedLettersCount - 1; decodedCharIndex >= decodedWordBegin; --decodedCharIndex)
			{
				var letterCode = encodedLetters % Alphabet.LettersCount;
				encodedLetters /= Alphabet.LettersCount;
				decodedId[decodedCharIndex] = Alphabet.DecodeLower((int)letterCode);
			}

			decodedCharsCount += encodedLettersCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void DecodeIndex(ulong encodedId, int encodedBitsRemain, Span<char> decodedId, ref int decodedCharsCount, char indexSeparator)
		{
			ulong index = encodedId & ((1ul << encodedBitsRemain) - 1ul);
			if (index == 0ul)
				return;

			if (indexSeparator != '\0')
				decodedId[decodedCharsCount++] = indexSeparator;

			index.TryFormat(decodedId.Slice(decodedCharsCount), out var indexLength, null, FormatProvider);
			decodedCharsCount += indexLength;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Encode(ReadOnlySpan<char> decodedId, out ulong encodedId, out EncodingError error)
		{
			const int encodedBitsCount = 64;
			error = EncodingError.None;
			Encode(decodedId, out encodedId, out var nameBitsCount, out var index);
			encodedId |= index;
			if (nameBitsCount > encodedBitsCount)
				error |= EncodingError.LettersOverflow;

			if (index != 0ul && nameBitsCount > encodedBitsCount - MathI.Log2(index))
				error |= EncodingError.IndexOverflow;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Encode(ReadOnlySpan<char> decodedId, out ulong encodedName, out int encodedNameBitsCount, out ulong encodedIndex)
		{
			const int encodedNameCapacity = 64;
			encodedName = 0;
			encodedNameBitsCount = 0;
			int descriptorNextIndex = 0;
			ulong code = default;
			encodedIndex = 0ul;
			while (descriptorNextIndex < decodedId.Length)
			{
				TokenType token;
				do token = Alphabet.Encode(decodedId[descriptorNextIndex++], ref code);
				while (token == TokenType.Separator);

				if ((token & TokenType.Letter) != 0) // word
				{
					ulong encodedLetters = code;
					int encodedLettersCount = 1;
					while (descriptorNextIndex < decodedId.Length)
					{
						token = Alphabet.Encode(decodedId[descriptorNextIndex], ref code);
						if (token != TokenType.LetterLower)
							break;

						encodedLetters = encodedLetters * Alphabet.LettersCount + code;
						++encodedLettersCount;
						++descriptorNextIndex;
					}

					int encodedPrefixBitsCount = Alphabet.GetWordPrefixBitsCount(encodedNameCapacity - encodedNameBitsCount);
					int encodedLettersBitsCount = Alphabet.GetWordLettersBitsCount(encodedLettersCount);
					int encodedWordBitsCount = encodedPrefixBitsCount + encodedLettersBitsCount;
					ulong encodedWord = ((ulong)encodedLettersCount << encodedLettersBitsCount) | encodedLetters;
					encodedName |= encodedWord << (encodedNameCapacity - encodedNameBitsCount - encodedWordBitsCount);
					encodedNameBitsCount += encodedWordBitsCount;
				}
				else // index
				{
					for (encodedIndex = code; descriptorNextIndex < decodedId.Length; ++descriptorNextIndex)
					{
						token = Alphabet.Encode(decodedId[descriptorNextIndex], ref code);
						if (token == TokenType.Digit)
							encodedIndex = encodedIndex * 10ul + code;
						else
							break;
					}

					encodedName += encodedIndex;
				}
			}
		}
	}
}