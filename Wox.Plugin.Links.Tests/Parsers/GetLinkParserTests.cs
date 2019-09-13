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
            _saveParser = new GetLinkParser(_storage, _linkProcess, Substitute.For<IClipboardService>());
            _storage.GetLinks().Returns(_links);
        }

        private readonly IStorage _storage;
        private readonly GetLinkParser _saveParser;

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

        [Fact]
        public void InputIsWordWithCapitalCase_IgnoreMatchesOfLowerCase() {
            _saveParser.TryParse("GC".AsQuery(), out var results)
                .Should().BeFalse();
            results.Should().BeEmpty();
        }

        [Fact]
        public void InputIsWordWithCapitalCase_MatchByNameSplitByCapitalCases() {
            _saveParser.TryParse("GA".AsQuery(), out var results)
                .Should().BeTrue();
            results.Should().HaveCount(1);

            results[0].Title.Should().StartWith("[GoogleAction]");
        }

        [Fact]
        public void MatchByName_ReturnFullUrl() {
            _saveParser.TryParse("cut".AsQuery(), out var results).Should()
                .BeTrue();
            results.Should().HaveCount(2);

            results[0].Title.Should().Be("[Shortcut] Description 1");
            results[0].SubTitle.Should().Be("https://some.com/do");

            results[1].Title.Should().Be("[AustriaCut] Description 3");
            results[1].SubTitle.Should().Be("https://austria.com/");
        }

        [Fact]
        public void MatchByNameWithReplacement_ReturnFullUrl() {
            _storage.GetLinks().Returns(new[] {
                new Link {
                    Shortcut = "Shortcut",
                    Path = "https://jira.com/STF-@@",
                    Description = "Open IDPF-@@ ticket"
                }
            });

            _saveParser.TryParse("cut 8700".AsQuery(), out var results)
                .Should().BeTrue();


            results[0].Title.Should().Be("[Shortcut] Open IDPF-8700 ticket");
            results[0].SubTitle.Should().Be("https://jira.com/STF-8700");
            results[0].Action(new ActionContext()).Should().BeTrue();
        }

        [Fact]
        public void MatchByNameWithReplacement_ReturnFullUrlFalse() {
            _storage.GetLinks().Returns(new[] {
                new Link {
                    Shortcut = "Shortcut",
                    Path = "https://jira.com/STF-@@",
                    Description = "Open IDPF-@@ ticket"
                }
            });

            _saveParser.TryParse("cut".AsQuery(), out var results)
                .Should().BeTrue();


            results[0].Title.Should().Be("[Shortcut] Open IDPF-{Parameter is missing} ticket");
            results[0].SubTitle.Should().Be("https://jira.com/STF-{Parameter is missing}");
            results[0].Action(new ActionContext()).Should().BeFalse();
        }
    }
}