﻿using PostSharp.Aspects;
using PostSharp.Serialization;
using System;

namespace PostSharp.Sample
{
    //[PSerializable]
    //class LoggerAttribute : OnMethodBoundaryAspect
    //{
    //    public override void OnEntry(MethodExecutionArgs args)
    //    {
    //        Console.WriteLine("OnEntry {0}", args.Method.Name);
    //    }

    //    public override void OnExit(MethodExecutionArgs args)
    //    {
    //        Console.WriteLine("OnExit {0}", args.Method.Name);
    //    }
    //}

    [PSerializable]
    class LoggerAttribute : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            Console.WriteLine("BEFORE");

            args.Proceed();

            Console.WriteLine("AFTER");

        }




    }



}
