using FluentAssertions;
using RSql4Net.Configurations;
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
            var pageableModelBinderProvider = new PageableModelBinderProvider(new Settings());
            var expected = pageableModelBinderProvider.GetBinder(modelBinderProviderContextMock);
            expected
                .Should()
                .BeNull();
        }

        [Fact]
        public void ShouldBeWithClassPageableModelBinder()
        {
            var modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(Pageable<string>));
            var pageableModelBinderProvider = new PageableModelBinderProvider(new Settings());
            var expected = pageableModelBinderProvider.GetBinder(modelBinderProviderContextMock);
            expected
                .Should()
                .BeOfType<PageableModelBinder<string>>();
        }

        [Fact]
        public void ShouldBeWithIntefacePageableModelBinder()
        {
            var modelBinderProviderContextMock = new MockModelBinderProviderContext(typeof(IPageable<string>));
            var pageableModelBinderProvider = new PageableModelBinderProvider(new Settings());
            var expected = pageableModelBinderProvider.GetBinder(modelBinderProviderContextMock);
            expected
                .Should()
                .BeOfType<PageableModelBinder<string>>();
        }
    }
}
