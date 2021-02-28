using System;
using System.Collections.Generic;
using Xunit;
using RandomNeko.NekosMoe.Model;

namespace RandomNeko.Test
{
    public class ImageTest
    {
        [Fact]
        public void TruncateTagsTest() {
            var image = new Image() {Tags = new List<string>() {
                "abcd", "efgh"
            }};
            Assert.Equal("a...", image.TagsTruncated(4));
        }
    }
}
