﻿using Mono.Cecil;
using System;

namespace SoftCube.Aspect
{
    /// <summary>
    /// メソッドレベルのアスペクト。
    /// </summary>
    public abstract class MethodLevelAspect : Attribute
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター
        /// </summary>
        public MethodLevelAspect()
            : base()
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 注入する。
        /// </summary>
        /// <param name="method">注入対象のメソッド定義</param>
        public void Inject(MethodDefinition method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            OnInject(method);
        }

        /// <summary>
        /// 注入する。
        /// </summary>
        /// <param name="target">注入対象のメソッド定義</param>
        protected abstract void OnInject(MethodDefinition method);

        #endregion
    }
}