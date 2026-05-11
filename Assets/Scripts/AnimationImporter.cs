using System;
using System.Text;
using UnityEngine;
using Unity.Mathematics;
using Quantum;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

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
        var genFrameArray = new FrameContainer[Frames.Length];
        var elementList = new List<AnimatableElement>();
            
        switch (Info.Type)
        {
            case AnimationType.Full:
                elementList.AddRange((AnimatableElement[])Enum.GetValues(typeof(AnimatableElement)));
                break;
            
            case AnimationType.Lower:
                elementList.Add(AnimatableElement.Legs);
                break;
            
            case AnimationType.Upper:
                elementList.AddRange(Enum.GetValues(typeof(AnimatableElement)).Cast<AnimatableElement>().Where(e => e != AnimatableElement.Legs));
                break;
        }
        
        for (int i = 0; i < Frames.Length; i++)
        {
            genFrameArray[i] = GetFrameData(Frames[i], elementList);
        }

        data.Info = Info;
        data.Frames = genFrameArray;
    }

    private FrameContainer GetFrameData(ImportedFrame frame, List<AnimatableElement> elementEnums)
    {
        var frameElements = new List<FrameElement>();

        foreach (var element in elementEnums)
        {
            var transform = frame.Object.transform.Find(element + "_MESH");
            var mesh = transform.GetComponent<MeshFilter>().sharedMesh;

            frameElements.Add(new FrameElement
            {
                Element = element,

                Mesh = mesh,
                LocalPosition = transform.localPosition,
                LocalRotation = transform.localRotation,
                LocalScale = transform.localScale
            });
        }
        
        return new FrameContainer
        {
            FrameElements = frameElements.ToArray(),
            Span = frame.Span
        };
    }

    private string GetAssetPath()
    {
        var path = new StringBuilder();

        path.Append(SaveLocation);
        path.Append(Info.Type + "/");
        path.Append(Info.ID.ToString());
        path.Append(Info.Type.ToString());
        
        if (Info.IsSaberDirDependent)
        {
            path.Append(Info.SaberDirection.ToString());
        }
        
        path.Append(SaveNameSuffix);
        path.Append(".asset");

        return path.ToString();
    }
}
#endif
