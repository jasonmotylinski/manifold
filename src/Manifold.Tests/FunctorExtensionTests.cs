﻿using System.Linq;
using NUnit.Framework;

namespace Manifold.Tests
{
    [TestFixture]
    public class FunctorExtensionTests
    {
        [Test]
        public void verify_unary_functor_behavior()
        {
            var func = new Pipe<int, string>(a => a.ToString());

            var output = func.fmap(new[] {1, 2}).ToList();

            Assert.That(output.Count(), Is.EqualTo(2));
            Assert.That(output[0], Is.EqualTo("1"));
            Assert.That(output[1], Is.EqualTo("2"));

        }

        [Test]
        public void verify_named_unary_functor_behavior()
        {
            var func = new Pipe<string, int, string>((n,a) => a.ToString());

            var output = func.fmap("foo", new[] { 1, 2 }).ToList();

            Assert.That(output.Count(), Is.EqualTo(2));
            Assert.That(output[0], Is.EqualTo("1"));
            Assert.That(output[1], Is.EqualTo("2"));

        }


        
    }
}
