namespace SoftCube.Asserts
{
    /// <summary>
    /// Contains�A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́AAssert.Contains(...)�̎��s���ɓ�������B
    /// </remarks>
    public class ContainsException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl</param>
        /// <param name="actual">�����l</param>
        public ContainsException(object expected, object actual)
            : base(expected, actual, "Assert.Contains() Failure", "Not found", "In value")
        {
        }

        #endregion
    }
}