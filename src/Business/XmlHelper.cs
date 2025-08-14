using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

public static class XmlHelper
{
    public static string SerializeToXml<T>(T obj)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));

        var settings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            OmitXmlDeclaration = false
        };

        using (var stringWriter = new StringWriter())
        using (var writer = XmlWriter.Create(stringWriter, settings))
        {
            xmlSerializer.Serialize(writer, obj);
            return stringWriter.ToString();
        }
    }
}
