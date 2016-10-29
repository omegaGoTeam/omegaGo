#Internationalization
##`ILocalizationService`
This is the main interface for localization in Omega Go. It provides a `string` indexer that returns resource translations for a given key.
```
public interface ILocalizationService
{
    /// <summary>
    /// Provides localization of a given key
    /// </summary>
    /// <param name="key">Key to localize</param>
    /// <returns>Localization of the key</returns>
    string this[ string key ] { get; }
}
```

##Localization service
The base class for localization services is the `OmegaGo.UI.Services.Localization.LocalizationService` class. Its constructor expects the `ResourceManager` of the Resource class you want to use for localization and then uses it to provide translations.

###Example usage
Suppose you have created resource files TestResources.resx and TestResources.cs.resx, which generated a `TestResources` class. If you want to use this resource class for localization, you first create an instance of the `LocalizationService` passing in the `ResourceManager` as constructor argument:

```
var localizationService = new LocalizationService( TestResources.ResourceManager );
```

Now you can use this instance to find the localization for given resources like this:

```
var localized = localizationService[ "aKeyToLocalize" ];
```

The default behavior localizes for the `CultureInfo.CurrentUICulture`. If you want to choose the language manually, use the `GetString( string, CultureInfo )` method:

```
var localizedInJapan = localizationService.GetString( "keyToLocalize", CultureInfo.GetCultureInfo( "ja-JP" );
```

##Best practices
The Resource files should be stored in the `OmegaGo.UI.Resources` assembly. To compile it correctly, you will need to install the [Multilingual App Toolkit extension for Visual Studio](https://developer.microsoft.com/cs-cz/windows/develop/multilingual-app-toolkit). This extension makes it simple to maintain localization for multiple languages. The default strings (en) are input manually in the main RESX files and MAT generates the RESX for other supported languages.

Main resource file for UI strings is `OmegaGo.UI.Resources.LocalizedStrings`. Unless you need to group strings for some specific purpose together, I recommend to add your strings here. 

The `OmegaGo.UI.Services.Localization.Localizer` class is provided for simple usage with `LocalizedStrings` resources. This class is also available as a property of both the `ViewBase` and `ViewModelBase` classes in the UI, so that it is possible to use it anytime required in the UI.