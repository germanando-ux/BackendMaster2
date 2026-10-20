using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Infraestructure.Settings;

public class QwenSettings
{
    public const string SectionName = "AiSettings:Qwen";
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Model { get; set; } = "text-embedding-v3";
}
