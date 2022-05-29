﻿using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Linq;

namespace AutoVDesktop.DesktopRestorer
{
    internal class Storage
    {
        // 保存桌面设置和图标位置
        public static void SaveIconsAndRegistry(IEnumerable<NamedDesktopPoint> iconPositions, IDictionary<string, string> registryValues, string fileName)
        {
            Program.Logger.Debug("开始保存图标位置: " + fileName);
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
            string filePath = Path.Combine(Environment.CurrentDirectory, "Desktops", fileName + ".xml");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                var dirName = Path.GetDirectoryName(filePath);
                if (dirName != null)
                {
                    Directory.CreateDirectory(dirName);
                }


            }
            using Stream outStream = File.OpenWrite(filePath);
            using var writer = XmlWriter.Create(outStream);
            xDoc.WriteTo(writer);

        }

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
            string fileDir = Path.Combine(Environment.CurrentDirectory, "Desktops");
            string filePath = Path.Combine(fileDir, fileName + ".xml");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            else if (!Directory.Exists(filePath)) Directory.CreateDirectory(fileDir);
            using Stream outStream = File.OpenWrite(filePath);
            using var writer = XmlWriter.Create(outStream);
            xDoc.WriteTo(writer);

        }

        public static IEnumerable<NamedDesktopPoint> GetIconPositions(string fileName)
        {
            using var storage = IsolatedStorageFile.GetUserStoreForAssembly();
            if (storage.FileExists(fileName) == false)
            { return Array.Empty<NamedDesktopPoint>(); }

            using Stream inStream = File.OpenRead(fileName);
            using var reader = XmlReader.Create(inStream);
            var xDoc = XDocument.Load(reader);
            return xDoc.Root.Element("Icons").Elements("Icon")
                .Select(el => new NamedDesktopPoint(el.Value, int.Parse(el.Attribute("x").Value), int.Parse(el.Attribute("y").Value)))
                .ToArray();
        }

        public static IDictionary<string, string> GetRegistryValues(string fileName)
        {
            using var storage = IsolatedStorageFile.GetUserStoreForAssembly();
            if (storage.FileExists(fileName) == false)
            { return new Dictionary<string, string>(); }

            using var stream = storage.OpenFile(fileName, FileMode.Open);
            using var reader = XmlReader.Create(stream);
            var xDoc = XDocument.Load(reader);

            return xDoc.Root.Element("Registry").Elements("Value")
                .ToDictionary(el => el.Element("Name").Value, el => el.Element("Data").Value);
        }
    }
}