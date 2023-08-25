using System.Text.Json;

namespace BudgetDataLoader;

public class Config
{
    private readonly Dictionary<string, object> _configKeys;

    private readonly string[] _requiredKeys = new string[]
    {
        "DatabasePath",
        "DeleteDatabase"
    };
    
    public string? DatabasePath
    {
        get => _configKeys["DatabasePath"].ToString();
    }

    public bool DeleteDatabase { get; private set; }
    
    public Config()
    {
        _configKeys = GetConfigKeys(ReadConfigFile());
        ValidateConfig();
    }
    
    private static string ReadConfigFile()
    {
        string filePath = Path.Combine(System.AppContext.BaseDirectory, "config.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("config.json not found in the application base directory!");
        }
        return File.ReadAllText(filePath);
    }
    
    private static Dictionary<string, object> GetConfigKeys(string jsonString)
    {
        JsonSerializerOptions opts = new()
        {
            AllowTrailingCommas = true,
        };
        return JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, opts);
    }

    private void ValidateConfig()
    {
        foreach (string key in _configKeys.Keys)
        {
            if (_configKeys[key] is null)
            {
                throw new InvalidOperationException($"The key {key} has NULL value in the config file");
            }
        }

        foreach (string key in _requiredKeys)
        {
            if (!_configKeys.ContainsKey(key))
            {
                throw new InvalidOperationException($"The key {key} is missing in the config file!");
            }
        }
        
        try
        {
            JsonElement del = (JsonElement)_configKeys["DeleteDatabase"];
            DeleteDatabase = del.GetBoolean();
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}