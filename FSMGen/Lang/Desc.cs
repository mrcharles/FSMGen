using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace FSMGen.Lang
{
    class Desc
    {
        public string TokenDelimiter = "%";
        
        public string StateToken = "state";
        public string TransitionToken = "transition";
        public string CommandToken = "command";
        public string ClassToken = "class";

        public string SectionDelimiter = "%%";
        public string HeaderToken = "header";
        public string MainToken = "main";
        public string FooterToken = "footer";

        public bool UsesGlobalCommands = true;

        public string GlobalCommandsTemplate = "globalcommands.template";

        string GlobalCommandsHeader;
        string GlobalCommandsMain;
        string GlobalCommandsFooter;
                                                    
        void InitializeGlobalCommandsTemplate()
        {
            StreamReader reader = new StreamReader(GlobalCommandsTemplate);
            string buffer = reader.ReadToEnd();

            //string [] sections = buffer.Split( new string[] { SectionDelimiter }, StringSplitOptions.None );





        }

        public bool UsesDeclaration = true;

        public string DeclarationTemplate;

        public string FSMDeclarationTemplate;
        public string StateDeclarationTemplate;
        public string TransitionDeclarationTemplate;
        public string InterfaceCommandDeclarationTemplate;
        public string InterfaceTransitionDeclarationTemplate;

        public bool UsesInitialization = true;

        public Desc()
        { 
        
        }


    }
}
