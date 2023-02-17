using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firefly_pp_Runner.Settings
{
    public class KeyValueStoreSettings
    {
        public string Collection { get; set; }
        public string Path { get; set; }
        public string DefaultValueValue { get; set; } 
        public int AutocompleteMaxResults { get; set; }
    }
}
