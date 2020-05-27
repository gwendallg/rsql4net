using Xunit;

namespace RSql4Net.Tests.Models.Queries
{
    public class LogicalOperatorTest
    {
        [Fact]
        public void AndSimpleTest()
        {
            const string query = "int32P<0;stringP==a";
            var expected = Helper.Function<MockQuery>(query);

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
            const string query = "(int32P<0,stringP==a);(int16NullP=is-null=true)";
            var expected = Helper.Function<MockQuery>(query);

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
            const string query = "(int32P<0,stringP==a),(int16NullP=is-null=true)";
            var expected = Helper.Function<MockQuery>(query);

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
            var query = "int32P<0,stringP==a";
            var expected = Helper.Function<MockQuery>(query);

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
