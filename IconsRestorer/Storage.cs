using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Linq;

namespace AutoVDesktop.IconsRestorer
{
    internal class Storage
    {
        public void SaveIconPositions(IEnumerable<NamedDesktopPoint> iconPositions, IDictionary<string, string> registryValues, string fileName)
        {
            Program.Logger.Debug("开始保存图标位置: " + fileName);
            foreach (var position in iconPositions)
            {
                Console.WriteLine($"in Desktop: {position.Name} ({position.X},{position.Y})");

            }
            var xDoc = new XDocument(
                new XElement("Desktop",
                    new XElement("Icons",
                        iconPositions.Select(p => new XElement("Icon",
                            new XAttribute("x", p.X),
                            new XAttribute("y", p.Y),
                            new XText(p.Name)))),
                    new XElement("Registry",
                        registryValues.Select(p => new XElement("Value",
                            new XElement("Name", new XCData(p.Key)),
                            new XElement("Data", new XCData(p.Value)))))));
            string filePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Desktops",fileName + ".xml");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else
            {
               if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }
            }
            using (Stream outStream = File.OpenWrite(filePath))
            {
                using (var writer = XmlWriter.Create(outStream))
                {
                    xDoc.WriteTo(writer);
                }
            }

        }
        //测试用重写

        public void SaveIconPositions(IEnumerable<NamedDesktopPoint> iconPositions, string fileName)
        {
            Program.Logger.Debug("开始保存图标位置: "+fileName);
            foreach (var position in iconPositions)
            {
                Console.WriteLine($"in Desktop: {position.Name} ({position.X},{position.Y})");

            }
            var xDoc = new XDocument(
                new XElement("Desktop",
                    new XElement("Icons",
                        iconPositions.Select(p => new XElement("Icon",
                            new XAttribute("x", p.X),
                            new XAttribute("y", p.Y),
                            new XText(p.Name))))));
            string filePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Desktops",fileName + ".xml");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            using (Stream outStream = File.OpenWrite(filePath))
            {
                using (var writer = XmlWriter.Create(outStream))
                {
                    xDoc.WriteTo(writer);
                }
            }

        }

        public IEnumerable<NamedDesktopPoint> GetIconPositions(string fileName)
        {
            string filePath = Path.Combine(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase,"Desktops", fileName + ".xml");
            if (File.Exists(filePath) == false)
            {
                return new NamedDesktopPoint[0];
            }
            using (Stream inStream = File.OpenRead(filePath))
            {
                using (var reader = XmlReader.Create(inStream))
                {
                    var xDoc = XDocument.Load(reader);
                    return xDoc.Root.Element("Icons").Elements("Icon")
                        .Select(el => new NamedDesktopPoint(el.Value, int.Parse(el.Attribute("x").Value), int.Parse(el.Attribute("y").Value)))
                        .ToArray();
                }
            }
        }

        public IDictionary<string, string> GetRegistryValues(string fileName)
        {
            using (var storage = IsolatedStorageFile.GetUserStoreForAssembly())
            {
                if (storage.FileExists(fileName) == false)
                { return new Dictionary<string, string>(); }

                using (var stream = storage.OpenFile(fileName, FileMode.Open))
                {
                    using (var reader = XmlReader.Create(stream))
                    {
                        var xDoc = XDocument.Load(reader);

                        return xDoc.Root.Element("Registry").Elements("Value")
                            .ToDictionary(el => el.Element("Name").Value, el => el.Element("Data").Value);
                    }
                }
            }
        }
    }
}
