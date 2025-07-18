using System;
using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;

public class ModuleWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var afterAttributeType = FindType("ECS.Fody.AfterAttribute");
        if (afterAttributeType == null)
        {
            Debug.Log("未找到AfterAttribute类型，跳过织入");
            return;
        }

        var methodsWithAfterAttribute = FindMethodsWithAfterAttribute(afterAttributeType);
        ProcessAfterAttributeMethods(methodsWithAfterAttribute);
    }

    private TypeDefinition FindType(string typeName)
    {
        return ModuleDefinition.Types.FirstOrDefault(t => t.FullName == typeName) ??
               ModuleDefinition.AssemblyReferences
                   .SelectMany(ar => ModuleDefinition.AssemblyResolver.Resolve(ar).MainModule.Types)
                   .FirstOrDefault(t => t.FullName == typeName);
    }

    private List<MethodDefinition> FindMethodsWithAfterAttribute(TypeDefinition afterAttributeType)
    {
        var methodsWithAttribute = new List<MethodDefinition>();

        foreach (var type in ModuleDefinition.Types)
        {
            foreach (var method in type.Methods)
            {
                var afterAttribute = method.CustomAttributes
                    .FirstOrDefault(attr => attr.AttributeType.FullName == afterAttributeType.FullName);

                if (afterAttribute != null)
                {
                    methodsWithAttribute.Add(method);
                }
            }
        }

        return methodsWithAttribute;
    }

    private void ProcessAfterAttributeMethods(List<MethodDefinition> methodsWithAttribute)
    {
        foreach (var method in methodsWithAttribute)
        {
            var afterAttribute = method.CustomAttributes.First(attr => attr.AttributeType.FullName == "ECS.Fody.AfterAttribute");

            var targetMethodName = (string)afterAttribute.ConstructorArguments[1].Value;
            TypeDefinition targetType = null;

            // 检查是否指定了目标类型
            if (afterAttribute.ConstructorArguments[0].Value is TypeReference typeRef)
            {
                targetType = typeRef.Resolve();
            }

            var targetMethod = FindTargetMethod(targetType, targetMethodName);
            if (targetMethod != null)
            {
                // 获取IL处理器
                var il = targetMethod.Body.GetILProcessor();

                int paramOffset = targetMethod.IsStatic ? 0 : 1;
                int paramCount = targetMethod.Parameters.Count;
                int targetParamCount = targetMethod.Parameters.Count;
                int afterParamCount = method.Parameters.Count;

                // 参数数量不一致时，跳过或报错
                if (targetParamCount != afterParamCount)
                {
                    Debug.LogWarning($"参数数量不一致，跳过注入: {targetMethod.FullName} -> {method.FullName}");
                    continue;
                }

                // 遍历所有返回指令
                var returnInstructions = targetMethod.Body.Instructions.Where(i => i.OpCode == OpCodes.Ret).ToList();
                foreach (var ret in returnInstructions)
                {
                    for (int i = 0; i < afterParamCount; i++)
                    {
                        int argIndex = targetMethod.IsStatic ? i : i + 1;
                        if (argIndex <= 255)
                            il.InsertBefore(ret, il.Create(OpCodes.Ldarg_S, (byte)argIndex));
                        else
                            il.InsertBefore(ret, il.Create(OpCodes.Ldarg, argIndex));
                    }
                    // 调用After方法
                    il.InsertBefore(ret, il.Create(OpCodes.Call, method));
                }
                //Debug.Log($"已为方法 {targetMethod.FullName} 注入 After 调用: {method.FullName}");
            }
            else
            {
                Debug.Log($"未找到目标方法: {targetType?.FullName}.{targetMethodName}");
            }
        }
    }

    private MethodDefinition FindTargetMethod(TypeDefinition targetType, string methodName)
    {
        return targetType?.Methods.FirstOrDefault(m => m.Name == methodName);
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
        yield return "System";
        yield return "System.Runtime";
        yield return "System.Core";
    }

    public override bool ShouldCleanReference => true;
}
