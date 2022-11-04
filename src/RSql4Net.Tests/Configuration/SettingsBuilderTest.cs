using System;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using RSql4Net.Configurations;
using RSql4Net.Configurations.Exceptions;
using RSql4Net.Models.Paging.Exceptions;
using RSql4Net.Models.Queries;
using Xunit;

namespace RSql4Net.Tests.Configuration
{
    public class SettingsBuilderTest
    {
        [Fact]
        public void ShouldBeAlreadyFieldNameUsedException()
        {
            var builder = new SettingsBuilder();
            builder
                .PageNumberFieldName("A")
                .PageSizeFieldName("A");

            builder.Invoking(b => b.Build())
                .Should()
                .Throw<AlreadyFieldNameUsedException>();
        }

        [Fact]
        public void ShouldBeCustomValidBuilder()
        {
            var builder = new SettingsBuilder();
            builder
                .PageNumberFieldName("a")
                .PageSize(2)
                .PageSizeFieldName("b")
                .QueryFieldName("c")
                .SortFieldName("d");

            var expected = builder.Build();

            expected
                .PageNumberField.Should().Be("a");
            expected
                .PageSize.Should().Be(2);
            expected
                .PageSizeField.Should().Be("b");
            expected
                .QueryField.Should().Be("c");
            expected
                .SortField.Should().Be("d");
        }

        [Fact]
        public void ShouldBeDefaultValidBuilder()
        {
            var expected = new SettingsBuilder().Build();

            expected
                .PageNumberField.Should().Be(Settings.CDefaultPageNumberFieldName);
            expected
                .PageSize.Should().Be(Settings.CDefaultPageSize);
            expected
                .PageSizeField.Should().Be(Settings.CDefaultPageSizeFieldName);
            expected
                .QueryField.Should().Be(Settings.CDefaultQueryFieldName);
            expected
                .SortField.Should().Be(Settings.CDefaultSortFieldName);
        }

        [Fact]
        public void ShouldBeInvalidFormatFieldNameException()
        {
            var builder = new SettingsBuilder();
            builder
                .PageNumberFieldName("A+");

            builder.Invoking(b => b.Build())
                .Should()
                .Throw<InvalidFormatFieldNameException>();
        }

        [Fact]
        public void ShouldBeOutOfRangePageSizeException()
        {
            var builder = new SettingsBuilder();
            builder
                .PageSize(-2);

            builder.Invoking(b => b.Build())
                .Should()
                .Throw<OutOfRangePageSizeException>();
        }

        [Fact]
        public void ShouldBeWithQueryCache()
        {
            var action = new MemoryRSqlQueryCache();
            var builder =
                new SettingsBuilder()
                    .QueryCache(action);
            var expected = builder.Build();
            expected.QueryCache.Should()
                .Be(action);
        }
    }

}
