using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

public static class SerializationExtension
{
    public static T Deserialize<T>(this string json)
    {
        var bytes = Encoding.Unicode.GetBytes(json);
        using (var stream = new MemoryStream(bytes))
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return (T)serializer.ReadObject(stream);
        }
    }

    public static string Serialize(this object instance)
    {
        using (var stream = new MemoryStream())
        {
            var serializer = new DataContractJsonSerializer(instance.GetType());
            serializer.WriteObject(stream, instance);
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
