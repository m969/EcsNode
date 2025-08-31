#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace KevinIglesias
{
    [System.Serializable]
    public class MaterialType
    {
        [HideInInspector]
        public string materialName;
        
        public Material defaultMaterial;
        public Material uRPMaterial;
        
        public SkinnedMeshRenderer[] sMR;
        public MeshRenderer[] mR;
    }

    [ExecuteInEditMode]
    public class HumanoidGiantDemo : MonoBehaviour
    {
        [SerializeField]
        public List<MaterialType> materialTypes;

        void OnValidate()
        {
            if(materialTypes == null)
            {
                return;
            }
            
            for(int i = 0; i < materialTypes.Count; i++)
            {
                
                if(materialTypes[i].defaultMaterial != null)
                {
                    materialTypes[i].materialName = materialTypes[i].defaultMaterial.name;
                }
                
                for(int j = 0; j < materialTypes[i].sMR.Length; j++)
                {
                    if(materialTypes[i].sMR[j] != null)
                    {
                        if(GraphicsSettings.currentRenderPipeline == null)
                        {
                            if(materialTypes[i].defaultMaterial != null)
                            {
                                materialTypes[i].sMR[j].material = materialTypes[i].defaultMaterial;
                            }
                        }else{
                            if(materialTypes[i].uRPMaterial != null)
                            {
                                materialTypes[i].sMR[j].material = materialTypes[i].uRPMaterial;
                            }
                        }
                    }
                }
                
                for(int j = 0; j < materialTypes[i].mR.Length; j++)
                {
                    if(materialTypes[i].mR[j] != null)
                    {
                        if(GraphicsSettings.currentRenderPipeline == null)
                        {
                            if(materialTypes[i].defaultMaterial != null)
                            {
                                materialTypes[i].mR[j].material = materialTypes[i].defaultMaterial;
                            }
                        }else{
                            if(materialTypes[i].uRPMaterial != null)
                            {
                                materialTypes[i].mR[j].material = materialTypes[i].uRPMaterial;
                            }
                        }
                    }
                }
            }
        }
        
        void OnEnable()
        {
            OnValidate();
        }
        
        void Update(){}
    }
}
#endif
