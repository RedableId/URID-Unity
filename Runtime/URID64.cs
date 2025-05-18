using System;
using System.Runtime.InteropServices;
using System.Text;

namespace URID
{
	[StructLayout(LayoutKind.Explicit, Size = BytesCount)]
	public unsafe struct URID64 : IEquatable<URID64>, IComparable<URID64>
	{
		public const int BytesCount = 8;
		public const int BitsCount = BytesCount * 8;

		[FieldOffset(0)] public fixed byte Bytes[BytesCount];
		[FieldOffset(0)] public ulong Encoded;

		public URID64(ulong encoded)
		{
			Encoded = encoded;
		}

		public int CompareTo(URID64 other)
			=> Encoded.CompareTo(other.Encoded);

		public readonly bool Equals(URID64 other)
			=> Encoded == other.Encoded;

		public override readonly bool Equals(object obj)
			=> obj is URID64 other && Encoded == other.Encoded;

		public override readonly int GetHashCode()
			=> (int)(Encoded ^ (Encoded >> 32));
	}
}