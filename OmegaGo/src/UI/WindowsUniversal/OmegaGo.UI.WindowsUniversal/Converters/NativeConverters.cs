using MvvmCross.Platform.WindowsCommon.Converters;
using MvvmCross.Plugins.Visibility;
using OmegaGo.UI.Converters;

namespace OmegaGo.UI.WindowsUniversal.Converters
{
    public class NativeLocalizingConverter : MvxNativeValueConverter<LocalizingConverter> { }

    public class NativeEnumLocalizingConverter : MvxNativeValueConverter<EnumLocalizingConverter> { }

    public class NativeConcreteEnumLocalizingConverter : MvxNativeValueConverter<ConcreteEnumLocalizingConverter> { }

    public class NativeVisibilityConverter : MvxNativeValueConverter<MvxVisibilityValueConverter> { }

    public class NativeInvertedVisibilityConverter : MvxNativeValueConverter<MvxInvertedVisibilityValueConverter> { }

    public class NativeTypeNameLocalizingConverter : MvxNativeValueConverter<TypeNameLocalizingConverter> { }

    public class NativeEnumSelectionConverter : MvxNativeValueConverter<EnumSelectionConverter> { }

    public class NativeBoolDoubleConverter : MvxNativeValueConverter<BoolDoubleConverter> { }

    public class NativeBoolNegationConverter : MvxNativeValueConverter<BoolNegationConverter> { }

    public class NativeStringToUpperConverter : MvxNativeValueConverter<StringToUpperConverter> { }
}
