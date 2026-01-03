using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Fody
{
    // Token: 0x02000022 RID: 34
    public class TypeCache
    {
        // Token: 0x06000094 RID: 148 RVA: 0x00003457 File Offset: 0x00001657
        public TypeCache(Func<string, AssemblyDefinition> resolve)
        {
            this.resolve = resolve;
        }

        // Token: 0x06000095 RID: 149 RVA: 0x00003471 File Offset: 0x00001671
        public void BuildAssembliesToScan(BaseModuleWeaver weaver)
        {
            this.BuildAssembliesToScan(new BaseModuleWeaver[]
            {
                weaver
            });
        }

        // Token: 0x06000096 RID: 150 RVA: 0x00003484 File Offset: 0x00001684
        public void BuildAssembliesToScan(IEnumerable<BaseModuleWeaver> weavers)
        {
            Dictionary<string, AssemblyDefinition> assemblyDefinitions = new Dictionary<string, AssemblyDefinition>(StringComparer.OrdinalIgnoreCase);
            foreach (string assemblyName in this.GetAssembliesForScanning(weavers))
            {
                AssemblyDefinition assembly = this.resolve(assemblyName);
                if (assembly != null && !assemblyDefinitions.ContainsKey(assemblyName))
                {
                    assemblyDefinitions.Add(assemblyName, assembly);
                }
            }
            this.Initialise(assemblyDefinitions.Values);
        }

        // Token: 0x06000097 RID: 151 RVA: 0x00003504 File Offset: 0x00001704
        private IEnumerable<string> GetAssembliesForScanning(IEnumerable<BaseModuleWeaver> weavers)
        {
            foreach (string assemblyName in TypeCache.defaultAssemblies)
            {
                yield return assemblyName;
            }
            List<string>.Enumerator enumerator = default(List<string>.Enumerator);
            foreach (BaseModuleWeaver weaver in weavers)
            {
                foreach (string assemblyName2 in weaver.GetAssembliesForScanning())
                {
                    yield return assemblyName2;
                }
                IEnumerator<string> enumerator3 = null;
            }
            IEnumerator<BaseModuleWeaver> enumerator2 = null;
            yield break;
            yield break;
        }

        // Token: 0x06000098 RID: 152 RVA: 0x00003514 File Offset: 0x00001714
        private void Initialise(IEnumerable<AssemblyDefinition> assemblyDefinitions)
        {
            List<AssemblyDefinition> definitions = assemblyDefinitions.ToList<AssemblyDefinition>();
            foreach (AssemblyDefinition assembly in definitions)
            {
                foreach (TypeDefinition type in assembly.MainModule.GetTypes())
                {
                    this.AddIfPublic(type);
                }
            }
            foreach (AssemblyDefinition assembly2 in definitions)
            {
                using (Collection<ExportedType>.Enumerator enumerator4 = assembly2.MainModule.ExportedTypes.GetEnumerator())
                {
                    while (enumerator4.MoveNext())
                    {
                        ExportedType exportedType = enumerator4.Current;
                        if (!definitions.Any((AssemblyDefinition x) => x.Name.Name == exportedType.Scope.Name))
                        {
                            TypeDefinition typeDefinition = exportedType.Resolve();
                            if (typeDefinition != null)
                            {
                                this.AddIfPublic(typeDefinition);
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x06000099 RID: 153 RVA: 0x00003660 File Offset: 0x00001860
        public virtual TypeDefinition FindType(string typeName)
        {
            TypeDefinition cacheType;
            if (this.cachedTypes.TryGetValue(typeName, out cacheType))
            {
                return cacheType;
            }
            TypeDefinition fromValueType;
            if (this.FindFromValues(typeName, out fromValueType))
            {
                return fromValueType;
            }
            throw new WeavingException("Could not find '" + typeName + "'.");
        }

        // Token: 0x0600009A RID: 154 RVA: 0x000036A4 File Offset: 0x000018A4
        private bool FindFromValues(string typeName, [NotNullWhen(true)] out TypeDefinition type)
        {
            if (typeName.Contains('.'))
            {
                type = null;
                return false;
            }
            List<TypeDefinition> types = (from x in this.cachedTypes.Values
                                          where x.Name == typeName
                                          select x).ToList<TypeDefinition>();
            if (types.Count > 1)
            {
                throw new WeavingException("Found multiple types for '" + typeName + "'.");
            }
            if (types.Count == 0)
            {
                type = null;
                return false;
            }
            type = types[0];
            return true;
        }

        // Token: 0x0600009B RID: 155 RVA: 0x0000372F File Offset: 0x0000192F
        public virtual bool TryFindType(string typeName, [NotNullWhen(true)] out TypeDefinition type)
        {
            return this.cachedTypes.TryGetValue(typeName, out type) || this.FindFromValues(typeName, out type);
        }

        // Token: 0x0600009C RID: 156 RVA: 0x0000374A File Offset: 0x0000194A
        private void AddIfPublic(TypeDefinition type)
        {
            if (!type.IsPublic)
            {
                return;
            }
            if (this.cachedTypes.ContainsKey(type.FullName))
            {
                return;
            }
            this.cachedTypes.Add(type.FullName, type);
        }

        // Token: 0x0400003A RID: 58
        private Func<string, AssemblyDefinition> resolve;

        // Token: 0x0400003B RID: 59
        public static List<string> defaultAssemblies = new List<string>
        {
            "mscorlib",
            "System",
            "System.Runtime",
            "System.Core",
            "netstandard"
        };

        // Token: 0x0400003C RID: 60
        private Dictionary<string, TypeDefinition> cachedTypes = new Dictionary<string, TypeDefinition>();
    }
}
