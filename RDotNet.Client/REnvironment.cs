using System;
using System.Collections.Generic;

namespace RDotNet.Client
{
    public class REnvironment : SymbolicExpression
    {
        public REnvironment(IRSafeHandle handle)
            : base(handle)
        { }

        public REnvironment(REnvironment parent)
            : base(parent.Handle)
        {
            SetAsNewEnvironment(parent);
        }

        public REnvironment GetParent()
        {
            var parent = GetParentEnvironment();
            return parent;
        }

        public Symbol GetSymbol(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException();

            var installedName = Install(name);
            var value = FindVar(installedName);

            if (value.IsPromise())
            {
                value = Evaluate(value.ToExpression());
            }

            return value.ToSymbol();
        }

        public void SetSymbol(string name, SymbolicExpression expression)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException();

            var installedName = Install(name);
            DefineVar(installedName, expression);
        }

        public IEnumerable<string> GetSymbolNames(bool all = false)
        {
            var symbolNames = LsInternal(all);
            return symbolNames;
        }

        public bool TryEvaluate(Expression expression, out SymbolicExpression evaluated)
        {
            return EnvironmentTryEvaluate(expression, out evaluated);
        }

        public SymbolicExpression Evaluate(Expression expression)
        {
            return EnvironmentEvaluate(expression);
        }
    }
}
