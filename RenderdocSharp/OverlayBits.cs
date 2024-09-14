using System;

namespace RenderdocSharp
{
    [Flags]
    public enum OverlayBits : int
    {
        Enabled = 1 << 0,
        FrameRate = 1 << 1,
        FrameNumber = 1 << 2,
        CaptureList = 1 << 3,
        
        Default = Enabled | FrameRate | FrameNumber | CaptureList,
        All = ~0,
        None = 0
    }
}