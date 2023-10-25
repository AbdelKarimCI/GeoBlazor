﻿using dymaptic.GeoBlazor.Core.Objects;
using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;


namespace dymaptic.GeoBlazor.Core.Components.Layers;

/// <summary>
///     The color visual variable is used to visualize features along a continuous color ramp based on the values of a numeric attribute field or an expression. The color ramp is defined along a sequence of stops, where color values are mapped to data values. Data values that fall between two stops are assigned a color that is linearly interpolated based on the value's position relative to the closest defined stops.
///     <a target="_blank" href="https://developers.arcgis.com/javascript/latest/api-reference/esri-renderers-visualVariables-ColorVariable.html">ArcGIS Maps SDK for JavaScript</a>
/// </summary>
public class ColorVariable : VisualVariable
{
    /// <inheritdoc />
    public override VisualVariableType VariableType => VisualVariableType.Color;

    /// <summary>
    ///     Name of the numeric attribute field by which to normalize the data. If this field is used, then the values in stops should be normalized as percentages or ratios.
    /// </summary>
    [Parameter]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NormalizationField { get; set; }

    /// <summary>
    ///     An array of sequential objects, or stops, that defines a continuous color ramp. You must specify 2 - 8 stops. In most cases, no more than five are needed. Features with values that fall between the given stops will be assigned colors linearly interpolated along the ramp in relation to the nearest stop values. The stops must be listed in ascending order based on the value of the value property in each stop.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyCollection<ColorStop>? Stops
    {
        get => _stops;
        set
        {
            if (value is not null)
            {
                _stops = new HashSet<ColorStop>(value);
            }
            else
            {
                _stops = null;
            }
        }
    }

    /// <inheritdoc />
    public override async Task RegisterChildComponent(MapComponent child)
    {
        switch (child)
        {
            case ColorStop stop:
                _stops ??= new HashSet<ColorStop>();
                _stops.Add(stop);

                break;
            default:
                await base.RegisterChildComponent(child);

                break;
        }
    }
    
    /// <inheritdoc />
    public override async Task UnregisterChildComponent(MapComponent child)
    {
        switch (child)
        {
            case ColorStop stop:
                _stops?.Remove(stop);

                break;
            default:
                await base.UnregisterChildComponent(child);

                break;
        }
    }
    
    internal override void ValidateRequiredChildren()
    {
        base.ValidateRequiredChildren();

        if (_stops is not null)
        {
            foreach (ColorStop stop in _stops)
            {
                stop.ValidateRequiredChildren();
            }
        }
    }
    
    private HashSet<ColorStop>? _stops;
}

/// <summary>
///     Defines a color stop used for creating a continuous color visualization in a color visual variable.
///     <a target="_blank" href="https://developers.arcgis.com/javascript/latest/api-reference/esri-renderers-visualVariables-support-ColorStop.html">ArcGIS Maps SDK for JavaScript</a>
/// </summary>
public class ColorStop: MapComponent
{
    /// <summary>
    ///     The Color used to render features with the given value.
    /// </summary>
    [Parameter]
    [RequiredProperty]
    public MapColor? Color { get; set; }
    
    /// <summary>
    ///     A string value used to label the stop along the color ramp in the Legend.
    /// </summary>
    [Parameter]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Label { get; set; }
    
    /// <summary>
    ///     Specifies the data value to map to the given color.
    /// </summary>
    [Parameter]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Value { get; set; }
}