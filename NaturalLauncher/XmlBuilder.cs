﻿/*
 * Natural Launcher

Copyright (C) 2018  Mael "Khelben" Vignaux

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the

GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
	
You can contact me with any question at the mail : mael.vignaux@elseware-experience.com
*/
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace NaturalLauncher
{
    class XmlBuilder
    {

        public static bool BuildXmlDocument(string ManifestRootPath)
        { 
            DirectoryInfo dir = new DirectoryInfo(ManifestRootPath);
            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"),
            StartCreateXML(dir));

            try
            {
                File.WriteAllText(ManifestRootPath + Path.DirectorySeparatorChar + "Manifest.xml", doc.ToString()); //write the new voidy ignore manifest
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public static XElement StartCreateXML(DirectoryInfo dir) // create a full directory xml
        {
            var xmlInfo = new XElement("MainDirectory");
            //get all the files first
            foreach (var file in dir.GetFiles())
            {
                xmlInfo.Add(new XElement("file", new XAttribute("name", file.Name)));
            }
            //get subdirectories
            var subdirectories = dir.GetDirectories().ToList().OrderBy(d => d.Name);
            foreach (var subDir in subdirectories)
            {
                xmlInfo.Add(CreateSubdirectoryXML(subDir));
            }
            return xmlInfo;
        }

        public static XElement CreateSubdirectoryXML(DirectoryInfo dir)
        {
            //get directories
            var xmlInfo = new XElement("folder", new XAttribute("name", dir.Name));
            //get all the files first
            foreach (var file in dir.GetFiles())
            {
                xmlInfo.Add(new XElement("file", new XAttribute("name", file.Name)));
            }
            //get subdirectories
            var subdirectories = dir.GetDirectories().ToList().OrderBy(d => d.Name);
            foreach (var subDir in subdirectories)
            {
                xmlInfo.Add(CreateSubdirectoryXML(subDir));
            }
            return xmlInfo;
        }

        public static bool ReadConfigXml(out string HLFolder, out bool isXP, out string customDiscordStatus, out bool keepLauncherAlive, out bool keepCustomFiles)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(Launcher.curDir + Path.DirectorySeparatorChar + Launcher.configName);
                XmlNodeList nodelist = doc.SelectNodes("/LauncherConfiguration");
            }
            catch
            {
                throw new FileNotFoundException("Could not read xml config file, please check read only status");
            }
            try
            {
                HLFolder = doc.SelectSingleNode("//HLFolder").InnerText;
            }
            catch
            {
                HLFolder = "";
            }
            try
            {
                isXP = doc.SelectSingleNode("//isXP").InnerText == "True";
            }
            catch
            {
                isXP = false;
            }
            try
            {
                customDiscordStatus = doc.SelectSingleNode("//DiscordStatus").InnerText;
            }
            catch
            {
                customDiscordStatus = "Gather forming";
            }
            try
            {
                keepLauncherAlive = doc.SelectSingleNode("//keeplauncherAlive").InnerText == "True";
            }
            catch
            {
                keepLauncherAlive = true;
            }
            try
            {
                keepCustomFiles = doc.SelectSingleNode("//keepCustomFiles").InnerText == "True";
            }
            catch
            {
                keepCustomFiles = false;
            }

            return true;

        }

        public static bool CreateConfigXml()
        {
            var xmlInfo = new XElement("LauncherConfiguration");
            xmlInfo.Add(new XElement("HLFolder", Launcher.HLFolder));
            xmlInfo.Add(new XElement("isXP", Launcher.isExperimental.ToString()));
            xmlInfo.Add(new XElement("DiscordStatus", Launcher.discordCustomStatus));
            xmlInfo.Add(new XElement("keeplauncherAlive", Launcher.keepLauncherAlive.ToString()));
            xmlInfo.Add(new XElement("keepCustomFiles", Launcher.keepCustomFiles.ToString()));

            var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"), xmlInfo);

            try
            {
                File.WriteAllText(Launcher.curDir + Path.DirectorySeparatorChar + Launcher.configName, doc.ToString()); //write the new voidy ignore manifest
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
