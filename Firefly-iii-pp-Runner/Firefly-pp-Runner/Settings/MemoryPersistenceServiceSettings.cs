using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Settings
{
    public class MemoryPersistenceServiceSettings
    {
        public Dictionary<string, List<string>> ExistingCollections { get; set; } = new Dictionary<string, List<string>>();
    }
}
