﻿using System.Diagnostics;

namespace SoftCube.Asserts
{
    /// <summary>
    /// アサート。
    /// </summary>
    public static partial class Assert
    {
        #region 静的メソッド

        #region False

        /// <summary>
        /// 条件がfalseであることを検証する。
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="message">検証失敗時の例外メッセージ</param>
        /// <exception cref="FalseException">条件がfalseでない場合、投げられる</exception>
        [Conditional("DEBUG")]
        public static void False(bool? condition, string message)
        {
            if (!condition.HasValue || condition.GetValueOrDefault())
            {
                throw new FalseException(condition, message);
            }
        }

        /// <summary>
        /// 条件がfalseであることを検証する。
        /// </summary>
        /// <param name="condition">条件</param>
        /// <exception cref="FalseException">条件がfalseでない場合、投げられる</exception>
        [Conditional("DEBUG")]
        public static void False(bool condition)
        {
            False((bool?)condition, null);
        }

        /// <summary>
        /// 条件がfalseであることを検証する。
        /// </summary>
        /// <param name="condition">条件</param>
        /// <exception cref="FalseException">条件がfalseでない場合、投げられる</exception>
        [Conditional("DEBUG")]
        public static void False(bool? condition)
        {
            False(condition, null);
        }

        /// <summary>
        /// 条件がfalseであることを検証する。
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="message">検証失敗時の例外メッセージ</param>
        /// <exception cref="FalseException">条件がfalseでない場合、投げられる</exception>
        [Conditional("DEBUG")]
        public static void False(bool condition, string message)
        {
            False((bool?)condition, message);
        }

        #endregion

        #region True

        /// <summary>
        /// 条件がtrueであることを検証する。
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="message">検証失敗時の例外メッセージ</param>
        /// <exception cref="TrueException">条件がtrueでない場合、投げられる</exception>
        public static void True(bool? condition, string message)
        {
            if (!condition.HasValue || !condition.GetValueOrDefault())
            {
                throw new TrueException(condition, message);
            }
        }

        /// <summary>
        /// 条件がtrueであることを検証する。
        /// </summary>
        /// <param name="condition">条件</param>
        /// <exception cref="TrueException">条件がtrueでない場合、投げられる</exception>
        [Conditional("DEBUG")]
        public static void True(bool condition)
        {
            True((bool?)condition, null);
        }

        /// <summary>
        /// 条件がtrueであることを検証する。
        /// </summary>
        /// <param name="condition">条件</param>
        /// <exception cref="TrueException">条件がtrueでない場合、投げられる</exception>
        [Conditional("DEBUG")]
        public static void True(bool? condition)
        {
            True(condition, null);
        }

        /// <summary>
        /// 条件がtrueであることを検証する。
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="message">検証失敗時の例外メッセージ</param>
        /// <exception cref="TrueException">条件がtrueでない場合、投げられる</exception>
        public static void True(bool condition, string message)
        {
            True((bool?)condition, message);
        }

        #endregion

        #endregion
    }
}