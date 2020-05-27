using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using RSql4Net.Models.Paging;
using Xunit;

namespace RSql4Net.Tests.Models.Paging
{
  
    public class PageableModelBinderProviderTest
    {
        [Fact]
        public void ShouldBetNotPageableModelBinder()
        {
            var modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(string));
            var pageableModelBinderProvider = new RSqlPageableModelBinderProvider();
            var expected = pageableModelBinderProvider.GetBinder(modelBinderProviderContextMock);
            expected
                .Should()
                .BeNull();
        }

        [Fact]
        public void ShouldBeWithClassPageableModelBinder()
        {
            var modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(RSqlPageable<string>));
            var pageableModelBinderProvider = new RSqlPageableModelBinderProvider();
            var expected = pageableModelBinderProvider.GetBinder(modelBinderProviderContextMock);
            expected
                .Should()
                .BeOfType<BinderTypeModelBinder>();
        }

        [Fact]
        public void ShouldBeWithInterfacePageableModelBinder()
        {
            var modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(IRSqlPageable<string>));
            var pageableModelBinderProvider = new RSqlPageableModelBinderProvider();
            var expected = pageableModelBinderProvider.GetBinder(modelBinderProviderContextMock);
            expected
                .Should()
                .BeOfType<BinderTypeModelBinder>();
        }
    }
}
