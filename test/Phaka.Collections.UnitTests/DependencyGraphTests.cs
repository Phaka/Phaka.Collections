// Copyright (c) Werner Strydom. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Phaka.Collections.UnitTests
{
    using System.Linq;
    using Xunit;

    public class DependencyGraphTests
    {
        [Fact]
        public void AddDependency()
        {
            // Arrange
            var target = new DependencyGraph<string>();

            // Act
            target.AddDependency("t1", "t2");

            // Assert
            Assert.Equal(2, target.Count);
            Assert.Contains("t2", target.GetDirectDependents("t1").ToArray());
            Assert.Contains("t1", target.GetAntecedents("t2").ToArray());
            Assert.Contains("t1", target.GetRootNodes().ToArray());
            Assert.Contains("t2", target.GetLeafNodes().ToArray());
        }

        [Fact]
        public void GetSubgraph_Simple()
        {
            // Arrange
            var target = new DependencyGraph<string>();
            target.AddDependency("t1", "t2");
            target.AddDependency("t2", "t3");
            target.AddDependency("t3", "t4");
            target.AddDependency("t4", "t5");

            // Act
            var expected = target.GetSubgraph(s => s == "t3");

            // Assert
            var nodes = expected.GetNodes().ToList();
            Assert.Equal(3, expected.Count);
            Assert.Contains("t1", nodes);
            Assert.Contains("t2", nodes);
            Assert.Contains("t3", nodes);
            Assert.False(nodes.Contains("t4"));
            Assert.False(nodes.Contains("t5"));
        }

        [Fact]
        public void GetSubgraph_TestCase2()
        {
            // Arrange
            var target = new DependencyGraph<string>();
            target.AddDependency("t1", "t2");
            target.AddDependency("t2", "t3");
            target.AddDependency("t1", "t4");
            target.AddDependency("t4", "t5");
            target.AddDependency("t3", "t5");

            // Act
            var expected = target.GetSubgraph(s => s == "t4");

            // Assert
            var nodes = expected.GetNodes().ToList();
            Assert.Equal(2, expected.Count);
            Assert.Contains("t1", nodes);
            Assert.Contains("t4", nodes);
            Assert.False(nodes.Contains("t2"));
            Assert.False(nodes.Contains("t3"));
            Assert.False(nodes.Contains("t5"));
        }


        [Fact]
        public void GetSubgraph_TestCase3()
        {
            // Arrange
            var target = new DependencyGraph<string>();
            target.AddDependency("t1", "t2");
            target.AddDependency("t2", "t3");
            target.AddDependency("t1", "t4");
            target.AddDependency("t4", "t5");
            target.AddDependency("t3", "t5");

            target.AddDependency("s1", "s2");
            target.AddDependency("s2", "s3");
            target.AddDependency("s1", "s4");
            target.AddDependency("s4", "s5");
            target.AddDependency("s3", "s5");


            // Act
            var expected = target.GetSubgraph(s => s == "t4" || s == "s4");

            // Assert
            var nodes = expected.GetNodes().ToList();
            Assert.Equal(4, expected.Count);
            Assert.Contains("t1", nodes);
            Assert.Contains("t4", nodes);
            Assert.False(nodes.Contains("t2"));
            Assert.False(nodes.Contains("t3"));
            Assert.False(nodes.Contains("t5"));

            Assert.Contains("s1", nodes);
            Assert.Contains("s4", nodes);
            Assert.False(nodes.Contains("s2"));
            Assert.False(nodes.Contains("s3"));
            Assert.False(nodes.Contains("s5"));
        }
    }
}