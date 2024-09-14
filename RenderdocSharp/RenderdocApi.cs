namespace RenderdocSharp
{
#pragma warning disable CS0649
    internal unsafe struct RenderdocApi
    {
        public delegate*<int*, int*, int*, void> GetApiVersion;
        
        public delegate*<CaptureOption, int, int> SetCaptureOptionU32;
        public delegate*<CaptureOption, float, int> SetCaptureOptionF32;
        public delegate*<CaptureOption, int> GetCaptureOptionU32;
        public delegate*<CaptureOption, float> GetCaptureOptionF32;

        public delegate*<InputButton*, int, void> SetFocusToggleKeys;
        public delegate*<InputButton*, int, void> SetCaptureKeys;

        public delegate*<OverlayBits> GetOverlayBits;
        public delegate*<OverlayBits, OverlayBits, void> MaskOverlayBits;

        public delegate*<void> RemoveHooks;
        public delegate*<void> UnloadCrashHandler;
        public delegate*<byte*, void> SetCaptureFilePathTemplate;
        public delegate*<byte*> GetCaptureFilePathTemplate;

        public delegate*<int> GetNumCaptures;
        public delegate*<int, byte*, int*, long*, int> GetCapture;
        public delegate*<void> TriggerCapture;
        public delegate*<int> IsTargetControlConnected;
        public delegate*<int, byte*, int> LaunchReplayUI;

        public delegate*<void*, void*, void> SetActiveWindow;
        public delegate*<void*, void*, void> StartFrameCapture;
        public delegate*<int> IsFrameCapturing;
        public delegate*<void*, void*, int> EndFrameCapture;

        // 1.1
        public delegate*<int, void> TriggerMultiFrameCapture;

        // 1.2
        public delegate*<byte*, byte*, void> SetCaptureFileComments;

        // 1.3
        public delegate*<void*, void*, int> DiscardFrameCapture;

        // 1.5
        public delegate*<int> ShowReplayUI;

        // 1.6
        public delegate*<byte*, void> SetCaptureTitle;
    }
#pragma warning restore CS0649
}
