#pragma once
#include "Core\CalContext.h"
#include "Engine.h"
using namespace System;

namespace TimeServices { namespace Engine {
	using namespace Core;
	public ref class CalEngineProvider : IEngineProvider
	{
	private:
		ObjectCollection^ _objects;
		TypeCollection^ _types;

	public:
		CalEngineProvider()
			: _objects(nullptr), _types(nullptr) {
			if (!s_context.Initialize())
				throw gcnew Exception(L"Test");
		}
		~CalEngineProvider()
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
