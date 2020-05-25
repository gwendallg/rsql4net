using System;
using FluentAssertions;
using RSql4Net.Configurations;
using RSql4Net.Models.Paging;
using Xunit;

namespace RSql4Net.Tests.Models.Paging
{
  
    public class PageableModelBinderProviderTest
    {
        [Fact]
        public void ShouldBeThrowArgumentNullException()
        {
            this.Invoking((a) => new RSqlPageableModelBinderProvider(null))
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShouldBetNotPageableModelBinder()
        {
            var modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(string));
            var pageableModelBinderProvider = new RSqlPageableModelBinderProvider(new Settings());
            var expected = pageableModelBinderProvider.GetBinder(modelBinderProviderContextMock);
            expected
                .Should()
                .BeNull();
        }

        [Fact]
        public void ShouldBeWithClassPageableModelBinder()
        {
            var modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(RSqlPageable<string>));
            var pageableModelBinderProvider = new RSqlPageableModelBinderProvider(new Settings());
            var expected = pageableModelBinderProvider.GetBinder(modelBinderProviderContextMock);
            expected
                .Should()
                .BeOfType<RSqlPageableModelBinder<string>>();
        }

        [Fact]
        public void ShouldBeWithIntefacePageableModelBinder()
        {
            var modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(IRSqlPageable<string>));
            var pageableModelBinderProvider = new RSqlPageableModelBinderProvider(new Settings());
            var expected = pageableModelBinderProvider.GetBinder(modelBinderProviderContextMock);
            expected
                .Should()
                .BeOfType<RSqlPageableModelBinder<string>>();
        }
    }
}
