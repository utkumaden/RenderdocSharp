namespace RenderdocSharp
{
    public enum CaptureOption : int
    {
        AllowVsync = 0,
        AllowFullscreen = 1,
        ApiValidation = 2,
        CaptureCallstacks = 3,
        CaptureCallstacksOnlyDraws = 4,
        DelayForDebugger = 5,
        VerifyBufferAccess = 6,
        HookIntoChildren = 7,
        RefAllSources = 8,
        SaveAllInitials = 9,
        CaptureAllCmdLists = 10,
        DebugOutputMute = 11,
        AllowUnsupportedVendorExtensions = 12,
        SoftMemoryLimit = 13,
    }
}