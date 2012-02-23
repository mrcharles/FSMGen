using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace FSMGen
{
    public class ConfigData
    {
        public string commandsdb;
        public bool useglobalcommands;
        public string commandheaderfile;

        public ConfigData()
        {
            commandsdb = "commands.db";
            commandheaderfile = "commands.h";
            useglobalcommands = true;
        }

    }

    class Config
    {
        ConfigData data;
        public string configFile;

        public ConfigData Data
        {
            get { return data;  }
        }

        public Config()
            :this( Path.Combine(Path.GetDirectoryName( System.Reflection.Assembly.GetEntryAssembly().Location), "fsmgen.config"))
        { 
        
        }

        public Config(string configfile)
        {
            configFile = configfile;
            try
            {
                StreamReader stream = new StreamReader(configfile);
                XmlSerializer xml = new XmlSerializer(typeof(ConfigData));
                data = xml.Deserialize(stream) as ConfigData;
            }
            catch(Exception)
            {
                data = new ConfigData();
                StreamWriter stream = new StreamWriter(configfile, false);
                XmlSerializer xml = new XmlSerializer(typeof(ConfigData));
                xml.Serialize(stream, data);
            }
        }
    }
}
