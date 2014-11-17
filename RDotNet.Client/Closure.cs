namespace RDotNet.Client
{
    public class Closure : Function, IClosure
    {
        public Closure(IRSafeHandle handle)
            : base(handle)
        { }

        public PairList GetArguments()
        {
            var arguments = GetClosureArguments();
            return arguments;
        }

        public Language GetBody()
        {
            var body = GetClosureBody();
            return body;
        }

        public REnvironment GetEnvironment()
        {
            var environment = GetClosureEnvironment();
            return environment;
        }
    }
}
