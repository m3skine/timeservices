using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using TimeServices.Engine.Core;
namespace TimeServices.Engine
{
    public class TypeCollection : Collection<IType>
    {
        public TypeCollection(IList<IType> list)
            : base(list) { }
    }
}
