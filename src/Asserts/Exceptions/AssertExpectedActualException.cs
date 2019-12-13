using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SoftCube.Asserts
{
    /// <summary>
    /// ���Ғl�Ǝ����l�̃A�T�[�g��O�B
    /// </summary>
    public class AssertExpectedActualException : AssertException
    {
        #region �v���p�e�B

        /// <summary>
        /// ���Ғl�B
        /// </summary>
        public string Expected { get; }

        /// <summary>
        /// ���Ғl�̃^�C�g���B
        /// </summary>
        public string ExpectedTitle { get; }

        /// <summary>
        /// �����l�B
        /// </summary>
        public string Actual { get; }

        /// <summary>
        /// �����l�̃^�C�g���B
        /// </summary>
        public string ActualTitle { get; }

        /// <summary>
        /// ��O���b�Z�[�W�i���Ғl�Ǝ����l���܂ށj�B
        /// </summary>
        public override string Message
        {
            get
            {
                var titleLength            = Math.Max(ExpectedTitle.Length, ActualTitle.Length) + 2;  // + the colon and space
                var formattedExpectedTitle = (ExpectedTitle + ":").PadRight(titleLength);
                var formattedActualTitle   = (ActualTitle + ":").PadRight(titleLength);

                return $"{base.Message}{Environment.NewLine}{formattedExpectedTitle}{Expected ?? "(null)"}{Environment.NewLine}{formattedActualTitle}{Actual ?? "(null)"}";
            }
        }

        #endregion

        #region �R���X�g���N�^�[

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl</param>
        /// <param name="actual">�����l</param>
        /// <param name="userMessage">���[�U�[���b�Z�[�W</param>
        /// <param name="expectedTitle">���Ғl�̃^�C�g��</param>
        /// <param name="actualTitle">�����l�̃^�C�g��</param>
        public AssertExpectedActualException(object expected, object actual, string userMessage, string expectedTitle = null, string actualTitle = null)
            : this(expected, actual, userMessage, expectedTitle, actualTitle, null)
        {
        }

        /// <summary>
        /// �R���X�g���N�^�[�B
        /// </summary>
        /// <param name="expected">���Ғl</param>
        /// <param name="actual">�����l</param>
        /// <param name="message">���b�Z�[�W</param>
        /// <param name="expectedTitle">���Ғl�̃^�C�g��</param>
        /// <param name="actualTitle">�����l�̃^�C�g��</param>
        /// <param name="innerException">������O</param>
        public AssertExpectedActualException(object expected, object actual, string message, string expectedTitle, string actualTitle, Exception innerException)
            : base(message, innerException)
        {
            Actual        = actual == null ? null : ConvertToString(actual);
            ActualTitle   = actualTitle ?? "Actual";
            Expected      = expected == null ? null : ConvertToString(expected);
            ExpectedTitle = expectedTitle ?? "Expected";

            if (actual != null && expected != null && Actual == Expected && actual.GetType() != expected.GetType())
            {
                Actual += string.Format(CultureInfo.CurrentCulture, " ({0})", actual.GetType().FullName);
                Expected += string.Format(CultureInfo.CurrentCulture, " ({0})", expected.GetType().FullName);
            }
        }

        #endregion

        #region �ÓI���\�b�h

        private static string ConvertToString(object value)
        {
            if (value is string stringValue)
            {
                return stringValue;
            }

            var formattedValue = ArgumentFormatter.Format(value);
            if (value is IEnumerable)
            {
                formattedValue = string.Format("{0} {1}", ConvertToSimpleTypeName(value.GetType().GetTypeInfo()), formattedValue);
            }

            return formattedValue;
        }
        private static string ConvertToSimpleTypeName(TypeInfo typeInfo)
        {
            if (!typeInfo.IsGenericType)
            {
                return typeInfo.Name;
            }

            var simpleNames = typeInfo.GenericTypeArguments.Select(type => ConvertToSimpleTypeName(type.GetTypeInfo()));
            var backTickIndex = typeInfo.Name.IndexOf('`');
            if (backTickIndex < 0)
            {
                backTickIndex = typeInfo.Name.Length;  // F# doesn't use backticks for generic type names
            }

            return string.Format("{0}<{1}>", typeInfo.Name.Substring(0, backTickIndex), string.Join(", ", simpleNames));
        }

        #endregion
    }
}