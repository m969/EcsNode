using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;

namespace Fody
{
    // Token: 0x0200001E RID: 30
    public class TestAssemblyResolver : IAssemblyResolver, IDisposable
    {
        // Token: 0x06000078 RID: 120 RVA: 0x00002E4C File Offset: 0x0000104C
        public void Dispose()
        {
            foreach (AssemblyDefinition definition in this.definitions.Values)
            {
                definition.Dispose();
            }
        }

        // Token: 0x06000079 RID: 121 RVA: 0x00002EA4 File Offset: 0x000010A4
        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return this.Resolve(name.Name);
        }

        // Token: 0x0600007A RID: 122 RVA: 0x00002EB4 File Offset: 0x000010B4
        public AssemblyDefinition Resolve(string name)
        {
            AssemblyDefinition definition;
            if (!this.definitions.TryGetValue(name, out definition))
            {
                string assemblyLocation;
                if (!TestAssemblyResolver.TryGetAssemblyLocation(name, out assemblyLocation))
                {
                    return null;
                }
                definition = (this.definitions[name] = this.GetAssemblyDefinition(assemblyLocation));
            }
            return definition;
        }

        // Token: 0x0600007B RID: 123 RVA: 0x00002EF4 File Offset: 0x000010F4
        private static bool TryGetAssemblyLocation(string name, [NotNullWhen(true)] out string assemblyLocation)
        {
            if (string.Equals(name, "netstandard", StringComparison.OrdinalIgnoreCase))
            {
                string netstandard = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet\\sdk\\NuGetFallbackFolder\\netstandard.library\\2.0.0\\build\\netstandard2.0\\ref\\netstandard.dll");
                if (File.Exists(netstandard))
                {
                    assemblyLocation = netstandard;
                    return true;
                }
            }
            Assembly assembly = TestAssemblyResolver.GetAssembly(name);
            if (assembly == null)
            {
                assemblyLocation = null;
                return false;
            }
            assemblyLocation = assembly.Location;
            return true;
        }

        // Token: 0x0600007C RID: 124 RVA: 0x00002F4C File Offset: 0x0000114C
        private static Assembly GetAssembly(string name)
        {
            if (string.Equals(name, "System", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(GeneratedCodeAttribute).Assembly;
            }
            Assembly result;
            try
            {
                result = Assembly.LoadWithPartialName(name);
            }
            catch (FileNotFoundException)
            {
                result = null;
            }
            return result;
        }

        // Token: 0x0600007D RID: 125 RVA: 0x00002F98 File Offset: 0x00001198
        private AssemblyDefinition GetAssemblyDefinition(string assemblyLocation)
        {
            ReaderParameters readerParameters = new ReaderParameters()
            {
                ReadWrite = false,
                ReadSymbols = false,
                AssemblyResolver = this
            };
            return AssemblyDefinition.ReadAssembly(assemblyLocation, readerParameters);
        }

        // Token: 0x0600007E RID: 126 RVA: 0x00002FC8 File Offset: 0x000011C8
        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return this.Resolve(name);
        }

        // Token: 0x04000034 RID: 52
        private Dictionary<string, AssemblyDefinition> definitions = new Dictionary<string, AssemblyDefinition>(StringComparer.OrdinalIgnoreCase);
    }
}
