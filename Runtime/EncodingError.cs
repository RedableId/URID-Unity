namespace URID
{
    [System.Flags]
    public enum EncodingError : byte
    {
        None,
        LettersOverflow = 0x01,
        IndexOverflow = 0x02,
    }
}