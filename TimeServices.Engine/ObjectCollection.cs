using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using TimeServices.Engine.Core;
namespace TimeServices.Engine
{
    public class ObjectCollection : Collection<IObject>
    {
        public ObjectCollection(IList<IObject> list)
            : base(list) { }
    }
}
