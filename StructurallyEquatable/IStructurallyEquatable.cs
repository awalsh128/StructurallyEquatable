namespace System
{
    /// <summary>
    /// Interface for objects using structural equality. Use 
    /// <see cref="StructuralEquator.Instance"/> for implementation.
    /// </summary>
    public interface IStructurallyEquatable
    {
        bool IsStructurallyEqual(object obj);
    }
}
