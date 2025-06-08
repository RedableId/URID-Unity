using System;
using System.Runtime.InteropServices;

namespace URID
{
	[StructLayout(LayoutKind.Explicit, Size = BytesCount)]
	public unsafe struct Urid128 : IEquatable<Urid128>, IComparable<Urid128>
	{
		public const int BytesCount = 16;
		public const int BitsCount = BytesCount * 8;

		[FieldOffset(0)] public fixed byte Bytes[BytesCount];
		[FieldOffset(0)] public ulong High;
		[FieldOffset(8)] public ulong Low;

		public Urid128(Guid guid)
		{
			Low = default;
			High = default;
			fixed (byte* bytesPtr = Bytes)
			{
				guid.TryWriteBytes(new Span<byte>(bytesPtr, BytesCount));
			}
		}

		public static implicit operator Guid(Urid128 urid)
			=> new Guid(new ReadOnlySpan<byte>(urid.Bytes, BytesCount));

		public int CompareTo(Urid128 other)
			=> High != other.High
				? High.CompareTo(other.High)
				: Low.CompareTo(other.Low);

		public readonly bool Equals(Urid128 other)
			=> Low == other.Low && High == other.High;

		public readonly override bool Equals(object obj)
			=> obj is Urid128 other && Low == other.Low && High == other.High;

		public readonly override int GetHashCode()
			=> (int)(Low ^ (Low >> 32) ^ High ^ (High >> 32));
	}
}