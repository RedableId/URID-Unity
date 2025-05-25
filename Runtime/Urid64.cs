using System;
using System.Runtime.InteropServices;

namespace URID
{
	[StructLayout(LayoutKind.Explicit, Size = BytesCount)]
	public unsafe struct Urid64 : IEquatable<Urid64>, IComparable<Urid64>
	{
		public const int BytesCount = 8;
		public const int BitsCount = BytesCount * 8;

		[FieldOffset(0)] public fixed byte Bytes[BytesCount];
		[FieldOffset(0)] public ulong Encoded;

		public Urid64(ulong encoded)
		{
			Encoded = encoded;
		}

		public int CompareTo(Urid64 other)
			=> Encoded.CompareTo(other.Encoded);

		public readonly bool Equals(Urid64 other)
			=> Encoded == other.Encoded;

		public override readonly bool Equals(object obj)
			=> obj is Urid64 other && Encoded == other.Encoded;

		public override readonly int GetHashCode()
			=> (int)(Encoded ^ (Encoded >> 32));

		public override readonly string ToString()
		{
			const int resultCapacity = 32; // TODO: calculate more precisely
			var resultBuffer = stackalloc char[resultCapacity];
			var resultLength = 0;

			Codec.DecodeUnsafe(resultBuffer, resultCapacity, ref resultLength, Encoded, BitsCount);

			return new string(resultBuffer, 0, resultLength);
		}
	}
}