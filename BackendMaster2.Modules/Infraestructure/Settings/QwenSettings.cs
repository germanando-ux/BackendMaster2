using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Modules.Infraestructure.Settings;

public class QwenSettings
{
    public const string SectionName = "Qwen";
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions";
    public string Model { get; set; } = "qwen-turbo";
}
