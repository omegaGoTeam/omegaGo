using OmegaGo.Core.Sgf.Properties.Values;

namespace OmegaGo.Core.Sgf.Parsing
{
    /// <summary>
    /// Delegate for parsers of property values
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Parsed value</returns>

    public delegate ISgfPropertyValue SgfPropertyValueParser(string value);
}
