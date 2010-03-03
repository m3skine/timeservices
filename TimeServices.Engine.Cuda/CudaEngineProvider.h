#pragma once
#include "Core\CuContext.h"
#include "Engine.h"
using namespace System;
namespace TimeServices { namespace Engine {
	public ref class CudaEngineProvider : IEngineProvider
	{
	private:
		ObjectCollection^ _objects;
		TypeCollection^ _types;

	public:
		CudaEngineProvider()
			: _objects(nullptr), _types(nullptr) {
			if (!s_context.Initialize())
				throw gcnew Exception(L"Test");
		}
		~CudaEngineProvider()
		{
			s_context.Dispose();
		}

#pragma region Engine
		property ObjectCollection^ Objects
		{
			virtual ObjectCollection^ get() { return _objects; }
		}

		property TypeCollection^ Types
		{
			virtual TypeCollection^ get() { return _types; }
		}

		virtual void EvaluateFrame(Int64 time)
		{
			s_engine.EvaluateFrame(time);
		}
#pragma endregion

	};
}}
