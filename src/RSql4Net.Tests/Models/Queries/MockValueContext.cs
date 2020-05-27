using Antlr4.Runtime;
using RSql4Net.Models.Queries;

namespace RSql4Net.Tests.Models.Queries
{
    public class MockValueContext : RSqlQueryParser.ValueContext
    {
        public MockValueContext(ParserRuleContext parent, int invokingState) : base(parent, invokingState)
        {
        }
    }
}
