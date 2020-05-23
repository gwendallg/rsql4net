using static RSql4Net.Models.Queries.QueryParser;

namespace RSql4Net.Tests.Models.Queries
{
    public class MockComparisonContext : ComparisonContext
    {
        private readonly ArgumentsContext _argumentsContext;
        private readonly SelectorContext _selectorContext;

        public MockComparisonContext(SelectorContext selectorContext, ArgumentsContext argumentsContext) : base(null, 0)
        {
            _selectorContext = selectorContext;
            _argumentsContext = argumentsContext;
        }

        public new SelectorContext selector()
        {
            return _selectorContext;
        }

        public new ArgumentsContext arguments()
        {
            return _argumentsContext;
        }
    }
}
