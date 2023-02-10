namespace Gameloop.Vdf;

public sealed class VdfSerializerSettings
{
    public static VdfSerializerSettings Default => new();
    public static VdfSerializerSettings Common => new()
    {
        UsesEscapeSequences = true,
        UsesConditionals = true
    };

    /// <summary>
    /// Determines whether the parser should translate escape sequences (/n, /t, etc.).
    /// </summary>
    public bool UsesEscapeSequences = false;

    /// <summary>
    /// Determines whether the parser should evaluate conditional blocks ([$WINDOWS], etc.).
    /// </summary>
    public bool UsesConditionals = true;

    // System information
    //public const bool IsXBox360 = false, IsWin32 = true;
    //public const bool IsWindows = true, IsOSX = false, IsLinux = false, IsPosix = false;
}
