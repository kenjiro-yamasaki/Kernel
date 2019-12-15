namespace SoftCube.Asserts
{
    /// <summary>
    /// NotEmptyアサート例外。
    /// </summary>
    /// <remarks>
    /// 本例外は、Assert.NotEmpty(...)の失敗時に投げられます。
    /// </remarks>
    public class NotEmptyException : AssertException
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public NotEmptyException()
            : base("Assert.NotEmpty() Failure")
        {
        }

        #endregion
    }
}
