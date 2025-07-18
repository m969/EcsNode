/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using FieldAttributes = Mono.Cecil.FieldAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace Fody.Unity
{
    public class WeavingTask : IWeavingTask
    {
        protected Action<int, string> log;
        protected XElement config;
        protected string assemblyPath;
        protected Func<string, Type> typeFinder;
        protected TestAssemblyResolver assemblyResolver;
        protected ModuleDefinition moduleDefinition;
        protected TypeCache typeCache;
        protected TypeSystem typeSystem;
        protected List<BaseModuleWeaver> weavers = new List<BaseModuleWeaver>();
        public WeavingTask(string assemblyPath, Func<string, Type> typeFinder, XElement config) : this(assemblyPath, typeFinder, config, null)
        {
        }
        public WeavingTask(string assemblyPath, Func<string, Type> typeFinder, XElement config, Action<int, string> log)
        {
            this.assemblyPath = assemblyPath;
            this.typeFinder = typeFinder;
            this.config = config;
            this.log = log;
        }

        public Func<string, Type> TypeFinder
        {
            get { return this.typeFinder; }
            set { this.typeFinder = value; }
        }

        public bool Execute()
        {
            bool hasSymbols = false;
            using (this.assemblyResolver = new TestAssemblyResolver())
            using (this.moduleDefinition = ReadModule(assemblyPath, out hasSymbols))
            {
                if (HasWeavingInfo())
                {
                    //Debug.LogError($"HasWeavingInfo");
                    return false;
                }

                InitializeWeavers();
                if (weavers.Count <= 0)
                {
                    //Debug.LogError($"weavers.Count <= 0");
                    return false;
                }

                ExecuteWeavers();
                AddWeavingInfo();
                WriteModule(assemblyPath, hasSymbols);
                return true;
            }
        }

        protected virtual void InitializeWeavers()
        {
            typeCache = new TypeCache(assemblyResolver.Resolve);
            foreach (XElement element in config.Elements())
            {
                var weaverName = element.Name.LocalName;
                var weaverType = typeFinder(weaverName);
                if (weaverType == null)
                {
                    log?.Invoke(0, $"Weaver type '{weaverName}' not found.");
                    continue;
                }
                BaseModuleWeaver weaver = (BaseModuleWeaver)Activator.CreateInstance(weaverType);
                InitializeWeaver(weaver, element);
                weavers.Add(weaver);
            }

            if (weavers.Count <= 0)
                return;

            typeSystem = new TypeSystem(typeCache.FindType, this.moduleDefinition);
            weavers.ForEach(m => m.TypeSystem = typeSystem);
        }

        protected void InitializeWeaver(BaseModuleWeaver weaver, XElement config)
        {
            weaver.Config = config;
            weaver.ModuleDefinition = moduleDefinition;
            weaver.AssemblyFilePath = assemblyPath;
#pragma warning disable CS0618 
            var value = config.Attributes("Debug").Select(a => a.Value).SingleOrDefault();
            if (value != null && XmlConvert.ToBoolean(value))
            {
                weaver.LogDebug = message => log?.Invoke(0, message);
                weaver.LogInfo = message => log?.Invoke(1, message);
                weaver.LogWarning = message => log?.Invoke(2, message);
                weaver.LogError = message => log?.Invoke(3, message);
                weaver.LogMessage = LogMessage;
                weaver.LogWarningPoint = LogWarningPoint;
                weaver.LogErrorPoint = LogErrorPoint;
            }

            weaver.FindType = typeCache.FindType;
            weaver.TryFindType = typeCache.TryFindType;
#pragma warning restore CS0618
            weaver.ResolveAssembly = assemblyResolver.Resolve;
            weaver.AssemblyResolver = assemblyResolver;
            typeCache.BuildAssembliesToScan(weaver);
        }

        protected virtual void ExecuteWeavers()
        {
            foreach (var weaver in weavers)
            {
                weaver.Execute();
            }
        }

        void LogMessage(string message, MessageImportance importance)
        {
            switch (importance)
            {
                case MessageImportance.High:
                    log?.Invoke(2, message);
                    break;
                case MessageImportance.Normal:
                    log?.Invoke(1, message);
                    break;
                case MessageImportance.Low:
                default:
                    log?.Invoke(0, message);
                    break;
            }
        }

        void LogWarningPoint(string message, SequencePoint point)
        {
            if (point == null)
                log?.Invoke(2, message);
            else
                log?.Invoke(2, string.Format("{0};point.Document.Url:{1}, point.StartLine:{2}, point.StartColumn:{3}, point.EndLine:{4}, point.EndColumn:{5}", message, point.Document.Url, point.StartLine, point.StartColumn, point.EndLine, point.EndColumn));
        }

        void LogErrorPoint(string message, SequencePoint point)
        {
            if (point == null)
                log?.Invoke(3, message);
            else
                log?.Invoke(3, string.Format("{0};point.Document.Url:{1}, point.StartLine:{2}, point.StartColumn:{3}, point.EndLine:{4}, point.EndColumn:{5}", message, point.Document.Url, point.StartLine, point.StartColumn, point.EndLine, point.EndColumn));
        }

        protected ModuleDefinition ReadModule(string assemblyFilePath, out bool hasSymbols)
        {
            var readerParameters = new ReaderParameters
            {
                AssemblyResolver = this.assemblyResolver,
                InMemory = true
            };

            var module = ModuleDefinition.ReadModule(assemblyFilePath, readerParameters);
            hasSymbols = false;
            try
            {
                module.ReadSymbols();
                hasSymbols = true;
            }
            catch { }
            return module;
        }

        protected virtual void WriteModule(string assemblyFilePath, bool hasSymbols)
        {
            var parameters = new WriterParameters
            {
                //StrongNameKeyPair = StrongNameKeyPair,
                WriteSymbols = hasSymbols
            };

            //ModuleDefinition.Assembly.Name.PublicKey = PublicKey;
            moduleDefinition.Write(assemblyFilePath, parameters);
        }

        protected virtual bool HasWeavingInfo()
        {
            var weavingInfoClassName = GetWeavingInfoClassName();
            if (moduleDefinition.Types.Any(x => x.Name == weavingInfoClassName))
                return true;
            return false;
        }

        protected virtual void AddWeavingInfo()
        {
            const TypeAttributes typeAttributes = TypeAttributes.NotPublic | TypeAttributes.Class;
            var typeDefinition = new TypeDefinition(null, GetWeavingInfoClassName(), typeAttributes, typeSystem.ObjectReference);
            moduleDefinition.Types.Add(typeDefinition);

            AddVersionField(typeof(BaseModuleWeaver).Assembly, "FodyVersion", typeDefinition);

            foreach (var weaver in weavers)
            {
                var configAssembly = weaver.GetType().Assembly;
                var name = weaver.Config.Name.LocalName;
                AddVersionField(configAssembly, name, typeDefinition);
            }
        }

        protected void AddVersionField(Assembly assembly, string name, TypeDefinition typeDefinition)
        {
            var weaverVersion = "0.0.0.0";
            var attrs = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute));
            var fileVersionAttribute = (AssemblyFileVersionAttribute)attrs.FirstOrDefault();
            if (fileVersionAttribute != null)
                weaverVersion = fileVersionAttribute.Version;

            const FieldAttributes fieldAttributes = FieldAttributes.Assembly |
                                                    FieldAttributes.Literal |
                                                    FieldAttributes.Static |
                                                    FieldAttributes.HasDefault;
            var field = new FieldDefinition(name, fieldAttributes, typeSystem.StringReference)
            {
                Constant = weaverVersion
            };

            typeDefinition.Fields.Add(field);
        }

        protected string GetWeavingInfoClassName()
        {
            var classPrefix = moduleDefinition.Assembly.Name.Name.Replace(".", "");
            return $"{classPrefix}_ProcessedByFody";
        }
    }
}