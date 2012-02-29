using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FSMGen.Lang
{
    class Template
    {
        //List<Section> Sections;
        Dictionary<string, string> Sections;
        HashSet<string> SectionNames;

        string SectionTitle;

        public Template(string delimiter, List<string> sections)
        {
            SectionTitle = delimiter;
            SectionNames = new HashSet<string>(sections);
            Sections = new Dictionary<string, string>();
        }

        string GetSectionTitle(string line)
        {
            if (line.Trim().StartsWith(SectionTitle))
            {
                return line.Trim().Substring( SectionTitle.Length );
            }

            return null;
        }

        public void Parse(StreamReader reader)
        {
            string activesection = null;
            string data = "";

            while (!reader.EndOfStream)
            {
                string buffer = reader.ReadLine();
                
                string section = GetSectionTitle( buffer );

                if (section != null)
                {
                    if (activesection != null)
                    {
                        SectionNames.Add(activesection);
                        Sections[activesection] = data;
                    }

                    activesection = section;
                    data = "";
                }
                else 
                {
                    data += buffer;
                }

            }


        }
    }
}
