﻿namespace Helpers;

/// <summary>
///     This helper used for saving information about included "plugins" with sites parsers by developers
///     name of field should be name of class with needed urls (WeatherParser.Service.Entities.Urls folder)
/// </summary>
public static class SitesHelperCollection
{
    public static readonly Guid GismeteoSaratovCollection = new("ed13908a-c2dc-4edb-bb9c-1678300a3435");
}