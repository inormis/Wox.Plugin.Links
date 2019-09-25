using System.Linq;
using FluentAssertions;
using NSubstitute;
using Wox.Plugin;
using Wox.Plugin.Links;
using Wox.Plugin.Links.Parsers;
using Wox.Plugin.Links.Services;
using Xunit;

namespace Wox.Links.Tests.Parsers {
    public class GetLinkParserTests {
        public GetLinkParserTests() {
            _storage = Substitute.For<IStorage>();
            _linkProcess = Substitute.For<ILinkProcessService>();
            _linkProcess.Open(Arg.Any<string>()).Returns(true);
            _parser = new GetLinkParser(_storage, _linkProcess, Substitute.For<IClipboardService>());
            _storage.GetLinks().Returns(_links);
        }

        private readonly IStorage _storage;
        private readonly GetLinkParser _parser;

        private readonly Link[] _links = {
            new Link {
                Shortcut = "Shortcut",
                Path = "https://some.com/do",
                Description = "Description 1"
            },
            new Link {
                Shortcut = "GoogleAction",
                Path = "https://google.com/action",
                Description = "Description 2"
            },
            new Link {
                Shortcut = "AustriaCut",
                Path = "https://austria.com/",
                Description = "Description 3"
            },
            new Link {
                Shortcut = "Jira",
                Path = "https://some.com/idpf-@@",
                Description = "Description 4"
            }
        };

        private readonly ILinkProcessService _linkProcess;
        private readonly ActionContext _actionContext = new ActionContext {SpecialKeyState = new SpecialKeyState()};

        [Theory(Skip = "https://github.com/Wox-launcher/Wox/issues/1857")]
        [InlineData("gooo")]
        [InlineData("gan")]
        [InlineData("golea")]
        [InlineData("actn")]
        public void MatchShortCutByConsecutiveMatches(string query) {
            _parser.TryParse(query.AsQuery(), out var results)
                .Should().BeTrue();
            results.Should().HaveCount(1);
            results.Single().Title.Should().Contain("GoogleAction");
        }

        [Fact]
        public void MatchByName_ReturnFullUrl() {
            _parser.TryParse("cut".AsQuery(), out var results).Should()
                .BeTrue();
            results.Should().HaveCount(2);

            results[0].Title.Should().Be("[Shortcut] Description 1");
            results[0].SubTitle.Should().Be("https://some.com/do");

            results[1].Title.Should().Be("[AustriaCut] Description 3");
            results[1].SubTitle.Should().Be("https://austria.com/");
        }

        [Fact]
        public void MatchesShortCutAndParameterProvided_ReturnTrue() {
            _storage.GetLinks().Returns(new[] {
                new Link {
                    Shortcut = "Shortcut",
                    Path = "https://jira.com/STF-@@",
                    Description = "Open IDPF-@@ ticket"
                }
            });

            _parser.TryParse("cut 8700".AsQuery(), out var results)
                .Should().BeTrue();


            results[0].Title.Should().Be("[Shortcut] Open IDPF-8700 ticket");
            results[0].SubTitle.Should().Be("https://jira.com/STF-8700");
            results[0].Action(_actionContext).Should().BeTrue();
            _linkProcess.Received(1).Open("https://jira.com/STF-8700");
        }

        [Fact]
        public void MatchesShortCutAndParameterNotProvided_ReturnFalse() {
            _storage.GetLinks().Returns(new[] {
                new Link {
                    Shortcut = "Shortcut",
                    Path = "https://jira.com/STF-@@",
                    Description = "Open IDPF-@@ ticket"
                }
            });

            _parser.TryParse("cut".AsQuery(), out var results)
                .Should().BeTrue();


            results[0].Title.Should().Be("[Shortcut] Open IDPF-{Parameter is missing} ticket");
            results[0].SubTitle.Should().Be("https://jira.com/STF-{Parameter is missing}");
            results[0].Action(_actionContext).Should().BeFalse();
            _linkProcess.DidNotReceive().Open(Arg.Any<string>());
        }

        [Fact(Skip = "https://github.com/Wox-launcher/Wox/issues/1857")]
        public void MatchSeveralLinks_SortByMatching() {
            var links = new[] {"Stackoverflows", "so", "sos", "1so"}
                .Select(x => new Link {Shortcut = x, Path = "https://so.com"}).ToArray();
            _storage.GetLinks().Returns(links);

            var parsed = _parser.TryParse("so".AsQuery(), out var results);

            parsed.Should().BeTrue();
            results.Should().HaveCount(4);

            results[0].Title.Should().StartWith("[so]");
            results[1].Title.Should().StartWith("[sos]");
            results[2].Title.Should().StartWith("[1so]");
            results[3].Title.Should().StartWith("[Stackoverflows]");
        }
        
        [Theory(Skip = "https://github.com/Wox-launcher/Wox/issues/1857")]
        [InlineData("so", 0)]
        [InlineData("sos", 0)]
        [InlineData("1so", 1)]
        [InlineData("s1o", 1)]
        [InlineData("_so", 1)]
        [InlineData("stackOVerflow", 4)]
        public void LinkMatches(string shortcut, int expectedIndex) {
            new Link {
                Shortcut = shortcut
            }.Matches("so").Index.Should().Be(expectedIndex);
        }
    }
}