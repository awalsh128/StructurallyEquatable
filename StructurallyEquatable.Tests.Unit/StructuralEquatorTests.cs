using NUnit.Framework;
using System.Collections.Generic;

namespace Awalsh128.Tests.Unit
{
    [TestFixture]
    public class StructuralEquatorTests
    {
        public class EmptyClass
        { }

        public class EmptyClass2
        { }

        public struct EmptyStruct
        { }

        public class PopulatedClass
        {
            public int Integer { get; set; }

            public PopulatedClass OtherPopulatedClass { get; set; }

            public PopulatedClass(int integer, PopulatedClass otherPopulatedClass)
            {
                Integer = integer;
                OtherPopulatedClass = otherPopulatedClass;
            }
        }

        public struct PopulatedStruct
        {
            public int Integer { get; set; }

            public PopulatedClass PopulatedClass { get; set; }

            public PopulatedStruct(int integer, PopulatedClass populatedClass)
            {
                Integer = integer;
                PopulatedClass = populatedClass;
            }
        }

        public static IEnumerable<object[]> IsEqualBaseCasesSource()
        {
            yield return new object[] { 1, 1, true };
            yield return new object[] { 1, 2, false };
            yield return new object[] { null, null, true };
            yield return new object[] { null, 1, false };
            yield return new object[] { 1, null, false };
            yield return new object[] { new EmptyClass(), new EmptyClass(), true };
            yield return new object[] { new EmptyClass(), new EmptyClass2(), false };
            yield return
                new object[]
                {
                    new PopulatedClass(1, new PopulatedClass(2, null)),
                    new PopulatedClass(1, new PopulatedClass(2, new PopulatedClass(2, null))),
                    false
                };
        }

        [Test, TestCaseSource("IsEqualBaseCasesSource")]
        public void IsEqualBaseCases(object a, object b, bool expected)
        {
            Assert.AreEqual(expected, StructuralEquator.Instance.IsEqual(a, b));
        }

        [Test]
        public void IsNotEqualForRecursiveCaseWithDepth2()
        {
            var a = new PopulatedClass(1, new PopulatedClass(2, null));
            var b = new PopulatedClass(1, new PopulatedClass(2, new PopulatedClass(2, null)));
            Assert.AreEqual(false, StructuralEquator.Instance.IsEqual(a, b));
        }

        [Test]
        public void IsEqualForRecursiveCaseWithDepth2()
        {
            var recursive = new PopulatedClass(2, null);
            recursive.OtherPopulatedClass = recursive;
            var a = new PopulatedClass(1, recursive);
            var b = new PopulatedClass(1, recursive);
            Assert.AreEqual(true, StructuralEquator.Instance.IsEqual(a, b));
        }

        [Test]
        public void IsEqualForArray()
        {
            var a = new[] { new PopulatedClass(1, null), new PopulatedClass(2, new PopulatedClass(3, null)) };
            var b = new[] { new PopulatedClass(1, null), new PopulatedClass(2, new PopulatedClass(3, null)) };
            Assert.AreEqual(true, StructuralEquator.Instance.IsEqual(a, b));
        }

        [Test]
        public void IsNotEqualForArrayWithDifferentLengths()
        {
            var a = new[] { new PopulatedClass(1, null), new PopulatedClass(2, new PopulatedClass(3, null)) };
            var b = new[] { new PopulatedClass(1, null), new PopulatedClass(2, new PopulatedClass(3, null)), null };
            Assert.AreEqual(false, StructuralEquator.Instance.IsEqual(a, b));
        }
    }
}
