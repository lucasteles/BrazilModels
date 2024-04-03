namespace BrazilModels;

interface IStringValue
{
#if NET8_0_OR_GREATER
    static abstract int ValueSize { get; }
#endif

    bool IsEmpty { get; }
}
