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

		//temp hack so we don't pop the entire parsed fsm off the stack and thus lose it.
		Statement lastpopped;

		static void InitTokensDictionary()
		{
			if (Tokens == null)
			{
				Tokens = new Dictionary<string, Type>(StringComparer.CurrentCultureIgnoreCase);

				Tokens.Add("startfsm", typeof(GlobalStatement));
				Tokens.Add("class", typeof(ClassStatement));
				Tokens.Add("interfacecommand", typeof(InterfaceCommandStatement));
				Tokens.Add("state", typeof(StateStatement));
				Tokens.Add("test", typeof(TestStatement));
				Tokens.Add("initial", typeof(InitialStatement));
				Tokens.Add("update", typeof(UpdateStatement));
				Tokens.Add("transition", typeof(TransitionStatement));
				Tokens.Add("endstate", typeof(GenericPopStatement));
				Tokens.Add("endfsm", typeof(GenericPopStatement));

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


		public FSM(StreamReader stream)
		{
			InitTokensDictionary();

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
            if (statements.Count > 0) //we have terminated without closing the FSM.
            {
                throw new MalformedFSMException("Unexpected EOF: Did you forget an endstate/endfsm?", currentLine);
            }

		}
		
		public void Export(StreamWriter stream)
		{ 
            //CommandsVisitor commands = new CommandsVisitor(stream);

            //commands.Init();
            //lastpopped.AcceptVisitor(commands);
            //commands.End();

			DeclarationVisitor declaration = new DeclarationVisitor(stream);

			declaration.Init();
			lastpopped.AcceptVisitor(declaration);
			declaration.End();

			InitializationVisitor init = new InitializationVisitor(stream);

			init.Init();
			lastpopped.AcceptVisitor(init);
			init.End();
		}
	}

}
