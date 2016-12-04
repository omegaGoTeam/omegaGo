﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmCross.Platform.WindowsCommon.Converters;
using MvvmCross.Plugins.Visibility;
using OmegaGo.UI.Converters;

namespace OmegaGo.UI.WindowsUniversal.Converters
{
    public class NativeLocalizingConverter : MvxNativeValueConverter<LocalizingConverter> { }

    public class NativeEnumLocalizingConverter : MvxNativeValueConverter<EnumLocalizingConverter> { }

    public class NativeVisibilityConverter : MvxNativeValueConverter<MvxVisibilityValueConverter> { }
}
