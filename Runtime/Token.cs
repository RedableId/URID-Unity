namespace URID
{
	[System.Flags]
	public enum TokenType : byte
	{
		LetterLower = 0x01,
		LetterUpper = 0x02,
		Letter = LetterLower | LetterUpper,
		Digit = 0x04,
		Separator = 0x08,
	}
}