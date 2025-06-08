using System.Runtime.CompilerServices;

namespace URID
{
	public static class MathI
	{
		/// <summary> Calculates how many bits are needed to represent a given value. </summary>
		/// <param name="value"> The value to analyze. </param>
		/// <returns> The number of bits required to represent the value. </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Log2(ulong value)
		{
			if (value == 0)
				return 0;

			int result = 1;
			if ((value & 0xFFFFFFFF00000000ul) != 0) { value >>= 32; result += 32; }
			if ((value & 0x00000000FFFF0000ul) != 0) { value >>= 16; result += 16; }
			if ((value & 0x000000000000FF00ul) != 0) { value >>= 8; result += 8; }
			if ((value & 0x00000000000000F0ul) != 0) { value >>= 4; result += 4; }
			if ((value & 0x000000000000000Cul) != 0) { value >>= 2; result += 2; }
			if ((value & 0x0000000000000002ul) != 0) { result += 1; }

			return result;
		}
	}
}