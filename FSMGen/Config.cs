using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using FSMGen.Visitors;

namespace FSMGen
{
    public class ConfigData
    {
        public string commandsdb;
        //public bool useglobalcommands;
        public string commandheaderfile;
        public string[] visitors;
        public string implementationextension;
        public string definitionextension;

        public ConfigData()
        {
            commandsdb = "commands.db";
            commandheaderfile = "commands.h";
            //useglobalcommands = true;
            visitors = new string []{ "GlobalCommandVisitor", "DeclarationVisitor", "InitializationVisitor", "DefinitionVisitor" };
            implementationextension = ".fsm.h";
            definitionextension = ".cpp";

        }

    }

    class Config
    {
        ConfigData data;
        string configFile;
        string rootPath;

        public IEnumerable<Type> VisitorTypes()
        {
            foreach (string s in data.visitors)
            {
                Type visitor = Type.GetType("FSMGen.Visitors." + s, true);

                yield return visitor;
            }
        }

        public string ImplementationExt
        {
            get
            {
                return data.implementationextension;
            }
        }

        public string DefinitionExt
        {
            get
            {
                return data.definitionextension;
            }
        }

        public string CommandsDB
        {
            get
            {
                return Path.Combine(rootPath, data.commandsdb);
            }
        }

        public string CommandsHeader
        {
            get
            {
                return Path.Combine(rootPath, data.commandheaderfile);
            }
        }

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
            if (Path.IsPathRooted(configfile))
            {
                configFile = configfile;
                rootPath = Path.GetDirectoryName(configFile);
            }
            else
            {
                rootPath = Directory.GetCurrentDirectory();
                configFile = Path.Combine(rootPath, configfile);
            }

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
