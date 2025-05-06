using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSEditor
{
    [Serializable]
    public class FolderData
    {
        [HorizontalGroup("Split"), HideLabel]
        public string FolderPath;
        [HorizontalGroup("Split"), HideLabel]
        public UnityEngine.Object Folder;
    }

    [Serializable]
    public class ModuleData
	{
        [HorizontalGroup("Split"), HideLabel]
        public string Name;

        [HideLabel, ReadOnly]
        public List<FolderData> ModuleFolders;

        [HorizontalGroup("Split/right", width: 40)]
        [Button("°²×°")]
        private void Install()
        {
            
        }

        [HorizontalGroup("Split/right", width: 40)]
        [Button("Ð¶ÔØ")]
        private void Uninstall()
        {
            
        }
    }

	[CreateAssetMenu(fileName = "ModuleGroup", menuName = "ModuleGroup")]
	public class ModuleGroup : SerializedScriptableObject
	{
        [HideReferenceObjectPicker, ListDrawerSettings(ShowPaging = false)]
        public List<ModuleData> Modules;
	}
}
