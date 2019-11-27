﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SoftCube.Aspect
{
    /// <summary>
    /// メソッド境界アスペクト属性。
    /// </summary>
    public abstract class OnMethodBoundaryAspect : MethodLevelAspect
    {
        #region コンストラクター

        /// <summary>
        /// コンストラクター。
        /// </summary>
        public OnMethodBoundaryAspect()
        {
        }

        #endregion

        #region メソッド

        /// <summary>
        /// 注入する。
        /// </summary>
        /// <param name="target">注入対象のメソッド定義</param>
        protected override void OnInject(MethodDefinition method)
        {
            var processor = method.Body.GetILProcessor();
            Debug.WriteLine($"-------------------");
            foreach (var instruction in processor.Body.Instructions)
            {
                Debug.WriteLine($"{instruction}");
            }

            var module = method.DeclaringType.Module.Assembly.MainModule;

            // ローカル変数を追加する。
            var aspectIndex = processor.Body.Variables.Count();
            processor.Body.Variables.Add(new VariableDefinition(module.ImportReference(GetType())));

            var methodExecutionArgsIndex = processor.Body.Variables.Count();
            processor.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(MethodExecutionArgs))));

            var exceptionIndex = processor.Body.Variables.Count();
            processor.Body.Variables.Add(new VariableDefinition(module.ImportReference(typeof(Exception))));

            // 命令を書き換える。
            var first = processor.Body.Instructions.First();
            var last  = processor.Body.Instructions.Last();
            Instruction tryStart;
            Instruction handlerStart;

            if (last.OpCode == OpCodes.Throw)
            {
                processor.Append(last = processor.Create(OpCodes.Ret));
            }
            else
            {
                if (method.ReturnType.FullName != "System.Void")
                {
                    last = last.Previous.Previous.Previous.Previous;
                }
            }

            //
            {
                // OnMethodBoundaryAspectの派生クラスを生成する。
                processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(GetType().GetConstructor(new Type[] { }))));
                processor.InsertBefore(first, processor.Create(OpCodes.Stloc, aspectIndex));

                // MethodExecutionArgsを生成する。
                processor.InsertBefore(first, processor.Create(OpCodes.Ldarg_0));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldc_I4, method.Parameters.Count));
                processor.InsertBefore(first, processor.Create(OpCodes.Newarr, module.ImportReference(typeof(object))));
                for (int parameterIndex = 0; parameterIndex < method.Parameters.Count; parameterIndex++)
                {
                    var parameter = method.Parameters[parameterIndex];
                    processor.InsertBefore(first, processor.Create(OpCodes.Dup));
                    processor.InsertBefore(first, processor.Create(OpCodes.Ldc_I4, parameterIndex));
                    processor.InsertBefore(first, processor.Create(OpCodes.Ldarg, parameterIndex + 1));
                    if (parameter.ParameterType.IsValueType)
                    {
                        processor.InsertBefore(first, processor.Create(OpCodes.Box, parameter.ParameterType));
                    }
                    processor.InsertBefore(first, processor.Create(OpCodes.Stelem_Ref));
                }
                processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(typeof(Arguments).GetConstructor(new Type[] { typeof(object[]) }))));
                processor.InsertBefore(first, processor.Create(OpCodes.Newobj, module.ImportReference(typeof(MethodExecutionArgs).GetConstructor(new Type[] { typeof(object), typeof(Arguments) }))));
                processor.InsertBefore(first, processor.Create(OpCodes.Stloc, methodExecutionArgsIndex));

                processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, methodExecutionArgsIndex));
                processor.InsertBefore(first, processor.Create(OpCodes.Call, module.ImportReference(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod), new Type[]{ }))));
                processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Method)).GetSetMethod())));

                // OnEntryメソッドを呼び出す。
                processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, aspectIndex));
                processor.InsertBefore(first, processor.Create(OpCodes.Ldloc, methodExecutionArgsIndex));
                processor.InsertBefore(first, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnEntry)))));

                // tryの開始位置を挿入する。
                processor.InsertBefore(first, tryStart = processor.Create(OpCodes.Nop));
            }

            {
                // OnSuccessメソッドを呼び出す。
                processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, aspectIndex));
                processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, methodExecutionArgsIndex));
                processor.InsertBefore(last, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnSuccess)))));

                // OnExitメソッドを呼び出す。
                processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, aspectIndex));
                processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, methodExecutionArgsIndex));
                processor.InsertBefore(last, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));

                processor.InsertBefore(last, processor.Create(OpCodes.Leave_S, last));
            }
            {
                // catchの開始位置を挿入する。
                processor.InsertBefore(last, handlerStart = processor.Create(OpCodes.Stloc, exceptionIndex));

                processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, methodExecutionArgsIndex));
                processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, exceptionIndex));
                processor.InsertBefore(last, processor.Create(OpCodes.Callvirt, module.ImportReference(typeof(MethodExecutionArgs).GetProperty(nameof(MethodExecutionArgs.Exception)).GetSetMethod())));

                // OnExceptionメソッドを呼び出す。
                processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, aspectIndex));
                processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, methodExecutionArgsIndex));
                processor.InsertBefore(last, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnException)))));

                // OnExitメソッドを呼び出す（例外時）。
                processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, aspectIndex));
                processor.InsertBefore(last, processor.Create(OpCodes.Ldloc, methodExecutionArgsIndex));
                processor.InsertBefore(last, processor.Create(OpCodes.Callvirt, module.ImportReference(GetType().GetMethod(nameof(OnExit)))));

                //
                processor.InsertBefore(last, processor.Create(OpCodes.Rethrow));
            }

            // 例外ハンドラーを追加する。
            {
                // finallyハンドラーを追加する。
                var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
                {
                    CatchType    = module.ImportReference(typeof(Exception)),
                    TryStart     = tryStart,
                    TryEnd       = handlerStart,
                    HandlerStart = handlerStart,
                    HandlerEnd   = last,
                };

                processor.Body.ExceptionHandlers.Add(handler);
            }

            Debug.WriteLine($"-------------------");
            foreach (var instruction in processor.Body.Instructions)
            {
                Debug.WriteLine($"{instruction}");
            }
        }

        #region イベントハンドラー

        /// <summary>
        /// 開始イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数</param>
        public virtual void OnEntry(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// 成功終了イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数</param>
        public virtual void OnSuccess(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// 例外終了イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数</param>
        public virtual void OnException(MethodExecutionArgs args)
        {
        }

        /// <summary>
        /// 終了イベントハンドラー。
        /// </summary>
        /// <param name="args">メソッド実行引数</param>
        public virtual void OnExit(MethodExecutionArgs args)
        {
        }

        #endregion

        #endregion
    }
}