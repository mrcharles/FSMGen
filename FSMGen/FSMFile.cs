using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FSMGen
{
    public class FSMFile
    {
        public Config config;
        
        string fullname;
        string name;
        string path;

        public FSMFile(string file, Config _config)
        {
            fullname = Path.GetFileName(file);
            name = Path.GetFileNameWithoutExtension(file);
            path = Path.GetDirectoryName(file);
            config = _config;
        }

        public void ResetImplementation()
        {
            string name = ImplementationFile;

            StreamWriter w = new StreamWriter(name, false);

            w.WriteLine("// FSM Implementation File");

            w.Close();
        }

        public string SourceFile
        {
            get
            {
                return Path.Combine(path, fullname);
            }
        }

        public string DefinitionFile
        {
            get
            {
                return Path.Combine(path, name + config.DefinitionExt);
            }
        }

        public string ImplementationFile
        {
            get
            {
                return Path.Combine(path, name + config.ImplementationExt);
            }
        }
    }
}
