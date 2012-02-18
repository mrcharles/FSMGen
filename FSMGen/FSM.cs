using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

	abstract class FSMVisitor
	{
		protected StreamWriter stream;
		public string ClassName = null;
		public FSMVisitor(StreamWriter _stream)
		{
			stream = _stream;
		}
		public abstract void Init();
		public virtual bool Valid(Statement s)
		{
			if (s is ClassStatement)
			{
				return true;
			}
			return false;
		}

		public virtual void Visit(Statement s)
		{ 
			if( s is ClassStatement )
			{
				ClassName = (s as ClassStatement).name;
			}
		}
		public abstract void End();
	}

	class CommandsVisitor: FSMVisitor
	{
		int commandIndex = 0;
		public CommandsVisitor(StreamWriter _stream)
			:base(_stream)
			{}
		public override void Init()
		{
			stream.WriteLine("enum InterfaceCommands");
			stream.WriteLine("{");
		}

		public override bool Valid(Statement s)
		{
			if (s is InterfaceCommandStatement)
			{
				return true;
			}

			return base.Valid(s);
		}

		public override void Visit(Statement s)
		{
			if (s is InterfaceCommandStatement)
			{
				if (ClassName == null)
					throw new MalformedFSMException("Encountered interfacecommand directive before class directive.");

				InterfaceCommandStatement command = s as InterfaceCommandStatement;
				
				stream.WriteLine("\t" + command.name + " = " + commandIndex + ",");
				commandIndex++;
			}
			else
			{
				base.Visit(s);
			}
		}

		public override void End()
		{
			stream.WriteLine("};");
			stream.WriteLine();
			stream.WriteLine();
		}
	}

	abstract class StateVisitor : FSMVisitor
	{
		Stack<string> statenames = new Stack<string>();

		public StateVisitor(StreamWriter stream)
			: base(stream)
		{ 
		
		}
		public string GetState()
		{	
			return statenames.Peek();
		}
		public string GetParent()
		{
			string current = statenames.Pop();
			string parent = statenames.Peek();
			statenames.Push(current);
			return parent;
		}
		public override bool Valid(Statement s)
		{
			if (s is StateStatement || s is GenericPopStatement)
				return true;
			
			return base.Valid(s);
		}

		public override void Visit(Statement s)
		{
			if (s is StateStatement)
			{
				StateStatement state = s as StateStatement;
				statenames.Push(state.name);
			}
			if (s is GenericPopStatement)
			{
				try
				{
					statenames.Pop();
				}
				catch (Exception e)
				{
					throw new MalformedFSMException("Unexpected EOF. Unterminated state declaration?");
				}
			}

			base.Visit(s);
		}
		
	}

	class DeclarationVisitor : StateVisitor
	{
		public DeclarationVisitor(StreamWriter stream)
			: base(stream)
		{ }

		public override void Init()
		{
			stream.WriteLine("\tFSM::StateMachine<MyClass> FSM;");
			stream.WriteLine("\tvoid onEnterFSM();");
			stream.WriteLine("\tvoid onExitFSM();");
			stream.WriteLine();
		}

		public override bool Valid(Statement s)
		{
			if (s is StateStatement || s is TransitionStatement || s is TestStatement)
			{
				return true;
			}

			return base.Valid(s) ;
		}

		public override void Visit(Statement s)
		{
			base.Visit(s);
			if (ClassName == null)			
				throw new MalformedFSMException("No class statement found before state implementation.");
			if (s is StateStatement)
			{ 
				StateStatement state = s as StateStatement;
				//state statements only have enter/exit/update func declarations

				stream.WriteLine("\tFSM::State<" + ClassName + "> " + state.name + ";");
				stream.WriteLine("\tvoid onEnter" + state.name + "();");
				stream.WriteLine("\tvoid onExit" + state.name + "();");
				if (state.HasStatement(typeof(UpdateStatement)))
				{
					stream.WriteLine("\tvoid onUpdate" + state.name + "(float dt);");
				}
				stream.WriteLine();
			}
			if (s is TestStatement)
			{
				TestStatement test = s as TestStatement;
				string state = GetState();
				if(state == null)
					throw new MalformedFSMException("Interface Command found outside of state block");

				string transName = state + "On" + test.name;

				stream.WriteLine("\tFSM::InterfaceCommand<" + ClassName + "> " + transName + ";");

				stream.WriteLine("\tFSM::InterfaceParam::Enum test"+transName+"(FSM::InterfaceParam* param);");
				stream.WriteLine("\tvoid exec"+transName+"(FSM::InterfaceParam* param);");
				stream.WriteLine();
			}
			if( s is TransitionStatement )
			{
				TransitionStatement transition = s as TransitionStatement;
				string state = GetState();
				if(state == null)
					throw new MalformedFSMException("Interface Command found outside of state block");
				if(transition.command != null)
				{
					string transName = state + "To" + transition.targetstate + "On" + transition.command;
					stream.WriteLine("\tFSM::InterfaceTransition<" + ClassName + "> " + transName + ";");
					stream.WriteLine("\tFSM::InterfaceParam::Enum test"+transName+"(FSM::InterfaceParam* param);");
					stream.WriteLine("\tvoid exec" + state + "To" + transition.targetstate + "On" + transition.command + "(FSM::InterfaceParam* param);");
				}
				else
				{
					string transName = state + "To" + transition.targetstate;
					stream.WriteLine("\tFSM::Transition<" + ClassName + "> " + transName + ";");
					stream.WriteLine("\tbool test" + transName + "();");
					stream.WriteLine("\tvoid exec" + transName + "();");
				}
				stream.WriteLine();
			}
			
		}

		public override void End()
		{
			//throw new NotImplementedException();
		}
	}

	class InitializationVisitor : StateVisitor
	{
		public InitializationVisitor(StreamWriter stream)
			: base(stream)
		{ }

		public override void Init()
		{
			stream.WriteLine("\tInitializeFSM()");
			stream.WriteLine("\t{");
		}

		public override bool Valid(Statement s)
		{
			if (s is StateStatement || s is TransitionStatement || s is TestStatement)
			{
				return true;
			}

			return base.Valid(s);
		}

		public override void Visit(Statement s)
		{
			base.Visit(s);

			if (ClassName == null)
				throw new MalformedFSMException("No class statement found before state implementation.");
			if (s is StateStatement)
			{
				StateStatement state = s as StateStatement;

				bool initial = state.HasStatement(typeof(InitialStatement));
				bool update = state.HasStatement(typeof(UpdateStatement));

				string parent = GetParent();
				if (parent == null)
					parent = "FSM";

				if (update)
				{
					stream.WriteLine("FSM_INIT_STATE_UPDATE(" + ClassName + ", " + state.name + ", " + (initial ? "true" : "false") +");");
				}
				else
				{
					stream.WriteLine("FSM_INIT_STATE(" + ClassName + ", " + state.name + ", " + (initial ? "true" : "false") + ");");
				}

				stream.WriteLine(parent + ".addChild(" + state.name + ");");

				stream.WriteLine();
			}
			if (s is TestStatement)
			{
				TestStatement test = s as TestStatement;
				//FSM_INIT_INTERFACECOMMAND(MyClass, TestA, MyNamedCommand);

				stream.WriteLine();
			}
			if (s is TransitionStatement)
			{
				TransitionStatement transition = s as TransitionStatement;

				string state = GetState();
				if (state == null)
					throw new MalformedFSMException("Interface Command found outside of state block");
				//FSM_INIT_TRANSITION(MyClass, SubstateAA, SubstateAB);
				//FSM_INIT_INTERFACETRANSITION(MyClass, SubstateAA, MyNamedCommand, TestB);

				stream.WriteLine();
			}
		}

		public override void End()
		{
			stream.WriteLine("\t}");	
		}
	}


	abstract class Statement
	{
		public FSM owner;

		public virtual void Consume(Queue<string> tokens) { }
		public virtual bool ShouldPush() { return false; }
		public virtual bool ShouldPop() { return false; }
		public virtual List<Statement> Statements() { return null; }
		public bool HasStatement(Type _type)
		{
			List<Statement> list = Statements();
			if (list != null)
			{
				foreach (Statement s in list)
				{
					if (s.GetType().Equals(_type))
						return true;
				}
			}
			return false;
		}

		public void AcceptVisitor(FSMVisitor visitor)
		{
			if (visitor.Valid(this))
			{
				visitor.Visit(this);
			}
			if (Statements() != null)
			{
				foreach (Statement s in Statements())
				{
					s.AcceptVisitor(visitor);
				}
			}
		}

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
		public string targetstate;
		public string command;
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
		
		public void Export(StreamWriter stream)
		{ 
			CommandsVisitor commands = new CommandsVisitor(stream);

			commands.Init();
			lastpopped.AcceptVisitor(commands);
			commands.End();

			DeclarationVisitor declaration = new DeclarationVisitor(stream);

			declaration.Init();
			lastpopped.AcceptVisitor(declaration);
			declaration.End();
		}
	}

}
