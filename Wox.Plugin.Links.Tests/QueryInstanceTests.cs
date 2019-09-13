using FluentAssertions;
using Wox.Plugin.Links;
using Xunit;

namespace Wox.Links.Tests {
    public class QueryInstanceTests {
        [Fact]
        public void ArgumentsParser() {
            var query = "link  gl https://google.com  ";
            var instance = new QueryInstance(query);
            instance.FirstSearch.Should().Be("link");
            instance.SecondToEndSearch.Should().Be("gl https://google.com");
        }
    }
}