using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using FSMGen.Visitors;
using FSMGen.Statements;

namespace FSMGen
{

	class FSM
	{
		Queue<string> rawtokens;
		//string [] rawtokens;
		static public Dictionary<string, Type> Tokens;
		public HashSet<string> Commands = new HashSet<string>();
        int currentLine = 0;
		Stack<Statement> statements = new Stack<Statement>();

        FSMFile file;

		//temp hack so we don't pop the entire parsed fsm off the stack and thus lose it.
		Statement lastpopped;

        static Dictionary<string, Type> GetTokenDictionary()
        {
            Dictionary<string, Type> dict = new Dictionary<string,Type>(StringComparer.CurrentCultureIgnoreCase);
            foreach (Type type in System.Reflection.Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach ( TokenAttribute token in type.GetCustomAttributes(typeof(TokenAttribute), true))
                {
                    System.Diagnostics.Debug.Assert(!dict.ContainsKey(token.id), "Multiple classes are flagged to handle the same token, this is not supported.");
                    dict.Add(token.id, type);
                }
            }
            return dict;
        }

        static void InitTokensDictionary()
		{
			if (Tokens == null)
			{
                Tokens = GetTokenDictionary();

                
                //Tokens.Add("startfsm", typeof(GlobalStatement));
                //Tokens.Add("class", typeof(ClassStatement));
                //Tokens.Add("interfacecommand", typeof(InterfaceCommandStatement));
                //Tokens.Add("state", typeof(StateStatement));
                //Tokens.Add("test", typeof(TestStatement));
                //Tokens.Add("initial", typeof(InitialStatement));
                //Tokens.Add("update", typeof(UpdateStatement));
                //Tokens.Add("transition", typeof(TransitionStatement));
                //Tokens.Add("endstate", typeof(GenericPopStatement));
                //Tokens.Add("endfsm", typeof(GenericPopStatement));

				//tokens.Add("class", new Token() { argcount = 1, StatementType = typeof(ClassStatement) });
				//tokens.Add("interfacecommand", new Token() { argcount = 1, StatementType = typeof(InterfaceCommandStatement) });
				//tokens.Add("state", new Token() { argcount = 1, StatementType = typeof(StateStatement) });

			}
		}

		static public bool IsToken(string token)
		{
			if (Tokens.ContainsKey(token))
				return true;
			return false;
		}

		public FSM(FSMFile _file)
		{
            file = _file;
			InitTokensDictionary();

            StreamReader stream = new StreamReader(file.SourceFile);

            try
            {

                while (!stream.EndOfStream)
                {
                    currentLine++;
                    rawtokens = new Queue<string>(stream.ReadLine().Split(null));

                    while (rawtokens.Count > 0)
                    {
                        string token = rawtokens.Dequeue();

                        if (token == "")
                            continue;

                        if (!IsToken(token))
                            throw new MalformedFSMException("Unexpected identifier " + token + ", expected keyword.", currentLine);

                        Type statementtype = Tokens[token];

                        Statement statement = (Statement)Activator.CreateInstance(statementtype);
                        statement.owner = this;
                        statement.line = currentLine;

                        System.Diagnostics.Debug.Assert(statement != null);

                        statement.Consume(rawtokens);
                        if (statement.ShouldPush())
                        {
                            //we need to add it to the current state, if it exists.
                            //it may not exist if this is the first statement (Global)
                            if (statements.Count > 0)
                                statements.Peek().Statements().Add(statement);
                            statements.Push(statement);
                        }
                        else if (statement.ShouldPop())
                        {
                            lastpopped = statements.Pop();

                            if (statements.Count > 0)
                                statements.Peek().Statements().Add(statement);
                        }
                        else //add to current statement
                        {
                            statements.Peek().Statements().Add(statement);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                stream.Close();
            }

            if (statements.Count > 0) //we have terminated without closing the FSM.
            {
                throw new MalformedFSMException("Unexpected EOF: Did you forget an endstate/endfsm?", currentLine);
            }

		}

        public void AcceptVisitor(FSMVisitor visitor)
        {
            lastpopped.AcceptVisitor(visitor);
        }
		
		public void Export(Config config)
		{
            foreach (Type t in config.VisitorTypes())
            {
                BaseVisitor v = Activator.CreateInstance(t, new object[] { config, file }) as BaseVisitor;

                v.Init();
                lastpopped.AcceptVisitor(v);
                v.End();
            }
		}
	}

}
