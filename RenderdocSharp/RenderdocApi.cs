namespace RenderdocSharp
{
#pragma warning disable CS0649
    internal unsafe struct RenderdocApi
    {
        public delegate* unmanaged[Cdecl]<int*, int*, int*, void> GetApiVersion;

        public delegate* unmanaged[Cdecl]<CaptureOption, int, int> SetCaptureOptionU32;
        public delegate* unmanaged[Cdecl]<CaptureOption, float, int> SetCaptureOptionF32;
        public delegate* unmanaged[Cdecl]<CaptureOption, int> GetCaptureOptionU32;
        public delegate* unmanaged[Cdecl]<CaptureOption, float> GetCaptureOptionF32;

        public delegate* unmanaged[Cdecl]<InputButton*, int, void> SetFocusToggleKeys;
        public delegate* unmanaged[Cdecl]<InputButton*, int, void> SetCaptureKeys;

        public delegate* unmanaged[Cdecl]<OverlayBits> GetOverlayBits;
        public delegate* unmanaged[Cdecl]<OverlayBits, OverlayBits, void> MaskOverlayBits;

        public delegate* unmanaged[Cdecl]<void> RemoveHooks;
        public delegate* unmanaged[Cdecl]<void> UnloadCrashHandler;
        public delegate* unmanaged[Cdecl]<byte*, void> SetCaptureFilePathTemplate;
        public delegate* unmanaged[Cdecl]<byte*> GetCaptureFilePathTemplate;

        public delegate* unmanaged[Cdecl]<int> GetNumCaptures;
        public delegate* unmanaged[Cdecl]<int, byte*, int*, long*, int> GetCapture;
        public delegate* unmanaged[Cdecl]<void> TriggerCapture;
        public delegate* unmanaged[Cdecl]<int> IsTargetControlConnected;
        public delegate* unmanaged[Cdecl]<int, byte*, int> LaunchReplayUI;

        public delegate* unmanaged[Cdecl]<void*, void*, void> SetActiveWindow;
        public delegate* unmanaged[Cdecl]<void*, void*, void> StartFrameCapture;
        public delegate* unmanaged[Cdecl]<int> IsFrameCapturing;
        public delegate* unmanaged[Cdecl]<void*, void*, int> EndFrameCapture;

        // 1.1
        public delegate* unmanaged[Cdecl]<int, void> TriggerMultiFrameCapture;

        // 1.2
        public delegate* unmanaged[Cdecl]<byte*, byte*, void> SetCaptureFileComments;

        // 1.3
        public delegate* unmanaged[Cdecl]<void*, void*, int> DiscardFrameCapture;

        // 1.5
        public delegate* unmanaged[Cdecl]<int> ShowReplayUI;

        // 1.6
        public delegate* unmanaged[Cdecl]<byte*, void> SetCaptureTitle;
    }
#pragma warning restore CS0649
}
