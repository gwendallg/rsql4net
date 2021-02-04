using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualBasic;
using Xunit;

namespace RSql4Net.Samples.Tests
{
    public class IntegrationTests :IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions {AllowAutoRedirect = true});
        }

        [Fact]
        public async Task ShouldBeEnsureHealthCheckEndpoint()
        {
            var expected = await _client.GetAsync("/health");
            expected
                .StatusCode
                .Should().Be(HttpStatusCode.OK);
            var content = await expected.Content.ReadAsStringAsync();
            var json = (JsonElement) JsonSerializer.Deserialize<object>(content);
            json.GetProperty("status").GetString().Should().Be("Healthy");
        }
        
        [Fact]
        public async Task ShouldBeEnsureMetricsEndpoint()
        {
            var expected = await _client.GetAsync("/customers");
            expected = await _client.GetAsync("/metrics");
            expected
                .StatusCode
                .Should().Be(HttpStatusCode.OK);
            var content = await expected.Content.ReadAsStringAsync();
            content.Should().Contain("rsql4net_sample_customer_get_duration_seconds_sum");
        }
        
        
        [Fact]
        public async Task ShouldBeEnsureCustomerEndpoint()
        {
            var expected = await _client.GetAsync("/customers");
            expected
                .StatusCode
                .Should().Be(HttpStatusCode.PartialContent);
            var content = await expected.Content.ReadAsStringAsync();
            var json = (JsonElement) JsonSerializer.Deserialize<object>(content);
            json.GetProperty("has_content").GetBoolean()
                .Should().BeTrue();
        }
    }
}
