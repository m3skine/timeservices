using System;
using System.Reflection;
namespace TimeServices.Engine
{
    public static class EngineProvider
    {
        private static Type s_engineProviderType;

        static EngineProvider()
        {
            Assembly assembly; 
            int caps = 1;
            switch (caps)
            {
                case 1:
                    assembly = Assembly.LoadFile(@"C:\_ROAMING\_APPLICATION\GOOGLE_\TIMESERVICES\x64\Debug\TimeServices.Engine.AtiStreams.dll");
                    s_engineProviderType = assembly.GetType("TimeServices.Engine.CalEngineProvider", true);
                    break;
                case 2:
                    assembly = Assembly.LoadFile(@"C:\_ROAMING\_APPLICATION\GOOGLE_\TIMESERVICES\x64\Debug\TimeServices.Engine.Cuda.dll");
                    s_engineProviderType = assembly.GetType("TimeServices.Engine.CudaEngineProvider", true);
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
    }
}
