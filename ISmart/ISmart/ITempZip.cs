using System;
using System.Collections;
using System.Collections.Generic;

namespace ISmart;

public interface ITempZip : IList<string>, ICollection<string>, IEnumerable<string>, IEnumerable, IDisposable
{
}
