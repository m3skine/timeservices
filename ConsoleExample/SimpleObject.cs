using System;
using TimeServices.Engine;
using System.Runtime.InteropServices;
using TimeServices.Engine.Core;
namespace ConsoleExample
{
    public struct SimpleObject : IObject
    {
        public string Type;
        public SimpleType.Data Data;
    }
}
