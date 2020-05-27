using System.Collections.Generic;
using FluentAssertions;
using Moq;
using RSql4Net.Models.Paging;
using Xunit;

namespace RSql4Net.Tests.Models.Paging
{
    public class PageTest
    {
        [Fact]
        public void ShouldBeWithEmptyPage()
        {
            var expected = new RSqlPage<object>(null);
            expected
                .HasContent.Should().BeFalse();
            expected
                .HasContent.Should().BeFalse();
            expected
                .HasPrevious.Should().BeFalse();
            expected
                .HasNext.Should().BeFalse();
            expected
                .Number.Should().Be(0);
            expected
                .NumberOfElements.Should().Be(0);
            expected
                .TotalElements.Should().Be(0);
            expected
                .TotalPages.Should().Be(0);
            expected
                .Content.Should().NotBeNull();
            expected
                .Content.Should().BeEmpty();
        }


        [Fact]
        public void ShouldBeWithHasContentAndHasPreviousAndHasNext()
        {
            var mockPageable = new Mock<IRSqlPageable<object>>();
            mockPageable.Setup(x => x.PageNumber()).Returns(50);
            mockPageable.Setup(x => x.PageSize()).Returns(2);

            var expected = new RSqlPage<object>(new List<object>(new object[] {"1", "2"}), mockPageable.Object, 200);

            expected
                .HasContent.Should().BeTrue();
            expected
                .Number.Should().Be(50);
            expected
                .HasPrevious.Should().BeTrue();
            expected
                .HasNext.Should().BeTrue();
            expected
                .TotalElements.Should().Be(200);
            expected
                .NumberOfElements.Should().Be(2);
            expected
                .TotalPages.Should().Be(100);
        }

        [Fact]
        public void ShouldBeWithHasContentAndIsFirstPage()
        {
            var mockPageable = new Mock<IRSqlPageable<object>>();
            mockPageable.Setup(x => x.PageNumber()).Returns(0);
            mockPageable.Setup(x => x.PageSize()).Returns(2);
            var expected = new RSqlPage<object>(new List<object>(new object[] {"1", "2"}), mockPageable.Object, 200);

            expected
                .HasContent.Should().BeTrue();
            expected
                .Number.Should().Be(0);
            expected
                .HasPrevious.Should().BeFalse();
            expected
                .HasNext.Should().BeTrue();
            expected
                .TotalElements.Should().Be(200);
            expected
                .NumberOfElements.Should().Be(2);
            expected
                .TotalPages.Should().Be(100);
        }

        [Fact]
        public void ShouldBeWithHasContentAndIsLastPage()
        {
            var mockPageable = new Mock<IRSqlPageable<object>>();
            mockPageable.Setup(x => x.PageNumber()).Returns(199);
            mockPageable.Setup(x => x.PageSize()).Returns(2);
            var expected = new RSqlPage<object>(new List<object>(new object[] {"1", "2"}), mockPageable.Object, 200);

            expected
                .HasContent.Should().BeTrue();
            expected
                .Number.Should().Be(199);
            expected
                .HasPrevious.Should().BeTrue();
            expected
                .HasNext.Should().BeFalse();
            expected
                .TotalElements.Should().Be(200);
            expected
                .NumberOfElements.Should().Be(2);
            expected
                .TotalPages.Should().Be(100);
        }
    }
}
