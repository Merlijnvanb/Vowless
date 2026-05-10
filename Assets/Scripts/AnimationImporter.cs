using System.Text;
using UnityEngine;
using Unity.Mathematics;
using Quantum;
using Sirenix.OdinInspector;
using UnityEditor;

[System.Serializable]
public struct ImportedFrame
{
    public GameObject Object;
    public int2 Span;
}

#if UNITY_EDITOR
[ExecuteInEditMode]
public class AnimationImporter : MonoBehaviour
{
    public string SaveLocation;
    public string SaveNameSuffix;
    
    public AnimationInfo Info;
    public ImportedFrame[] Frames;

    [Button("Generate Animation Data File")]
    private void GenerateAnimationDataFile()
    {
        var newData = ScriptableObject.CreateInstance<AnimationData>();
        FillAnimationData(newData);
        
        AssetDatabase.CreateAsset(newData, GetAssetPath());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void FillAnimationData(AnimationData data)
    {
        
    }

    private string GetAssetPath()
    {
        var path = new StringBuilder();

        path.Append(SaveLocation);
        path.Append(Info.ID.ToString());
        path.Append(Info.Type.ToString());
        
        if (Info.IsSaberDirDependent)
        {
            path.Append(Info.SaberDirection.ToString());
        }
        
        path.Append(SaveNameSuffix);

        return path.ToString();
    }
}
#endif
