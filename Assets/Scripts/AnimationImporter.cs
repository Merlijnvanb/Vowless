using System;
using System.Text;
using UnityEngine;
using Unity.Mathematics;
using Quantum;
using Sirenix.OdinInspector;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class AnimationImporter : MonoBehaviour
{
    private struct RawAnimationData
    {
        public Dictionary<string, RawFrame> frames;
    }

    private struct RawFrame
    {
        public int duration;
        public List<RawCurveEntry> guide;
        public List<RawCurveEntry> persistent;
        public List<RawCurveEntry> transient;
    }

    private struct RawCurveEntry
    {
        public string id;
        public float[] origin;
        public float[] rotation;
        public float[][] points;
    }

    public string JsonFileName;
    public string SaveLocation;
    public string SaveNameSuffix;
    
    public AnimationInfo Info;

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
        string path = Path.Combine(Application.streamingAssetsPath, "AnimationDataJSON/" + JsonFileName + ".json");
        string jsonText = File.ReadAllText(path);
        var raw = JsonConvert.DeserializeObject<RawAnimationData>(jsonText);

        var sortedFrame = raw.frames
            .OrderBy(kvp => int.Parse(kvp.Key))
            .Select(kvp => kvp.Value)
            .ToList();
        
        var genFrameArray = new FrameContainer[sortedFrame.Count];
        int cursor = 0;

        for (int i = 0; i < sortedFrame.Count; i++)
        {
            var rawFrame = sortedFrame[i];
            int duration =  rawFrame.duration;

            genFrameArray[i] = new FrameContainer
            {
                Span = new int2(cursor, cursor + duration - 1),
                Guide = rawFrame.guide.Select(r => ConvertCurveEntry(r, false)).ToArray(),
                Persistent = rawFrame.persistent.Select(r => ConvertCurveEntry(r, true)).ToArray(),
                Transient = rawFrame.transient.Select(r => ConvertCurveEntry(r, false)).ToArray()
            };
            
            cursor += duration;
        }

        data.Info = Info;
        data.Frames = genFrameArray;
    }

    private FrameCurveContainer ConvertCurveEntry(RawCurveEntry raw, bool persistent)
    {
        return new FrameCurveContainer
        {
            ID = persistent ? Enum.Parse<CurveID>(raw.id) : 0,
            Origin = ConvertPosition(raw.origin),
            Rotation = ConvertRotation(raw.rotation),
            Points = raw.points.Select(ConvertPosition).ToArray()
        };
    }

    private Vector3 ConvertPosition(float[] raw)
    {
        return new Vector3(raw[0], raw[2], raw[1]);
    }

    private Vector3 ConvertRotation(float[] raw)
    {
        return new Vector3(
            -raw[0] * Mathf.Rad2Deg,
            -raw[2] * Mathf.Rad2Deg,
            -raw[1] * Mathf.Rad2Deg
        );
    }

    private string GetAssetPath()
    {
        var path = new StringBuilder();
        var folderPath = Info.IsPartial ? "Partial/" : "Base/";
        
        path.Append(SaveLocation);
        path.Append(folderPath);
        path.Append(Info.ID.ToString());
        path.Append(folderPath);
        
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
