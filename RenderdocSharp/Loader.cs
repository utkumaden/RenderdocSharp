using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RenderdocSharp
{
    internal static unsafe class Loader
    {
        public static RenderdocApi* Load()
        {
            if (OperatingSystem.IsWindows())
            {
                return LoadWin32();
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolverNix);
                return LoadDlfcn();
            }
            else
            {
                throw new NotSupportedException("Platform not supported.");
            }
        }

        private static nint DllImportResolverNix(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName != "dl")
                return 0;
            
            nint module = 0;

            bool _ = 
                NativeLibrary.TryLoad("libdl.so", out module) ||
                NativeLibrary.TryLoad("libdl.so.2", out module) ||
                NativeLibrary.TryLoad("libdl.so.1", out module) ||
                NativeLibrary.TryLoad("libdl.so.0", out module);

            return module;
        }

        private static RenderdocApi *LoadWin32()
        {
            nint hmodule = GetModuleHandle("renderdoc.dll");

            if (hmodule == 0)
                return null;

            var getApi = (delegate*<RenderdocVersion, RenderdocApi**, int>) GetProcAddress(hmodule, "RENDERDOC_GetAPI");

            if (getApi == null)
                return null;
            
            RenderdocApi *api;
            if (getApi(RenderdocVersion.Version_1_0_0, &api) == 0)
                return null;

            return api;

            [DllImport("Kernel32.dll")]
            extern static nint GetModuleHandle([MarshalAs(UnmanagedType.LPStr)] string name);

            [DllImport("Kernel32.dll")]
            extern static nint GetProcAddress(nint module, [MarshalAs(UnmanagedType.LPStr)] string name);
        }

        const int RTLD_LAZY = 0x00001;
        const int RTLD_NOW = 0x00002;
        const int RTLD_BINDING_MASK = 0x3;
        const int RTLD_NOLOAD = 0x00004;
        const int RTLD_DEEPBIND = 0x00008;
        const int RTLD_GLOBAL = 0x00100;
        const int RTLD_LOCAL = 0;
        const int RTLD_NODELETE = 0x01000;

        private static RenderdocApi* LoadDlfcn()
        {
            nint hmodule = dlopen("librenderdoc.so", RTLD_NOW | RTLD_NOLOAD);

            if (hmodule == 0)
                return null;

            var getApi = (delegate*<RenderdocVersion, RenderdocApi**, int>)dlsym(hmodule, "RENDERDOC_GetAPI");

            if (getApi == null)
                return null;
            
            RenderdocApi *api;
            if (getApi(RenderdocVersion.Version_1_0_0, &api) == 0)
                return null;

            return api;

            [DllImport("dl")]
            extern static nint dlopen([MarshalAs(UnmanagedType.LPStr)] string file, int flags);
            [DllImport("dl")]
            extern static nint dlsym(nint module, [MarshalAs(UnmanagedType.LPStr)] string name);
        }
    }
}
