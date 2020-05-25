using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class LogicalOperatorTest : CommonQueryTest
    {
        [Fact]
        public void AndSimpleTest()
        {
            var query = "Int32P<0;StringP==a";
            var expected = BuildFunction(query);

            var mock = new MockQuery {Int32P = -1, StringP = "b"};
            Assert.True(!expected(mock));

            mock = new MockQuery {Int32P = -1, StringP = "a"};
            Assert.True(expected(mock));

            mock = new MockQuery {Int32P = 0, StringP = "a"};
            Assert.True(!expected(mock));

            mock = new MockQuery {Int32P = 0, StringP = "b"};
            Assert.True(!expected(mock));
        }

        [Fact]
        public void OrShouldBeIsAndExpressionWithGroupTest()
        {
            var query = "(Int32P<0,StringP==a);(Int16NullP=is-null=true)";
            var expected = BuildFunction(query);

            var mock = new MockQuery {Int32P = -1, StringP = "a"};
            Assert.True(expected(mock));

            mock = new MockQuery {Int32P = 12, StringP = "a"};
            Assert.True(expected(mock));

            mock = new MockQuery {Int32P = 12, StringP = "a", Int16NullP = 6};
            Assert.True(!expected(mock));

            mock = new MockQuery {Int32P = -1, StringP = "a", Int16NullP = 6};
            Assert.True(!expected(mock));
        }

        [Fact]
        public void OrShouldBeOrExpressionWithGroupTest()
        {
            var query = "(Int32P<0,StringP==a),(Int16NullP=is-null=true)";
            var expected = BuildFunction(query);

            var mock = new MockQuery {Int32P = -1, StringP = "a"};
            Assert.True(expected(mock));

            mock = new MockQuery {Int32P = -1, StringP = "a"};
            Assert.True(expected(mock));

            mock = new MockQuery {Int32P = 1, StringP = "a", Int16NullP = 6};
            Assert.True(expected(mock));

            mock = new MockQuery {Int32P = 1, StringP = "b", Int16NullP = 6};
            Assert.True(!expected(mock));

            mock = new MockQuery {Int32P = 1, StringP = "b", Int16NullP = null};
            Assert.True(expected(mock));
        }

        [Fact]
        public void ShouldBeOrExpression()
        {
            var query = "Int32P<0,StringP==a";
            var expected = BuildFunction(query);

            var mock = new MockQuery {Int32P = -1, StringP = "b"};
            Assert.True(expected(mock));

            mock = new MockQuery {Int32P = -1, StringP = "a"};
            Assert.True(expected(mock));

            mock = new MockQuery {Int32P = 0, StringP = "a"};
            Assert.True(expected(mock));

            mock = new MockQuery {Int32P = 0, StringP = "b"};
            Assert.True(!expected(mock));
        }
    }
}
