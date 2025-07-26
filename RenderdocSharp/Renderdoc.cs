using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace RenderdocSharp
{
    public static unsafe class Renderdoc
    {
        /// <summary>
        /// True if the API is available.
        /// </summary>
        public static bool IsAvailable => Api != null;

        /// <summary>
        /// Set the minimum version of the API you require.
        /// </summary>
        /// <remarks>Set this before you do anything else with the Renderdoc API, including <see cref="IsAvailable"/>.</remarks>
        public static Version MinimumRequired { get; set; } = new Version(1, 0, 0);

        /// <summary>
        /// Set to true to assert versions.
        /// </summary>
        public static bool AssertVersionEnabled { get; set; } = true;

        /// <summary>
        /// Version of the API available.
        /// </summary>
        [MemberNotNullWhen(true, nameof(IsAvailable))]
        public static Version? Version
        {
            get
            {
                if (!IsAvailable)
                    return null;

                int major, minor, build;
                Api->GetApiVersion(&major, &minor, &build);
                return new Version(major, minor, build);
            }
        }

        [RenderdocApiVersion(1, 0)]
        public static OverlayBits OverlayBits
        {
            get => Api->GetOverlayBits();
            set
            {
                Api->MaskOverlayBits(~value, value);
            }
        }

        [RenderdocApiVersion(1, 0)]
        public static string CaptureFilePathTemplate
        {
            get
            {
                byte* ptr = Api->GetCaptureFilePathTemplate();
                return Marshal.PtrToStringUTF8((IntPtr)ptr)!;
            }
            set
            {
                fixed (byte *ptr = Encoding.UTF8.GetBytes(value+'\0'))
                {
                    Api->SetCaptureFilePathTemplate(ptr);
                }
            }
        }

        [RenderdocApiVersion(1, 0)]
        public static int NumCaptures => Api->GetNumCaptures();

        [RenderdocApiVersion(1, 0)]
        public static bool IsTargetControlConnected => Api->IsTargetControlConnected() != 0;

        [RenderdocApiVersion(1, 0)]
        public static bool IsFrameCapturing => Api->IsFrameCapturing() != 0;

        [RenderdocApiVersion(1, 0)]
        public static bool SetCaptureOption(CaptureOption option, int integer)
        {
            return Api->SetCaptureOptionU32(option, integer) != 0;
        }

        [RenderdocApiVersion(1, 0)]
        public static bool SetCaptureOption(CaptureOption option, float single)
        {
            return Api->SetCaptureOptionF32(option, single) != 0;
        }

        [RenderdocApiVersion(1, 0)]
        public static void GetCaptureOption(CaptureOption option, out int integer)
        {
            integer = Api->GetCaptureOptionU32(option);
        }

        [RenderdocApiVersion(1, 0)]
        public static void GetCaptureOption(CaptureOption option, out float single)
        {
            single = Api->GetCaptureOptionF32(option);
        }

        [RenderdocApiVersion(1, 0)]
        public static int GetCaptureOptionU32(CaptureOption option) => Api->GetCaptureOptionU32(option);

        [RenderdocApiVersion(1, 0)]
        public static float GetCaptureOptionF32(CaptureOption option) => Api->GetCaptureOptionF32(option);

        [RenderdocApiVersion(1, 0)]
        public static void SetFocusToggleKeys(ReadOnlySpan<InputButton> buttons)
        {
            fixed (InputButton* ptr = buttons)
            {
                Api->SetFocusToggleKeys(ptr, buttons.Length);
            }
        }

        [RenderdocApiVersion(1, 0)]
        public static void SetCaptureKeys(ReadOnlySpan<InputButton> buttons)
        {
            fixed (InputButton* ptr = buttons)
            {
                Api->SetCaptureKeys(ptr, buttons.Length);
            }
        }

        [RenderdocApiVersion(1, 0)]
        public static void RemoveHooks()
        {
            Api->RemoveHooks();
        }

        [RenderdocApiVersion(1, 0)]
        public static void UnloadCrashHandler()
        {
            Api->UnloadCrashHandler();
        }

        [RenderdocApiVersion(1, 0)]
        public static void TriggerCapture()
        {
            Api->TriggerCapture();
        }

        [RenderdocApiVersion(1, 0)]
        public static Capture? GetCapture(int index)
        {
            int length = 0;
            if (Api->GetCapture(index, null, &length, null) == 0)
            {
                return null;
            }

            Span<byte> bytes = stackalloc byte[length+1];
            long timestamp;

            fixed (byte *ptr = bytes)
                Api->GetCapture(index, ptr, &length, &timestamp);

            string fileName = Encoding.UTF8.GetString(bytes.Slice(length));
            return new Capture(index, fileName, DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime);
        }

        [RenderdocApiVersion(1, 0)]
        public static bool LaunchReplayUI(bool connectTargetControl, string? commandLine)
        {
            if (commandLine == null)
            {
                return Api->LaunchReplayUI(connectTargetControl ? 1 : 0, null) != 0;
            }
            else
            {
                fixed (byte *ptr = Encoding.UTF8.GetBytes(commandLine+'\0'))
                {
                    return Api->LaunchReplayUI(connectTargetControl ? 1 : 0, ptr) != 0;
                }
            }
        }

        [RenderdocApiVersion(1, 0)]
        public static void SetActiveWindow(nint hDevice, nint hWindow)
        {
            Api->SetActiveWindow((void*)hDevice, (void*)hWindow);
        }

        [RenderdocApiVersion(1, 0)]
        public static void StartFrameCapture(nint hDevice, nint hWindow)
        {
            Api->StartFrameCapture((void*)hDevice, (void*)hWindow);
        }

        [RenderdocApiVersion(1, 0)]
        public static bool EndFrameCapture(nint hDevice, nint hWindow)
        {
            return Api->EndFrameCapture((void*)hDevice, (void*)hWindow) != 0;
        }

        [RenderdocApiVersion(1, 1)]
        public static void TriggerMultiFrameCapture(int numFrames)
        {
            AssertAtLeast(1, 1);
            Api->TriggerMultiFrameCapture(numFrames);
        }

        [RenderdocApiVersion(1, 2)]
        public static void SetCaptureFileComments(string fileName, string comments)
        {
            AssertAtLeast(1, 2);

            byte[] fileBytes = Encoding.UTF8.GetBytes(fileName+'\0');
            byte[] commentBytes = Encoding.UTF8.GetBytes(comments+'\0');

            fixed (byte* pfile = fileBytes)
            fixed (byte* pcomment = commentBytes)
            {
                Api->SetCaptureFileComments(pfile, pcomment);
            }
        }

        [RenderdocApiVersion(1, 3)]
        public static void DiscardFrameCapture(nint hdevice, nint hWindow)
        {
            AssertAtLeast(1, 3);
            Api->DiscardFrameCapture((void*)hdevice, (void*)hWindow);
        }

        [RenderdocApiVersion(1, 5)]
        public static bool ShowReplayUI()
        {
            AssertAtLeast(1, 5);
            return Api->ShowReplayUI() != 0;
        }

        [RenderdocApiVersion(1, 6)]
        public static void SetCaptureTitle(string title)
        {
            AssertAtLeast(1, 6);
            fixed (byte *ptr = Encoding.UTF8.GetBytes(title+'\0'))
                Api->SetCaptureTitle(ptr);
        }

        private static RenderdocApi *_api = null;
        private static bool _loaded = false;

        private static RenderdocApi* Api
        {
            get
            {
                if (_loaded)
                    return _api;

                lock (typeof(Renderdoc))
                {
                    // Prevent double loads.
                    if (_loaded)
                        return _api;

                    _loaded = true;
                    _api = GetApi(SystemVersionToRenderdocVersion(MinimumRequired));

                    if (_api != null)
                        AssertAtLeast(MinimumRequired.Major, MinimumRequired.Minor, MinimumRequired.Build);

                    return _api;
                }
            }
        }

        private static RenderdocApi* GetApi(RenderdocVersion minimumRequired = RenderdocVersion.Version_1_0_0)
        {
            Regex re = new Regex(@"(lib)?renderdoc(\.dll|\.so|\.dylib)(\.\d+)?", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                string moduleName = module.FileName ?? string.Empty;

                if (!re.IsMatch(moduleName))
                    continue;

                if (!NativeLibrary.TryLoad(moduleName, out IntPtr moduleHandle))
                    return null;

                if (!NativeLibrary.TryGetExport(moduleHandle, "RENDERDOC_GetAPI", out IntPtr procAddress))
                    return null;

                var RENDERDOC_GetApi = (delegate* unmanaged[Cdecl]<RenderdocVersion, RenderdocApi**, int>)procAddress;

                RenderdocApi* api;
                return RENDERDOC_GetApi(minimumRequired, &api) != 0 ? api : null;
            }

            return null;
        }

        private static void AssertAtLeast(int major, int minor, int patch = 0, [CallerMemberName] string callee = "")
        {
            if (!AssertVersionEnabled)
                return;

            if (Version!.Major < major)
                goto fail;
            else if (Version.Major > major)
                goto success;
            else if (Version.Minor < minor)
                goto fail;
            else if (Version.Minor > minor)
                goto success;
            else if (Version.Build < patch)
                goto fail;

        success:
            return;
        fail:
            Version minVersion = typeof(Renderdoc).GetMethod(callee)!.GetCustomAttribute<RenderdocApiVersionAttribute>()!.MinVersion;
            throw new NotSupportedException($"This API was introduced in RenderdocAPI {minVersion}. Current API version is {Version}.");
        }

        private static Version RenderdocVersionToSystemVersion(RenderdocVersion version)
        {
            int i = (int)version;
            return new Version(i/10000, (i % 10000)/100, i % 100);
        }

        private static RenderdocVersion SystemVersionToRenderdocVersion(Version version) =>
            (RenderdocVersion)(version.Major * 10000 + version.Minor * 100 + version.Build);
    }
}
