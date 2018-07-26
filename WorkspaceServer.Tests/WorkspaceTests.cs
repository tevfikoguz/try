using System;
using FluentAssertions;
using WorkspaceServer.Models.Execution;
using Xunit;

namespace WorkspaceServer.Tests
{
    public class WorkspaceTests
    {
        [Fact]
        public void When_there_is_only_one_buffer_and_it_has_no_id_then_GetAbsolutePosition_returns_its_position
            ()
        {
            var workspace = new Workspace(
                buffers:
                new[]
                {
                    new Workspace.Buffer("", "content", 123)
                });

            workspace.GetAbsolutePositionForGetBufferWithSpecifiedIdOrSingleBufferIfThereIsOnlyOne().Should().Be(123);
        }

        [Fact]
        public void When_there_is_only_one_buffer_and_it_has_an_id_that_does_not_match_then_GetAbsolutePosition_returns_its_position
            ()
        {
            var workspace = new Workspace(
                buffers:
                new[]
                {
                    new Workspace.Buffer("nonexistent.cs", "content", 123)
                });

            workspace.GetAbsolutePositionForGetBufferWithSpecifiedIdOrSingleBufferIfThereIsOnlyOne().Should().Be(123);
        }

        [Fact]
        public void When_buffer_id_is_empty_string_and_there_is_a_file_with_empty_string_for_id_then_GetFileFromBufferId_returns_it()
        {
            var workspace = new Workspace(
                buffers:
                new[]
                {
                    new Workspace.Buffer("", "content", 123)
                },
                files: new[]
                {
                    new Workspace.File("", "content")
                });

            workspace.GetFileFromBufferId("").Should().NotBeNull();
        }
    }
}