using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSMGen
{
	class MalformedFSMException : ApplicationException
	{
		public MalformedFSMException() {}
			
		public MalformedFSMException(string message) {}
			
		public MalformedFSMException(string message, System.Exception inner) {}
 
		protected MalformedFSMException(System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
		:base(info, context)
		{}

	}

	abstract class Statement
	{
		public FSM owner;

		public virtual void Consume(Queue<string> tokens) { }
		public virtual bool ShouldPush() { return false; }
		public virtual bool ShouldPop() { return false; }
		public virtual List<Statement> Statements() { return null; }

	}

	class NameStatement : Statement
	{
		public string name;
		public override void Consume(Queue<string> tokens)
		{
			if (FSM.IsToken(tokens.Peek()))
			{
				throw new MalformedFSMException( "Unexpected token: " + tokens.Peek() + ", expected identifier.");
			}

			name = tokens.Dequeue();
		}

	}

	class ClassStatement : NameStatement
	{ 
	
	}

	class InterfaceCommandStatement : NameStatement
	{
		public override void Consume(Queue<string> tokens)
		{
			base.Consume(tokens);

			owner.Commands.Add(name);
		}
	}

	class GlobalStatement : Statement
	{
		List<Statement> statements = new List<Statement>();

		public override List<Statement> Statements()
		{
			return statements;
		}

		public override bool ShouldPush()
		{
			return true;
		}
	}

	class StateStatement : NameStatement
	{
		List<Statement> statements = new List<Statement>();

		public override List<Statement> Statements()
		{
			return statements;
		}

		public override bool ShouldPush()
		{
			return true;
		}
	}

	class InitialStatement : Statement 
	{ 
	
	}

	class UpdateStatement : Statement 
	{
	
	}

	class TestStatement : NameStatement
	{ 
	
	}

	class TransitionStatement : Statement
	{
		string targetstate;
		string command;
		public override void Consume(Queue<string> tokens)
		{
			if (FSM.IsToken(tokens.Peek()))
				throw new MalformedFSMException("Unexpected token " + tokens.Peek() + ", expected identifier or interface command.");

			if (owner.Commands.Contains(tokens.Peek())) //this is an interfacecommand transition
			{
				command = tokens.Dequeue();
			}

			if (FSM.IsToken(tokens.Peek()))
			{
				throw new MalformedFSMException("Unexpected token " + tokens.Peek() + ", expected identifier.");
			}
			targetstate = tokens.Dequeue();
		}
	}

	class GenericPopStatement : Statement
	{
		public override bool ShouldPop()
		{
			return true;
		}
	}

	class FSM
	{
		Queue<string> rawtokens;
		//string [] rawtokens;
		static public Dictionary<string, Type> Tokens;
		public HashSet<string> Commands = new HashSet<string>();

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


		public FSM(string buffer)
		{
			InitTokensDictionary();
			rawtokens = new Queue<string>(buffer.Split(null));

			while (rawtokens.Count > 0)
			{
				string token = rawtokens.Dequeue();

				if (token == "")
					continue;

				if (!IsToken(token))
					throw new MalformedFSMException("Unexpected identifier " + token + ", expected keyword.");

				Type statementtype = Tokens[token];

				Statement statement = (Statement)Activator.CreateInstance(statementtype);
				statement.owner = this;

				System.Diagnostics.Debug.Assert(statement != null);

				statement.Consume(rawtokens);
				if (statement.ShouldPush())
				{
					//we need to add it to the current state, if it exists.
					//it may not exist if this is the first statement (Global)
					if(statements.Count > 0)
						statements.Peek().Statements().Add(statement);
					statements.Push(statement);
				}
				else if (statement.ShouldPop())
				{
					lastpopped = statements.Pop();
				}
				else //add to current statement
				{
					statements.Peek().Statements().Add(statement);
				}
				


			}

			
		}
	}
}
