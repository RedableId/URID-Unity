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
		[FieldOffset(0)] public ulong EncodedId;

		public Urid64(ulong encodedId)
			=> EncodedId = encodedId;

		public Urid64(ReadOnlySpan<char> decodedId)
			=> Codec.Encode(decodedId, out EncodedId, out _);

		public int CompareTo(Urid64 other)
			=> EncodedId.CompareTo(other.EncodedId);

		public readonly bool Equals(Urid64 other)
			=> EncodedId == other.EncodedId;

		public readonly override bool Equals(object obj)
			=> obj is Urid64 other && EncodedId == other.EncodedId;

		public readonly override int GetHashCode()
			=> (int)(EncodedId ^ (EncodedId >> 32));

		public readonly override string ToString()
		{
			const int decodedCapacity = 32; // TODO: calculate more precisely
			var decodedBuffer = stackalloc char[decodedCapacity];
			Codec.Decode(EncodedId, new Span<char>(decodedBuffer, decodedCapacity), out int decodedCharsCount);
			return new string(decodedBuffer, 0, decodedCharsCount);
		}
	}
}