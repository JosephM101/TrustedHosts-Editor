using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace TrustedHostsEditor
{
    public class ServerInfo
    {
        string _serverName, _lastUpdateTime;
        bool _status;

        public ServerInfo(string name, string status, string lastUpdateTime)
        {
            _serverName = name;
            _lastUpdateTime = lastUpdateTime;
            if (status == "1")
            {
                _status = true;
            }
            else
            {
                _status = false;
            }
        }

        public string Name
        { get { return _serverName; } }

        public bool Status
        { get { return _status; } }

        public string LastUpdateTime
        { get { return _lastUpdateTime; } }
    }

    internal static class ServerListParser
    {
        public static bool GetServers(out List<ServerInfo> serverList)
        {
            serverList = new List<ServerInfo>();
            string serverList_FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\\Windows\\ServerManager\\ServerList.xml");
            if (File.Exists(serverList_FilePath))
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(serverList_FilePath);

                    // Read ServerInfo entries
                    XmlNodeList serverInfoList = xmlDoc.GetElementsByTagName("ServerInfo");
                    foreach (XmlNode serverInfo in serverInfoList)
                    {
                        string name = serverInfo.Attributes["name"].Value;
                        string status = serverInfo.Attributes["status"].Value;
                        string lastUpdateTime = serverInfo.Attributes["lastUpdateTime"].Value;
                        serverList.Add(new ServerInfo(name, status, lastUpdateTime));
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else return false;
        }

        public static List<string> NamesFromServerList(List<ServerInfo> servers)
        {
            if (servers != null)
            {
                return servers.Select(x => x.Name).ToList();
            }
            else
            {
                return new List<string>();
            }
        }
    }
}
