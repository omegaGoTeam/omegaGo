# Settings - cross-platform
## Basic settings - `ISettingsService`
`OmegaGo.UI.Services.Settings.ISettingsService`

The main interface for settings storage and retrieval. The interface provides two methods:

### `T GetSetting<T>(string key, Func<T> defaultValueBuilder, SettingLocality locality)`
This method retrieves a setting by key. You must provide a default value using `defaultValueBuilder`.
In case the setting was not yet stored, the default value will be created.
`SettingLocality` specifies, whether the setting should be roamed across users devices (if this functionality is available on the target operating system). Locality defaults to **local**.

### `void SetSetting<T>(string key, T value, SettingLocality locality)`
This method stores a setting by key. In case the setting was already in the store, it is overwritten, otherwise it is created.
`SettingLocality` specifies, whether the setting should be roamed across users devices (if this functionality is available on the target operating system). Locality defaults to **local**.

These basic methods should be used for storing simple values only - `int`, `double`, `string`, etc. For more complex values, use the additional methods in `SettingsServiceBase`.

## Recommended way to use `ISettingsService` - settings manager
The best way to use `ISettingsService` is to create a wrapper (we can call it a **settings manager**), that will take the service as a dependency and will provide user-friendly, strongly-typed properties for individual settings.

```
public class MySettings
{
    private readonly ISettingsService _settings;

    public MySettings( ISettingsService settings )
    {
        _settings = settings;
    }

    private const string ASettingKey = "ASetting";

    public string ASetting
    {
        get 
        { 
            return _settings.GetSetting( ASettingKey, () => "default", SettingLocality.Roamed );
        }
        set
        {            
            _settings.SetSetting( ASettingKey, value, SettingLocality.Roamed );
        }
    }
}
```
Note that it is **not recommended** to use `nameof()` operator for setting keys, because the property names could change due to refactoring and would therefore reset some user settings.

The `OmegaGo.UI.Services.Settings.GameSettings` class is a great example of this approach.



## Storing complex data - `SettingsServiceBase`
`OmegaGo.UI.Services.Settings.SettingsServiceBase`

This abstract base settings service provides two additional methods: `GetComplexSetting` and `SetComplexSetting`.
These methods add a layer of JSON serialization on top of the functionality of the basic `ISettingsService`. This way you can store more complex classes and structures.
Because the JSON serialization requires a parameterless constructor, the type of the stored data is restricted to have it.

The serialization is transparent, which means that the caller doesn't know that it is actually happening and deals directly with strongly typed data.

## Grouping related settings - `SettingsGroup`
`OmegaGo.UI.Services.Settings.SettingsGroup`

The need to store a group of related settings is quite common. `SettingsGroup` simplifies this scenario by providing an abstract base class for this task.

You can create your own class deriving from `SettingsGroup` like this:

```
public class CarSettingsGroup : SettingsGroup
{
    public CarSettingsGroup( ISettingsService service ) : base( "Car" , service)
    {
    }

    private const string MaxSpeedKey = "MaxSpeed";

    public int MaxSpeed
    {
        get
        {
            return GetSetting(MaxSpeedKey, () => "LocalTest");
        }
        set
        {
            SetSetting(MaxSpeedKey, value);
        }
    }

    // other car properties ...
}

```

You can use the `SettingsGroup`-based classes in your **settings managers**:

```
public CarSettingsGroup Car { get; } = new CarSettingsGroup( _settings );
```

Now you can access the car properties with the syntax `settingsManager.Car.MaxSpeed`.



# Settings - platform specific

## `SettingsService` (UWP)
`OmegaGo.UI.WindowsUniversal.Services.Settings.SettingsService`

This class implements the `ISettingsService` for UWP platform. It uses the built in `ApplicationData` feature and supports both `Local` and `Roamed` settings.

Be aware that the `Roamed` settings may reach at most 100 KB (`ApplicationData.RoamingStorageQuota`) otherwise the roaming will stop working altogether and some entries will need to be removed.