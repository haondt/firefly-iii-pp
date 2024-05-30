using Haondt.Web.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Haondt.Web.Authentication.Pages
{
    public class NeedsAuthenticationDefaultPageEntryFactory(DefaultPageEntryFactoryData factoryData)
        : DefaultPageEntryFactory(factoryData), INeedsAuthenticationRegisteredPageEntryFactory
    {
    }
}
