﻿using dymaptic.GeoBlazor.Core.Components.Layers;
using Microsoft.AspNetCore.Components;

namespace dymaptic.GeoBlazor.Core.Components;

/// <summary>
///     The Map class contains properties and methods for storing, managing, and overlaying layers common to both 2D and 3D viewing. Layers can be added and removed from the map, but are rendered via a MapView (for viewing data in 2D) or a SceneView (for viewing data in 3D). Thus a map instance is a simple container that holds the layers, while the View is the means of displaying and interacting with a map's layers and basemap.
///     <a target="_blank" href="https://developers.arcgis.com/javascript/latest/api-reference/esri-Map.html">ArcGIS JS API</a>
/// </summary>
public class Map : MapComponent
{
    /// <summary>
    ///     Sets the basemap by way of the arcgis basemap name (e.g., arcgis-topographic).
    /// </summary>
    /// <remarks>
    ///     Either <see cref="ArcGISDefaultBasemap"/> or <see cref="Basemap"/> should be set, but not both.
    /// </remarks>
    [Parameter]
    public string? ArcGISDefaultBasemap { get; set; }

    /// <summary>
    ///     Specifies the surface properties for the map.
    /// </summary>
    [Parameter]
    public string? Ground { get; set; }
    
    /// <summary>
    ///     The <see cref="Basemap"/> for this map.
    /// </summary>
    /// <remarks>
    ///     Either <see cref="ArcGISDefaultBasemap"/> or <see cref="Basemap"/> should be set, but not both.
    /// </remarks>
    public Basemap? Basemap { get; set; }

    /// <summary>
    ///     A collection of operational <see cref="Layer"/>s.
    /// </summary>
    public HashSet<Layer> Layers { get; set; } = new();

    /// <inheritdoc />
    public override async Task RegisterChildComponent(MapComponent child)
    {
        switch (child)
        {
            case Basemap basemap:
                if (!basemap.Equals(Basemap))
                {
                    Basemap = basemap;
                    await UpdateComponent();
                }

                break;
            case Layer layer:
                await View!.AddLayer(layer);

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
            case Basemap _:
                Basemap = null;

                break;
            case Layer layer:
                Layers.Remove(layer);
                await UpdateComponent();

                break;
            default:
                await base.UnregisterChildComponent(child);

                break;
        }
    }

    /// <inheritdoc />
    public override void ValidateRequiredChildren()
    {
        base.ValidateRequiredChildren();
        Basemap?.ValidateRequiredChildren();

        foreach (Layer layer in Layers)
        {
            layer.ValidateRequiredChildren();
        }
    }

    /// <inheritdoc />
    public override async Task UpdateComponent()
    {
        await InvokeAsync(async () =>
        {
            StateHasChanged();
            await RenderView();
        });
    }
}