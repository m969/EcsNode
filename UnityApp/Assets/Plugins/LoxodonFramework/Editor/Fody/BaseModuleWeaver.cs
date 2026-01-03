using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Fody
{
    // Token: 0x02000018 RID: 24
    [Serializable]
    public enum MessageImportance
    {
        // Token: 0x04000027 RID: 39
        High,
        // Token: 0x04000028 RID: 40
        Normal,
        // Token: 0x04000029 RID: 41
        Low
    }
    public class SequencePointMessage
    {
        // Token: 0x06000075 RID: 117 RVA: 0x00002E24 File Offset: 0x00001024
        public SequencePointMessage(string text, SequencePoint sequencePoint)
        {
            this.Text = text;
            this.SequencePoint = sequencePoint;
        }

        // Token: 0x17000023 RID: 35
        // (get) Token: 0x06000076 RID: 118 RVA: 0x00002E3A File Offset: 0x0000103A
        public string Text { get; }

        // Token: 0x17000024 RID: 36
        // (get) Token: 0x06000077 RID: 119 RVA: 0x00002E42 File Offset: 0x00001042
        public SequencePoint SequencePoint { get; }
    }
    public static class CecilExtensions
    {
        // Token: 0x06000064 RID: 100 RVA: 0x00002948 File Offset: 0x00000B48
        public static SequencePoint GetSequencePoint(this MethodDefinition method)
        {
            Guard.AgainstNull("method", method);
            return (from instruction in method.Body.Instructions
                    select method.DebugInformation.GetSequencePoint(instruction)).FirstOrDefault((SequencePoint sequencePoint) => sequencePoint != null);
        }
    }
    public class WeavingException : Exception
    {
        // Token: 0x060000C1 RID: 193 RVA: 0x00003B42 File Offset: 0x00001D42
        public WeavingException(string message) : base(message)
        {
        }

        // Token: 0x1700004C RID: 76
        // (get) Token: 0x060000C2 RID: 194 RVA: 0x00003B4B File Offset: 0x00001D4B
        // (set) Token: 0x060000C3 RID: 195 RVA: 0x00003B53 File Offset: 0x00001D53
        public SequencePoint SequencePoint { get; set; }
    }
    // Token: 0x02000021 RID: 33
    // (Invoke) Token: 0x06000091 RID: 145
    [Obsolete("No longer required as BaseModuleWeaver.TryFindType has been replace with BaseModuleWeaver.TryFindTypeDefinition", false)]
    public delegate bool TryFindTypeFunc(string typeName, out TypeDefinition type);
    // Token: 0x02000015 RID: 21
    public abstract class BaseModuleWeaver
    {
        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000022 RID: 34 RVA: 0x000024EC File Offset: 0x000006EC
        // (set) Token: 0x06000023 RID: 35 RVA: 0x000024F4 File Offset: 0x000006F4
        public XElement Config { get; set; } = BaseModuleWeaver.Empty;

        // Token: 0x06000024 RID: 36 RVA: 0x000024FD File Offset: 0x000006FD
        public virtual void WriteDebug(string message)
        {
            Guard.AgainstNullAndEmpty("message", message);
            this.LogDebug(message);
        }

        // Token: 0x17000009 RID: 9
        // (get) Token: 0x06000025 RID: 37 RVA: 0x00002516 File Offset: 0x00000716
        // (set) Token: 0x06000026 RID: 38 RVA: 0x0000251E File Offset: 0x0000071E
        [Obsolete("Use WriteDebug", false)]
        public Action<string> LogDebug { get; set; } = delegate (string m)
        {
        };

        // Token: 0x06000027 RID: 39 RVA: 0x00002527 File Offset: 0x00000727
        public virtual void WriteInfo(string message)
        {
            Guard.AgainstNullAndEmpty("message", message);
            this.LogInfo(message);
        }

        // Token: 0x1700000A RID: 10
        // (get) Token: 0x06000028 RID: 40 RVA: 0x00002540 File Offset: 0x00000740
        // (set) Token: 0x06000029 RID: 41 RVA: 0x00002548 File Offset: 0x00000748
        [Obsolete("Use WriteInfo", false)]
        public Action<string> LogInfo { get; set; } = delegate (string m)
        {
        };

        // Token: 0x0600002A RID: 42 RVA: 0x00002551 File Offset: 0x00000751
        public virtual void WriteMessage(string message, MessageImportance importance)
        {
            Guard.AgainstNullAndEmpty("message", message);
            this.LogMessage(message, importance);
        }

        // Token: 0x1700000B RID: 11
        // (get) Token: 0x0600002B RID: 43 RVA: 0x0000256B File Offset: 0x0000076B
        // (set) Token: 0x0600002C RID: 44 RVA: 0x00002573 File Offset: 0x00000773
        [Obsolete("Use WriteMessage", false)]
        public Action<string, MessageImportance> LogMessage { get; set; } = delegate (string m, MessageImportance p)
        {
        };

        // Token: 0x0600002D RID: 45 RVA: 0x0000257C File Offset: 0x0000077C
        public virtual void WriteWarning(string message)
        {
            Guard.AgainstNullAndEmpty("message", message);
            this.LogWarning(message);
        }

        // Token: 0x0600002E RID: 46 RVA: 0x00002595 File Offset: 0x00000795
        public virtual void WriteWarning(string message, SequencePoint sequencePoint)
        {
            Guard.AgainstNullAndEmpty("message", message);
            this.LogWarningPoint(message, sequencePoint);
        }

        // Token: 0x0600002F RID: 47 RVA: 0x000025AF File Offset: 0x000007AF
        public virtual void WriteWarning(string message, MethodDefinition method)
        {
            Guard.AgainstNullAndEmpty("message", message);
            Guard.AgainstNull("method", method);
            this.LogWarningPoint(message, method.GetSequencePoint());
        }

        // Token: 0x1700000C RID: 12
        // (get) Token: 0x06000030 RID: 48 RVA: 0x000025D9 File Offset: 0x000007D9
        // (set) Token: 0x06000031 RID: 49 RVA: 0x000025E1 File Offset: 0x000007E1
        [Obsolete("Use WriteWarning", false)]
        public Action<string> LogWarning { get; set; } = delegate (string m)
        {
        };

        // Token: 0x1700000D RID: 13
        // (get) Token: 0x06000032 RID: 50 RVA: 0x000025EA File Offset: 0x000007EA
        // (set) Token: 0x06000033 RID: 51 RVA: 0x000025F2 File Offset: 0x000007F2
        [Obsolete("Use WriteWarning", false)]
        public Action<string, SequencePoint> LogWarningPoint
        {
            get;
            set;
        } = delegate (string m, SequencePoint p)
        {
        };

        // Token: 0x06000034 RID: 52 RVA: 0x000025FB File Offset: 0x000007FB
        public virtual void WriteError(string message)
        {
            Guard.AgainstNullAndEmpty("message", message);
            this.LogError(message);
        }

        // Token: 0x06000035 RID: 53 RVA: 0x00002614 File Offset: 0x00000814
        public virtual void WriteError(string message, SequencePoint sequencePoint)
        {
            Guard.AgainstNullAndEmpty("message", message);
            this.LogErrorPoint(message, sequencePoint);
        }

        // Token: 0x06000036 RID: 54 RVA: 0x0000262E File Offset: 0x0000082E
        public virtual void WriteError(string message, MethodDefinition method)
        {
            Guard.AgainstNullAndEmpty("message", message);
            this.LogErrorPoint(message, method.GetSequencePoint());
        }

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x06000037 RID: 55 RVA: 0x0000264D File Offset: 0x0000084D
        // (set) Token: 0x06000038 RID: 56 RVA: 0x00002655 File Offset: 0x00000855
        [Obsolete("Use WriteError", false)]
        public Action<string> LogError { get; set; } = delegate (string m)
        {
        };

        // Token: 0x1700000F RID: 15
        // (get) Token: 0x06000039 RID: 57 RVA: 0x0000265E File Offset: 0x0000085E
        // (set) Token: 0x0600003A RID: 58 RVA: 0x00002666 File Offset: 0x00000866

        [Obsolete("Use WriteError", false)]
        public Action<string, SequencePoint> LogErrorPoint
        {

            get;
            set;
        } = delegate (string m, SequencePoint p)
        {
        };

        // Token: 0x17000010 RID: 16
        // (get) Token: 0x0600003B RID: 59 RVA: 0x0000266F File Offset: 0x0000086F
        // (set) Token: 0x0600003C RID: 60 RVA: 0x00002677 File Offset: 0x00000877

        public Func<string, AssemblyDefinition> ResolveAssembly
        {

            get;
            set;
        }

        // Token: 0x17000011 RID: 17
        // (get) Token: 0x0600003D RID: 61 RVA: 0x00002680 File Offset: 0x00000880
        // (set) Token: 0x0600003E RID: 62 RVA: 0x00002688 File Offset: 0x00000888
        public IAssemblyResolver AssemblyResolver { get; set; }

        // Token: 0x17000012 RID: 18
        // (get) Token: 0x0600003F RID: 63 RVA: 0x00002691 File Offset: 0x00000891
        // (set) Token: 0x06000040 RID: 64 RVA: 0x00002699 File Offset: 0x00000899
        public ModuleDefinition ModuleDefinition { get; set; }

        // Token: 0x17000013 RID: 19
        // (get) Token: 0x06000041 RID: 65 RVA: 0x000026A2 File Offset: 0x000008A2
        // (set) Token: 0x06000042 RID: 66 RVA: 0x000026AA File Offset: 0x000008AA
        public TypeSystem TypeSystem { get; set; }

        // Token: 0x17000014 RID: 20
        // (get) Token: 0x06000043 RID: 67 RVA: 0x000026B3 File Offset: 0x000008B3
        // (set) Token: 0x06000044 RID: 68 RVA: 0x000026BB File Offset: 0x000008BB
        public string AssemblyFilePath { get; set; }

        // Token: 0x17000015 RID: 21
        // (get) Token: 0x06000045 RID: 69 RVA: 0x000026C4 File Offset: 0x000008C4
        // (set) Token: 0x06000046 RID: 70 RVA: 0x000026CC File Offset: 0x000008CC
        public string ProjectDirectoryPath { get; set; }

        // Token: 0x17000016 RID: 22
        // (get) Token: 0x06000047 RID: 71 RVA: 0x000026D5 File Offset: 0x000008D5
        // (set) Token: 0x06000048 RID: 72 RVA: 0x000026DD File Offset: 0x000008DD
        public string ProjectFilePath { get; set; }

        // Token: 0x17000017 RID: 23
        // (get) Token: 0x06000049 RID: 73 RVA: 0x000026E6 File Offset: 0x000008E6
        // (set) Token: 0x0600004A RID: 74 RVA: 0x000026EE File Offset: 0x000008EE
        public string DocumentationFilePath { get; set; }

        // Token: 0x17000018 RID: 24
        // (get) Token: 0x0600004B RID: 75 RVA: 0x000026F7 File Offset: 0x000008F7
        // (set) Token: 0x0600004C RID: 76 RVA: 0x000026FF File Offset: 0x000008FF
        public string AddinDirectoryPath { get; set; }

        // Token: 0x17000019 RID: 25
        // (get) Token: 0x0600004D RID: 77 RVA: 0x00002708 File Offset: 0x00000908
        // (set) Token: 0x0600004E RID: 78 RVA: 0x00002710 File Offset: 0x00000910
        public string SolutionDirectoryPath { get; set; }

        // Token: 0x1700001A RID: 26
        // (get) Token: 0x0600004F RID: 79 RVA: 0x00002719 File Offset: 0x00000919
        // (set) Token: 0x06000050 RID: 80 RVA: 0x00002721 File Offset: 0x00000921
        public string References { get; set; }

        // Token: 0x1700001B RID: 27
        // (get) Token: 0x06000051 RID: 81 RVA: 0x0000272A File Offset: 0x0000092A
        // (set) Token: 0x06000052 RID: 82 RVA: 0x00002732 File Offset: 0x00000932
        public List<string> ReferenceCopyLocalPaths { get; set; } = new List<string>();

        // Token: 0x1700001C RID: 28
        // (get) Token: 0x06000053 RID: 83 RVA: 0x0000273B File Offset: 0x0000093B
        // (set) Token: 0x06000054 RID: 84 RVA: 0x00002743 File Offset: 0x00000943
        public List<string> RuntimeCopyLocalPaths { get; set; } = new List<string>();

        // Token: 0x1700001D RID: 29
        // (get) Token: 0x06000055 RID: 85 RVA: 0x0000274C File Offset: 0x0000094C
        // (set) Token: 0x06000056 RID: 86 RVA: 0x00002754 File Offset: 0x00000954
        public List<string> DefineConstants { get; set; } = new List<string>();

        // Token: 0x06000057 RID: 87
        public abstract void Execute();

        // Token: 0x06000058 RID: 88 RVA: 0x0000275D File Offset: 0x0000095D
        public virtual void Cancel()
        {
        }

        // Token: 0x06000059 RID: 89
        public abstract IEnumerable<string> GetAssembliesForScanning();

        // Token: 0x0600005A RID: 90 RVA: 0x0000275F File Offset: 0x0000095F
        public TypeDefinition FindTypeDefinition(string name)
        {
            return this.FindType(name);
        }

        // Token: 0x1700001E RID: 30
        // (get) Token: 0x0600005B RID: 91 RVA: 0x0000276D File Offset: 0x0000096D
        // (set) Token: 0x0600005C RID: 92 RVA: 0x00002775 File Offset: 0x00000975
        [Obsolete("Use FindTypeDefinition", false)]
        public Func<string, TypeDefinition> FindType { get; set; } = delegate (string _)
        {
            throw new WeavingException("FindType has not been set.");
        };

        // Token: 0x0600005D RID: 93 RVA: 0x0000277E File Offset: 0x0000097E
        public bool TryFindTypeDefinition(string name, [NotNullWhen(true)] out TypeDefinition type)
        {
            return this.TryFindType(name, out type);
        }

        // Token: 0x1700001F RID: 31
        // (get) Token: 0x0600005E RID: 94 RVA: 0x0000278D File Offset: 0x0000098D
        // (set) Token: 0x0600005F RID: 95 RVA: 0x00002795 File Offset: 0x00000995
        [Obsolete("Use TryFindTypeDefinition", false)]
        public TryFindTypeFunc TryFindType { get; set; } = delegate (string name, out TypeDefinition type)
        {
            throw new WeavingException("TryFindType has not been set.");
        };

        // Token: 0x06000060 RID: 96 RVA: 0x0000279E File Offset: 0x0000099E
        public virtual void AfterWeaving()
        {
        }

        // Token: 0x17000020 RID: 32
        // (get) Token: 0x06000061 RID: 97 RVA: 0x000027A0 File Offset: 0x000009A0
        public virtual bool ShouldCleanReference
        {
            get
            {
                return false;
            }
        }

        // Token: 0x0400000D RID: 13
        private static XElement Empty = new XElement("Empty");
    }
}
