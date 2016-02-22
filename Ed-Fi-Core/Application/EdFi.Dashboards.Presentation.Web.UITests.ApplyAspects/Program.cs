using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if POSTSHARP

namespace EdFi.Dashboards.Presentation.Web.UITests.ApplyAspects
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("PostSharp is enabled in the solution, and so no aspects will be applied using Cecil.");
        }
    }
}

#else

using ApplyAspects;
using EdFi.Dashboards.Presentation.Core.UITests.Support.Aop.Architecture;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace EdFi.Dashboards.Presentation.Web.UITests.ApplyAspects
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("Full path to the assembly to be processed is required.");

            string filename = args[0];
            string targetAssemblyPath = Path.GetFullPath(filename);

            if (!File.Exists(targetAssemblyPath))
                throw new FileNotFoundException("Assembly to be processed was not found: " + targetAssemblyPath);

            // Load and process the assembly
            Console.WriteLine("Loading assembly module from '{0}'...", targetAssemblyPath);
            var readerParameters = new ReaderParameters { ReadSymbols = true };
            var module = ModuleDefinition.ReadModule(targetAssemblyPath, readerParameters);

            ProcessAssembly(module);

            Console.WriteLine("Saving assembly module to '{0}'...", targetAssemblyPath);
            var writerParameters = new WriterParameters { WriteSymbols = true };
            module.Write(targetAssemblyPath, writerParameters);
        }

        private static void ProcessAssembly(ModuleDefinition module)
        {
            var aspectAttributes =
                (from a in module.Assembly.CustomAttributes
                 where a.AttributeType.FullName.EndsWith("Aspect")
                 select a)
                .ToList();

            Console.WriteLine("{0} aspect attribute(s) found.", aspectAttributes.Count);

            foreach (var type in module.Types)
            {
                var aspectsToApplyToType =
                    (from a in aspectAttributes
                     where type.IsMatch(a)
                     orderby a.GetValue<int>("AspectPriority", int.MaxValue) descending
                     select a)
                    .ToList();

                // If no aspects apply, skip the type
                if (!aspectsToApplyToType.Any())
                    continue;

                Console.WriteLine("Processing type '{0}' with the following aspects: {1}",
                    type.FullName, string.Join(", ", aspectsToApplyToType.Select(a => a.AttributeType.Name)));

                foreach (var method in type.Methods.Where(m => !m.IsConstructor))
                {
                    var aspectsToApplyToMethod =
                        (from a in aspectsToApplyToType
                         where method.IsMatch(a)
                         orderby a.GetValue("AspectPriority", int.MaxValue) descending
                         select a)
                            .ToList();

                    foreach (var aspect in aspectsToApplyToMethod)
                        ProcessMethod(module, method, aspect);
                }
            }
        }

        private static void ProcessMethod(ModuleDefinition module, MethodDefinition method, CustomAttribute aspect)
        {
            method.Body.SimplifyMacros();

            MethodReference aspectConstructorRef = aspect.Constructor;

            MethodInfo getCurrentMethod = typeof(MethodBase).GetMethod("GetCurrentMethod", BindingFlags.Public | BindingFlags.Static);
            MethodReference getCurrentMethodRef = module.Import(getCurrentMethod);

            TypeReference methodInfoRef = module.Import(typeof(MethodInfo));
            TypeReference objectRef = module.Import(typeof(Object));

            MethodReference methodExecutionArgsCtorRef =
                module.Import(typeof(MethodExecutionArgs).GetConstructor(new Type[]
                    {
                        typeof(MethodInfo), typeof(object[])
                    }));

            MethodReference onEntryRef =
                module.Import(typeof(OnMethodBoundaryAspect).GetMethod("OnEntry"));

            MethodReference onSuccessRef =
                module.Import(typeof(OnMethodBoundaryAspect).GetMethod("OnSuccess"));

            MethodReference onExceptionRef =
                module.Import(typeof(OnMethodBoundaryAspect).GetMethod("OnException"));

            MethodReference onExitRef =
                module.Import(typeof(OnMethodBoundaryAspect).GetMethod("OnExit"));

            MethodInfo getFlowBehavior =
                typeof(MethodExecutionArgs).GetProperty("FlowBehavior").GetGetMethod();
            MethodReference getFlowBehaviorRef = module.Import(getFlowBehavior);

            MethodInfo setException =
                typeof(MethodExecutionArgs).GetProperty("Exception").GetSetMethod();
            MethodReference setExceptionRef = module.Import(setException);

            MethodInfo getException =
                typeof(MethodExecutionArgs).GetProperty("Exception").GetGetMethod();
            MethodReference getExceptionRef = module.Import(getException);

            MethodInfo getParameters = typeof(MethodBase).GetMethod("GetParameters");
            MethodReference getParametersRef = module.Import(getParameters);

            var processor = method.Body.GetILProcessor();
            var firstInstruction = method.Body.Instructions[0];

            // Original last instruction, change to a Nop.
            method.Body.Instructions.Last().OpCode = OpCodes.Nop;

            var finalInstruction = processor.Create(OpCodes.Nop);
            method.Body.Instructions.Add(finalInstruction);
            method.Body.Instructions.Add(processor.Create(OpCodes.Ret));

            // Add necessary variables
            int existingVariableCount = method.Body.Variables.Count;

            method.Body.Variables.Add(new VariableDefinition("__aspect", aspect.AttributeType)); // module.Import(aspect.GetType())));
            method.Body.Variables.Add(new VariableDefinition("__methodInfo", module.Import(typeof(MethodInfo))));
            method.Body.Variables.Add(new VariableDefinition("__methodExecutionArgs",
                                                             module.Import(typeof(MethodExecutionArgs))));
            method.Body.Variables.Add(new VariableDefinition("__ex", module.Import(typeof(Exception))));

            var argsVariableRef = new VariableDefinition("__args", module.Import(typeof(object[])));
            method.Body.Variables.Add(argsVariableRef);

            var boolVariableRef = new VariableDefinition("__bool", module.Import(typeof(bool)));
            method.Body.Variables.Add(boolVariableRef);

            var flowBehaviorVariableRef = new VariableDefinition("__flow", module.Import(typeof(FlowBehavior)));
            method.Body.Variables.Add(flowBehaviorVariableRef);

            Instruction L_0032_Instruction;
            Instruction firstAfterInstruction = Instruction.Create(OpCodes.Nop);

            const int Variable_aspect = 0;
            const int Variable_methodInfo = 1;
            const int Variable_methodExecutionArgs = 2;
            const int Variable_ex = 3;

            var beforeInstructions = new List<Instruction>
                {
                    processor.Create(OpCodes.Nop),
                    processor.Create(OpCodes.Newobj, aspectConstructorRef),
                    processor.Create(OpCodes.Stloc, Variable_aspect + existingVariableCount),
                    processor.Create(OpCodes.Call, getCurrentMethodRef),
                    processor.Create(OpCodes.Isinst, methodInfoRef),
                    processor.Create(OpCodes.Stloc, Variable_methodInfo + existingVariableCount),
                    processor.Create(OpCodes.Ldloc, Variable_methodInfo + existingVariableCount),
                };

            beforeInstructions.AddRange(new List<Instruction> {
                    processor.Create(OpCodes.Ldc_I4, method.Parameters.Count), // Number of arguments in method
                    processor.Create(OpCodes.Newarr, objectRef), // Create the object array (sized to hold args)
                    processor.Create(OpCodes.Stloc_S, argsVariableRef), // Save array to variable
                    processor.Create(OpCodes.Ldloc_S, argsVariableRef), // Load variable for evaluation
            });

            for (int i = 0; i < method.Parameters.Count; i++)
            {
                beforeInstructions.Add(processor.Create(OpCodes.Ldc_I4, i));  // Index value 0
                beforeInstructions.Add(processor.Create(OpCodes.Ldarg, i + 1)); // Load first argument

                var parameterType = method.Parameters[i].ParameterType;

                if (parameterType.IsValueType)
                    beforeInstructions.Add(processor.Create(OpCodes.Box, parameterType)); // Box the value types

                beforeInstructions.Add(processor.Create(OpCodes.Stelem_Ref)); // Assign argument to index 0 of array
                beforeInstructions.Add(processor.Create(OpCodes.Ldloc_S, argsVariableRef));
            }

            beforeInstructions.AddRange(new List<Instruction> {
                    //processor.Create(OpCodes.Ldc_I4, 2), // Number of arguments in method
                    //processor.Create(OpCodes.Newarr, objectRef), // Create the object array (sized to hold args)
                    //processor.Create(OpCodes.Stloc_S, argsVariableRef), // Save array to variable
                    //processor.Create(OpCodes.Ldloc_S, argsVariableRef), // Load variable for evaluation

                    //processor.Create(OpCodes.Ldc_I4_0), // Index value 0
                    //processor.Create(OpCodes.Ldarg_1), // Load first argument
                    //processor.Create(OpCodes.Stelem_Ref), // Assign argument to index 0 of array
                    //processor.Create(OpCodes.Ldloc_S, argsVariableRef),

                    //processor.Create(OpCodes.Ldc_I4_1), // Index value 1
                    //processor.Create(OpCodes.Ldarg_2), // Second argument
                    //processor.Create(OpCodes.Box, int32Ref), // Box the value types
                    //processor.Create(OpCodes.Stelem_Ref), // Assign argument to index 1 of array
                    //processor.Create(OpCodes.Ldloc_S, argsVariableRef),
                    
                    processor.Create(OpCodes.Newobj, methodExecutionArgsCtorRef),
                    processor.Create(OpCodes.Stloc, Variable_methodExecutionArgs + existingVariableCount),
                    (L_0032_Instruction = processor.Create(OpCodes.Nop)),
                    processor.Create(OpCodes.Ldloc, Variable_aspect + existingVariableCount),
                    processor.Create(OpCodes.Ldloc, Variable_methodExecutionArgs + existingVariableCount),
                    processor.Create(OpCodes.Callvirt, onEntryRef),
                    processor.Create(OpCodes.Nop),
                    processor.Create(OpCodes.Ldloc, Variable_methodExecutionArgs + existingVariableCount),
                    processor.Create(OpCodes.Callvirt, getFlowBehaviorRef),
                    processor.Create(OpCodes.Ldc_I4_3),
                    processor.Create(OpCodes.Ceq),
                    processor.Create(OpCodes.Ldc_I4_0),
                    processor.Create(OpCodes.Ceq),
                    processor.Create(OpCodes.Stloc_S, boolVariableRef),
                    processor.Create(OpCodes.Ldloc_S, boolVariableRef),
                    processor.Create(OpCodes.Brtrue_S, firstInstruction),
                    processor.Create(OpCodes.Leave, finalInstruction),
                });

            var L_0094_Instruction = processor.Create(OpCodes.Rethrow);
            var L_00a2_Instruction = processor.Create(OpCodes.Nop);
            var L_00b0_Instruction = processor.Create(OpCodes.Nop);
            var L_00b1_Instruction = processor.Create(OpCodes.Nop);
            var L_0096_Instruction = processor.Create(OpCodes.Leave_S, L_00b1_Instruction);
            var L_009f_Instruction = processor.Create(OpCodes.Nop);
            var L_0098_Instruction = processor.Create(OpCodes.Ldloc, 2 + existingVariableCount);

            Instruction L_005d_Instruction;
            Instruction L_00a5_Instruction;

            var afterInstructions = new List<Instruction>
                {
                    firstAfterInstruction,
                    processor.Create(OpCodes.Ldloc, Variable_aspect + existingVariableCount),
                    processor.Create(OpCodes.Ldloc, Variable_methodExecutionArgs + existingVariableCount),
                    processor.Create(OpCodes.Callvirt, onSuccessRef),
                    processor.Create(OpCodes.Nop),
                    processor.Create(OpCodes.Nop),
                    processor.Create(OpCodes.Leave_S, L_00a2_Instruction),
                    (L_005d_Instruction = processor.Create(OpCodes.Stloc, Variable_ex + existingVariableCount)),
                    processor.Create(OpCodes.Nop),
                    processor.Create(OpCodes.Ldloc, Variable_methodExecutionArgs + existingVariableCount),
                    processor.Create(OpCodes.Ldloc, Variable_ex + existingVariableCount),
                    processor.Create(OpCodes.Callvirt, setExceptionRef),
                    processor.Create(OpCodes.Nop),
                    processor.Create(OpCodes.Ldloc, Variable_aspect + existingVariableCount),
                    processor.Create(OpCodes.Ldloc, Variable_methodExecutionArgs + existingVariableCount),
                    processor.Create(OpCodes.Callvirt, onExceptionRef),
                    processor.Create(OpCodes.Nop),
                    processor.Create(OpCodes.Ldloc, Variable_methodExecutionArgs + existingVariableCount),
                    processor.Create(OpCodes.Callvirt, getFlowBehaviorRef),
                    processor.Create(OpCodes.Stloc_S, flowBehaviorVariableRef),
                    processor.Create(OpCodes.Ldloc_S, flowBehaviorVariableRef),
                    processor.Create(OpCodes.Switch,
                                     new[]
                                         {
                                             L_0094_Instruction, L_0096_Instruction, L_0094_Instruction, L_009f_Instruction,
                                             L_0098_Instruction
                                         }),
                    processor.Create(OpCodes.Br_S, L_009f_Instruction),
                    L_0094_Instruction,
                    L_0096_Instruction,
                    L_0098_Instruction,
                    processor.Create(OpCodes.Callvirt, getExceptionRef),
                    processor.Create(OpCodes.Throw),
                    L_009f_Instruction,
                    processor.Create(OpCodes.Leave_S, L_00a2_Instruction),
                    L_00a2_Instruction,
                    processor.Create(OpCodes.Leave_S, L_00b0_Instruction),
                    (L_00a5_Instruction = processor.Create(OpCodes.Nop)),
                    processor.Create(OpCodes.Ldloc, Variable_aspect + existingVariableCount),
                    processor.Create(OpCodes.Ldloc, Variable_methodExecutionArgs + existingVariableCount),
                    processor.Create(OpCodes.Callvirt, onExitRef), // L_00a8
                    processor.Create(OpCodes.Nop),
                    processor.Create(OpCodes.Nop),
                    processor.Create(OpCodes.Endfinally),
                    L_00b0_Instruction,
                    L_00b1_Instruction,
                };

            var catchHandler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                TryStart = L_0032_Instruction,
                TryEnd = L_005d_Instruction,
                HandlerStart = L_005d_Instruction,
                HandlerEnd = L_00a2_Instruction,
                CatchType = module.Import(typeof(Exception)),
            };

            var finallyHandler = new ExceptionHandler(ExceptionHandlerType.Finally)
            {
                TryStart = L_0032_Instruction,
                TryEnd = L_00a5_Instruction,
                HandlerStart = L_00a5_Instruction,
                HandlerEnd = L_00b0_Instruction,
            };

            // Add the OnEntry portion
            foreach (var instruction in beforeInstructions)
                processor.InsertBefore(firstInstruction, instruction);

            // Add the OnExit portion
            foreach (var instruction in afterInstructions)
                processor.InsertBefore(finalInstruction, instruction);

            //// Add try/catch
            method.Body.ExceptionHandlers.Add(catchHandler);
            method.Body.ExceptionHandlers.Add(finallyHandler);

            method.Body.OptimizeMacros();
        }
    }
}
#endif
