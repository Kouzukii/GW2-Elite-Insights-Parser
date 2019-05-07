using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace dotnetcoregen {
    class Program {
        
        // Files not compatible with dotnet core
        private static string[] IgnoredCSFiles = {
            "Controllers/FlashWindow.cs",
            "Properties/AssemblyInfo.cs",
            "MainForm.cs",
            "MainForm.Designer.cs",
            "Settings/SettingsForm.cs",
            "Settings/SettingsForm.Designer.cs"
        };

        private static string[] IgnoredExtensions = {
            ".settings",
            ".datasource",
            ".resx"
        };

        static void Main(string[] args) {
            string dir = Path.Join(Directory.GetCurrentDirectory(), "../../../LuckParser");
            if (!Directory.Exists(dir)) {
                Console.Error.WriteLine("Invalid startup dir.");
                return;
            }
            XElement project = XElement.Load(Path.Join(dir, "LuckParser.csproj"));
            string package = project.XPathSelectElement("/m:Project/m:PropertyGroup/m:AssemblyName").Value;
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("Project",
                    new XAttribute("Sdk", "Microsoft.NET.Sdk"),
                    new XElement("PropertyGroup",
                        new XElement("TargetFramework", "netcoreapp2.2"),
                        new XElement("AssemblyName", package),
                        new XElement("RootNamespace",
                            project.XPathSelectElement("/m:Project/m:PropertyGroup/m:RootNamespace").Value)
                    ),
                    new XElement("PropertyGroup",
                        new XElement("ApplicationIcon",
                            project.XPathSelectElement("/m:Project/m:PropertyGroup/m:ApplicationIcon").Value)
                    ),
                    new XElement("PropertyGroup",
                        new XElement("ApplicationManifest",
                            project.XPathSelectElement("/m:Project/m:PropertyGroup/m:ApplicationManifest").Value),
                        new XElement("PackageId", package),
                        new XElement("Authors", "baaron4"),
                        new XElement("Company", "baaron4"),
                        new XElement("Product", package)
                    ),
                    new XElement("ItemGroup",
                        IgnoredCSFiles.Select(f => new XElement("Compile", new XAttribute("Remove", f)))
                            .ToArray<object>()
                    ),
                    new XElement("ItemGroup",
                        project.XPathSelectElements("/m:Project/m:ItemGroup/m:*[@Include]").Select(
                            i => i.Attribute("Include").Value
                        ).Where(
                            i => IgnoredExtensions.Any(i.EndsWith)
                        ).Select(
                            i => new XElement("EmbeddedResource", new XAttribute("Remove", i))
                        ).ToArray<object>()
                    )
                )
            );
        }
    }
}