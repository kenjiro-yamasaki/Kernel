namespace SoftCube.Asserts
{
    /// <summary>
    /// Containsアサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、Assert.Contains(...)の失敗時に投げられます。
    /// </remarks>
    public class ContainsException : AssertExpectedActualException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        /// <param name="expected">期待値。</param>
        /// <param name="actual">実測値。</param>
        public ContainsException(object expected, object actual)
            : base(expected, actual, "Assert.Contains() Failure", "Not found", "In value")
        {
        }

        #endregion
    }
}