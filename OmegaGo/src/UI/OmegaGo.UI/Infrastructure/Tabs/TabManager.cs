﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.UI.Infrastructure.Tabs
{
    public abstract class TabManager<TTabInfo> : ITabManager
        where TTabInfo : ITabInfo
    {
    }
}
