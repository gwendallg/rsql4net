using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using FluentAssertions;
using Moq;
using RSql4Net.Models.Paging;
using RSql4Net.Models;
using Xunit;

namespace RSql4Net.Tests.Models.Paging
{
    public class PageTest
    {
        [Fact]
        public void ShouldBeWithEmptyPage()
        {
            var expected = new RSqlPage<object>();
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

        [Fact]
        public void ShouldBeToValid()
        {

            var mockPageable = new Mock<IRSqlPageable<Customer>>();
            mockPageable.Setup(x => x.PageNumber()).Returns(10);
            mockPageable.Setup(x => x.PageSize()).Returns(2);
            mockPageable.Setup(x => x.Sort()).Returns((RSqlSort<Customer>)null);

            var fake = new Faker<Customer>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .Generate(100)
                .AsQueryable()
                .Page(mockPageable.Object);

            var expected = fake.As(c => c.Name);
            expected
                .Should()
                .NotBeNull();

        }

        [Fact]
        public void ShouldBeToThrowArgumentNullExceptionWhenSelectorNull()
        {
            var mockPageable = new Mock<IRSqlPageable<Customer>>();
            mockPageable.Setup(x => x.PageNumber()).Returns(10);
            mockPageable.Setup(x => x.PageSize()).Returns(2);
            mockPageable.Setup(x => x.Sort()).Returns((RSqlSort<Customer>)null);

            var fake = new Faker<Customer>()
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .Generate(100)
                .AsQueryable()
                .Page(mockPageable.Object);
            this.Invoking(f => { fake.As((Func<Customer, string>)null); })
                .Should()
                .Throw<ArgumentNullException>();
        }
    }
}
