using System;
using System.Reflection;
using Microsoft.Win32;
namespace TimeServices.Engine
{
    public static class EngineProvider
    {
        private static Type s_engineProviderType;

        static EngineProvider()
        {
            Assembly assembly;
            int caps = GetTightestProvider();
            switch (caps)
            {
                case 1:
                    assembly = Assembly.LoadFile(@"C:\_ROAMING\_APPLICATION\GOOGLE_\TIMESERVICES\x64\Debug\TimeServices.Engine.Cuda.dll");
                    s_engineProviderType = assembly.GetType("TimeServices.Engine.CudaEngineProvider", true);
                    break;
                case 2:
                    assembly = Assembly.LoadFile(@"C:\_ROAMING\_APPLICATION\GOOGLE_\TIMESERVICES\x64\Debug\TimeServices.Engine.AtiStreams.dll");
                    s_engineProviderType = assembly.GetType("TimeServices.Engine.CalEngineProvider", true);
                    break;
                default:
                    assembly = Assembly.LoadFile(@"C:\_ROAMING\_APPLICATION\GOOGLE_\TIMESERVICES\x64\Debug\TimeServices.Engine.Cpu.dll");
                    s_engineProviderType = assembly.GetType("TimeServices.Engine.CpuEngineProvider", true);
                    break;
            }
        }

        public static IEngineProvider CreateProvider()
        {
            return (Activator.CreateInstance(s_engineProviderType) as IEngineProvider);
        }

        public static int GetTightestProvider()
        {
            return (GetHasCuda() ? 1 : (GetHasCal() ? 2 : 0));
        }

        public static bool GetHasCuda()
        {
            string cudaPath = (Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\NVIDIA Corporation\Installed Products\NVIDIA CUDA", "InstallDir", null) as string);
            return (!string.IsNullOrEmpty(cudaPath));
        }

        public static bool GetHasCal()
        {
            return false;
        }
    }
}
