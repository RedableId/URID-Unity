using System;
using System.Runtime.CompilerServices;

namespace URID
{
	public static class Codec
	{
		private readonly static IFormatProvider FormatProvider = System.Globalization.CultureInfo.InvariantCulture;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static unsafe void DecodeUnsafe(char* dstBuffer, int dstCapacity, ref int dstLength, ulong srcEncoded, int srcBitsRemain)
		{
			int prefixSize;
			do
			{
				int wordBegin = dstLength;
				Alphabet.GetWordPrefixInfo(srcBitsRemain, out var prefixMask, out prefixSize);
				if (prefixSize <= 0)
					break;

				int wordLettersRemain = (int)((srcEncoded >> (srcBitsRemain - prefixSize)) & prefixMask);
				srcBitsRemain -= prefixSize;

				if (wordLettersRemain == 0)
					break;

				Alphabet.GetWordInfo(wordLettersRemain, out var wordMask, out var wordBitsCount);
				var encodedWord = (srcEncoded >> srcBitsRemain) & wordMask;
				srcBitsRemain -= wordBitsCount;
				for (; wordLettersRemain > 0; --wordLettersRemain)
				{
					ulong letterCode = encodedWord % Alphabet.LettersCount;
					encodedWord /= Alphabet.LettersCount;
					dstBuffer[dstLength++] = Alphabet.DecodeLowerUnsafe((int)letterCode);
				}

				MemoryExtensions.Reverse(new Span<char>(dstBuffer + wordBegin, dstLength - wordBegin));
			}
			while (srcBitsRemain > 0);

			DecodeIndex(dstBuffer, dstCapacity, ref dstLength, srcEncoded, srcBitsRemain);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static unsafe void DecodeIndex(char* dstBuffer, int dstCapacity, ref int dstLength, ulong encoded, int encodedBitsRemain)
		{
			ulong index = encoded & ((1ul << encodedBitsRemain) - 1ul);
			index.TryFormat(new Span<char>(dstBuffer + dstLength, dstCapacity - dstLength), out var indexLength, "N", FormatProvider);
			dstLength += indexLength;
		}
	}
}