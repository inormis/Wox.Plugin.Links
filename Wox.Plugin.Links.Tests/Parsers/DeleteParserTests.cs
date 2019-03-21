using FluentAssertions;
using NSubstitute;
using Wox.Plugin;
using Wox.Plugin.Links;
using Wox.Plugin.Links.Parsers;
using Xunit;

namespace Wox.Links.Tests.Parsers {
    public class DeleteParserTests {
        public DeleteParserTests() {
            _storage = Substitute.For<IStorage>();
            _parser = new DeleteParser(_storage);
        }

        private readonly IStorage _storage;
        private readonly DeleteParser _parser;

        [Theory]
        [InlineData("--delete")]
        [InlineData("-d")]
        public void DeleteWithShortcut_ReturnAllLinks(string key) {
            _storage.GetLinks().Returns(new[] {
                new Link {
                    Shortcut = "Ad1",
                    Path = "https://ad1"
                },
                new Link {
                    Shortcut = "movie",
                    Path = "https://movie"
                },
                new Link {
                    Path = "https://gl",
                    Shortcut = "google"
                }
            });
            _parser.TryParse(key.AsQuery(), out var results).Should().BeTrue();
            results.Should().HaveCount(3);

            results[0].Title.Should().Be("Delete 'Ad1' link");
            results[0].SubTitle.Should().Be("https://ad1");

            results[1].Title.Should().Be("Delete 'google' link");
            results[1].SubTitle.Should().Be("https://gl");

            results[2].Title.Should().Be("Delete 'movie' link");
            results[2].SubTitle.Should().Be("https://movie");

            results[1].Action(new ActionContext());
            _storage.Received(1).Delete("google");
        }

        [Theory]
        [InlineData("--del")]
        [InlineData("-remo")]
        [InlineData("-re")]
        public void NotDeleteKeyWord_ReturnFalse(string key) {
            _storage.GetLinks().Returns(new[] {
                new Link {
                    Shortcut = "Ad1",
                    Path = "https://ad1"
                },
                new Link {
                    Shortcut = "movie",
                    Path = "https://movie"
                },
                new Link {
                    Path = "https://gl",
                    Shortcut = "google"
                }
            });
            _parser.TryParse(key.AsQuery(), out var results).Should().BeFalse();
        }

        [Fact]
        public void DeleteWithShortcut_ReturnLinksMatchingKeyWork() {
            _storage.GetLinks().Returns(new[] {
                new Link {
                    Shortcut = "Ad1",
                    Path = "https://ad1"
                },
                new Link {
                    Shortcut = "movie",
                    Path = "https://movie"
                },
                new Link {
                    Shortcut = "Movart",
                    Path = "https://gl"
                }
            });
            _parser.TryParse("-d mov".AsQuery(), out var results).Should().BeTrue();
            results.Should().HaveCount(2);

            results[1].Title.Should().Be("Delete 'movie' link");
            results[1].SubTitle.Should().Be("https://movie");

            results[0].Title.Should().Be("Delete 'Movart' link");
            results[0].SubTitle.Should().Be("https://gl");

            results[1].Action(new ActionContext());
            _storage.Received(1).Delete("movie");
        }
    }
}