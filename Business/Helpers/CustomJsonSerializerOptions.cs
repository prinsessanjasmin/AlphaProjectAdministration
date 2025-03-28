﻿using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;

namespace Business.Helpers;

public static class CustomJsonSerializerOptions
{
    public static readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true, 
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
    };
}