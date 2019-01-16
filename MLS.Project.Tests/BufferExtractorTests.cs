﻿using System.Linq;
using FluentAssertions;
using MLS.Protocol.Execution;
using MLS.Project.Extensions;
using Xunit;

namespace MLS.Project.Tests
{
    public class BufferExtractorTests
    {
        [Fact]
        public void it_generates_a_buffer_only_workspace_when_no_regions_are_defined()
        {
            var noRegionFiles = new[]
            {
                new Workspace.File("buffer1.cs", Sources.CodeWithNoRegions),
                new Workspace.File("buffer2.cs", Sources.CodeWithNoRegions),
            };

            var tansformer = new BufferFromRegionExtractor();
            var result = tansformer.Extract(noRegionFiles, workspaceType: "console");
            result.Should().NotBeNull();
            result.Buffers.Should().Contain(found => found.Id == "buffer1.cs" && found.Content == Sources.CodeWithNoRegions);
            result.Buffers.Should().Contain(found => found.Id == "buffer2.cs" && found.Content == Sources.CodeWithNoRegions);
            result.Files.Should().BeNullOrEmpty();
        }

        [Fact]
        public void it_generates_a_file_and_buffers_workspace()
        {
            var noRegionFiles = new[]
            {
                new Workspace.File("buffer1.cs", Sources.CodeWithTwoRegions),
                new Workspace.File("buffer2.cs", Sources.CodeWithNoRegions),
            };

            var tansformer = new BufferFromRegionExtractor();
            var result = tansformer.Extract(noRegionFiles, workspaceType: "console");
            result.Should().NotBeNull();

            result.Buffers.Should().Contain(found => found.Id == "buffer1.cs@objetConstruction" && found.Content == @"var simpleObject = new JObject
            {
                {""property"", 4}
            };");
            result.Buffers.Should().Contain(found => found.Id == "buffer1.cs@workspaceIdentifier" && found.Content == @"Console.WriteLine(""jsonDotNet workspace"");");
            result.Buffers.Should().Contain(found => found.Id == "buffer2.cs" && found.Content == Sources.CodeWithNoRegions);
            result.Files.Should().NotBeNullOrEmpty();
            result.Files.Should().Contain(found => found.Name == "buffer1.cs" && found.Text == Sources.CodeWithTwoRegions);
        }

        [Fact]
        public void it_generates_content_with_correct_indentation()
        {
            const string expectedCode = "// Instant represents time from epoch\nInstant now = SystemClock.Instance.GetCurrentInstant();\nConsole.WriteLine($\"now: {now}\");\n\n// Convert an instant to a ZonedDateTime\nZonedDateTime nowInIsoUtc = now.InUtc();\nConsole.WriteLine($\"nowInIsoUtc: {nowInIsoUtc}\");\n\n// Create a duration\nDuration duration = Duration.FromMinutes(3);\nConsole.WriteLine($\"duration: {duration}\");\n\n// Add it to our ZonedDateTime\nZonedDateTime thenInIsoUtc = nowInIsoUtc + duration;\nConsole.WriteLine($\"thenInIsoUtc: {thenInIsoUtc}\");\n\n// Time zone support (multiple providers)\nvar london = DateTimeZoneProviders.Tzdb[\"Europe/London\"];\nConsole.WriteLine($\"london: {london}\");\n\n// Time zone conversions\nvar localDate = new LocalDateTime(2012, 3, 27, 0, 45, 00);\nvar before = london.AtStrictly(localDate);\nConsole.WriteLine($\"before: {before}\");";

            var files = new[]
            {
                new Workspace.File("buffer1.cs", Sources.GistWithRegion),
            };

            var tansformer = new BufferFromRegionExtractor();
            var result = tansformer.Extract(files, workspaceType: "console");
            result.Should().NotBeNull();

            result.Buffers.Should().Contain(found => found.Id == "buffer1.cs@fragment");

            result.Buffers.First().Content.Replace("\r\n", "\n").Should().Be(expectedCode);
        }
    }
}