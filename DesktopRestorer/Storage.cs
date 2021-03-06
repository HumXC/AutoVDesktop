using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Linq;

namespace AutoVDesktop.DesktopRestorer
{
    internal class Storage
    {
        // 仅保存图标位置
        public static void SaveIconPositions(IEnumerable<NamedDesktopPoint> iconPositions, string fileName)
        {
            Program.Logger.Debug("开始保存图标位置: " + fileName);
            var xDoc = new XDocument(
                new XElement("Desktop",
                    new XElement("Icons",
                        iconPositions.Select(p => new XElement("Icon",
                            new XAttribute("x", p.X),
                            new XAttribute("y", p.Y),
                            new XText(p.Name))))));
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            var dir = Path.GetDirectoryName(fileName);
            if (dir == null)
            {
                throw new Exception("找不到文件的父目录: \n" + fileName);
            }
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using Stream outStream = File.OpenWrite(fileName);
            using var writer = XmlWriter.Create(outStream);
            xDoc.WriteTo(writer);

        }

        public static IEnumerable<NamedDesktopPoint> GetIconPositions(string fileName)
        {
            if (File.Exists(fileName) == false)
            { return Array.Empty<NamedDesktopPoint>(); }
            using Stream inStream = File.OpenRead(fileName);
            using var reader = XmlReader.Create(inStream);
            var xDoc = XDocument.Load(reader);
            return xDoc.Root.Element("Icons").Elements("Icon")
                .Select(el => new NamedDesktopPoint(el.Value, int.Parse(el.Attribute("x").Value), int.Parse(el.Attribute("y").Value)))
                .ToArray();
        }
    }
}
