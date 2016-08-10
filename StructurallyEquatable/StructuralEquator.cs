using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Awalsh128
{
    public class StructuralEquator
    {
        private static readonly Lazy<StructuralEquator> instance = new Lazy<StructuralEquator>();

        public static StructuralEquator Instance
        {
            get { return instance.Value; }
        }

        private StructuralEquator()
        { }

        private bool AreArraysEqual(object[] arrayA, object[] arrayB, ISet<Tuple<object, object>> visited)
        {
            if (arrayA.Length != arrayB.Length) return false;
            return arrayA
                .Zip(arrayB, (itemA, itemB) => new { itemA, itemB })
                .All(p => IsEqual(p.itemA, p.itemB, visited));
        }

        public bool IsEqual(object a, object b)
        {
            return IsEqual(a, b, new HashSet<Tuple<object, object>>());
        }

        private bool IsEqual(object a, object b, ISet<Tuple<object, object>> visited)
        {
            if (a == null) return b == null;
            if (b == null) return a == null;

            if (!IsSameType(a, b)) return false;

            if (a.GetType().IsArray)
            {
                return AreArraysEqual((object[])a, (object[])b, visited);
            }

            return AreRecordsEqual(a, b, visited);
        }

        private bool AreRecordsEqual(object a, object b, ISet<Tuple<object, object>> visited)
        {
            var aType = a.GetType();
            var bType = b.GetType();

            if (aType.IsPrimitive) return a.Equals(b);

            var pair = new Tuple<object, object>(a, b);
            if (visited.Contains(pair)) return true;

            visited.Add(pair);

            var aFields = aType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var bFields = bType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return aFields
                .Zip(bFields, (aField, bField) => new { aField, bField })
                .All(p => IsEqual(p.aField.GetValue(a), p.bField.GetValue(b), visited));
        }

        private bool IsSameType(object a, object b)
        {
            return a.GetType().Equals(b.GetType());
        }
    }
}
