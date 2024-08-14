using Newtonsoft.Json;

namespace Orbit.Client.Serializer;

public class Serializer
{
    public string Serialize<T>(T obj)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        return JsonConvert.SerializeObject(obj, settings);
    }

    public T Deserialize<T>(string str)
    {
        var settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        var configuration = JsonConvert.DeserializeObject<T>(str, settings);
        return configuration;
    }
}