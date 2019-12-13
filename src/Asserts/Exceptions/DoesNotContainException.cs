namespace SoftCube.Asserts
{
    /// <summary>
    /// DoesNotContain�A�T�[�g��O�B
    /// </summary>
    /// <remarks>
    /// �{��O�́AAssert.DoesNotContain(...)�̎��s���ɓ�������B
    /// </remarks>
    public class DoesNotContainException : AssertExpectedActualException
    {
        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl</param>
        /// <param name="actual">�����l</param>
        public DoesNotContainException(object expected, object actual)
            : base(expected, actual, "Assert.DoesNotContain() Failure", "Found", "In value")
        {
        }

        #endregion
    }
}