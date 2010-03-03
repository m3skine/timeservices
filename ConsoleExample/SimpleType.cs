using System;
using TimeServices.Engine;
using System.Runtime.InteropServices;
using TimeServices.Engine.Core;
namespace ConsoleExample
{
    public class SimpleType : IType
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Data
        {
            int Value;
        }
    }
}
