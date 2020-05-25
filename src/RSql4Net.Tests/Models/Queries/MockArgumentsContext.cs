using static RSql4Net.Models.Queries.RSqlQueryParser;

namespace RSql4Net.Tests.Models.Queries
{
    public class MockArgumentsContext : ArgumentsContext
    {
        private readonly string _text;

        public MockArgumentsContext(string text) : base(null, -1)
        {
            _text = text;
        }

        public new string GetText()
        {
            return _text;
        }
    }
}
