using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MineSweeperTools
{
    internal static unsafe partial class NativeMethods
    {
        // https://docs.microsoft.com/en-us/dotnet/standard/native-interop/cross-platform
        // Library path will search
        // win => __DllName, __DllName.dll
        // linux, osx => __DllName.so, __DllName.dylib

        static NativeMethods()
        {
            NativeLibrary.SetDllImportResolver(typeof(NativeMethods).Assembly, DllImportResolver);
        }

        static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName == __DllName)
            {
                var name = libraryName;
                var ext = "";
                var prefix = "";
                var platform = "";

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    platform = "win";
                    prefix = "";
                    ext = ".dll";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    platform = "osx";
                    prefix = libraryName.StartsWith("lib") ? "" : "lib";
                    ext = ".dylib";
                }
                else
                {
                    platform = "linux";
                    prefix = libraryName.StartsWith("lib") ? "" : "lib";
                    ext = ".so";
                }

                var arch = RuntimeInformation.OSArchitecture switch
                {
                    Architecture.Arm64 => "arm64",
                    Architecture.X64 => "x64",
                    Architecture.X86 => "x86",
                    _ => throw new NotSupportedException(),
                };

                var libName = Path.Combine("runtimes", $"{platform}-{arch}", "native", $"{prefix}{name}{ext}");
                return NativeLibrary.Load(libName, assembly, searchPath);
            }

            return IntPtr.Zero;
        }
    } 
}
