using System;
namespace TimeServices.Engine
{
	public interface IEngineProvider : IDisposable
	{
        ObjectCollection Objects { get; }
        TypeCollection Types { get; }
        void EvaluateFrame(long time);
	}
}
