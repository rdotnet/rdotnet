using System;
using System.Collections.Generic;
using System.Linq;

namespace RDotNet.Client
{
    public abstract class Function : SymbolicExpression, IFunction
    {
        private readonly List<string> _names = new List<string>();
        private readonly List<SymbolicExpression> _values = new List<SymbolicExpression>(); 

        protected Function(IRSafeHandle handle)
            : base(handle)
        { }

        public SymbolicExpression Invoke(IList<Expression> args)
        {
            Expression last = null;
            foreach (var expression in args.Reverse())
            {
                last = Cons(expression);
            }
            var call = LCons(last);

            SymbolicExpression evaluated;
            var result = call.TryEvaluate(GetGlobalEnvironment(), out evaluated);
            if (!result) call.ThrowWithLastError();

            return evaluated;
        }

        public void AddArgument(string name, SymbolicExpression value)
        {
            _names.Add(name);
            _values.Add(value);
        }

        public void AddArgumentRange(IList<string> names, IList<SymbolicExpression> values)
        {
            if (names.Count != values.Count) throw new InvalidOperationException();

            _names.AddRange(names);
            _values.AddRange(values);
        }

        public void ClearArguments()
        {
            _names.Clear();
            _values.Clear();
        }

        public SymbolicExpression Invoke()
        {
            var names = new CharacterVector(_names, Handle.API);
            var arguments = new GenericVector(_values, Handle.API);
            var namesSymbol = GetNamesSymbol();
            arguments.SetAttribute(namesSymbol, names);

            var call = LCons(arguments.ToPairList());
            
            SymbolicExpression evaluated;
            var result = call.TryEvaluate(GetGlobalEnvironment(), out evaluated);
            if (!result) call.ThrowWithLastError();

            return evaluated;
        }
    }
}
