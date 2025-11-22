using System.Collections.Generic;
using Quantum;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[ExecuteInEditMode]
public class SaberFrameTool : MonoBehaviour
{
    [PropertyOrder(1)] public Transform SaberTransform;
    [PropertyOrder(2)] public Transform PoleLPosition;
    [PropertyOrder(3)] public Transform PoleRPosition;
    [PropertyOrder(4)] public SaberAnimationData Animation;
    [PropertyOrder(7)] public List<SaberAnimationFrame> Frames;
    [PropertyOrder(9)] public SaberAnimationFrame CurrentFrame;

    [PropertyOrder(13)] [Header("Playback")]
    public bool Loop;

    private int frameIndex;
    private bool enablePlayback;
    private bool isPlaying;
    private float playingTime;

    void Update()
    {
        if (isPlaying)
        {
            playingTime += Time.deltaTime * 60;
        }

        enablePlayback = Application.IsPlaying(this);

        if (!enablePlayback) return;

        var frame = Mathf.FloorToInt(playingTime) + 1;

        if (Loop)
        {
            frame &= Frames[^1].StartEndFrame.Y;
        }

        for (int i = 0; i < Frames.Count; i++)
        {
            if (frame >= Frames[i].StartEndFrame.X && frame <= Frames[i].StartEndFrame.Y)
            {
                CurrentFrame = Frames[i];
                frameIndex = i;
                break;
            }
        }

        SetLocalTransform();
    }

    private void SetLocalTransform()
    {
        var pos = CurrentFrame.Position.ToUnityVector3();
        var rotEuler = CurrentFrame.Rotation.ToUnityVector3();

        var lPolePos = CurrentFrame.PoleLPosition.ToUnityVector3();
        var rPolePos = CurrentFrame.PoleRPosition.ToUnityVector3();

        SaberTransform.localPosition = pos;
        SaberTransform.localRotation = Quaternion.Euler(rotEuler);
        PoleLPosition.localPosition = lPolePos;
        PoleRPosition.localPosition = rPolePos;
    }

    [PropertyOrder(5)]
    [Button("Get Frames From Animation")]
    private void GetFramesFromAnimation()
    {
        foreach (var frame in Animation.Frames)
        {
            Frames.Add(frame);
        }
    }

    [PropertyOrder(6)]
    [Button("Apply Frames To Animation")]
    private void ApplyFramesToAnimation()
    {
        EditorUtility.SetDirty(Animation);
        Animation.Frames = Frames.ToArray();
    }

    [PropertyOrder(8)]
    [Button("Create Frame")]
    private void CreateFrame()
    {
        var frame = new SaberAnimationFrame();

        frame.Position = SaberTransform.localPosition.ToFPVector3();
        frame.Rotation = SaberTransform.localRotation.ToFPQuaternion().AsEuler;

        frame.PoleLPosition = PoleLPosition.localPosition.ToFPVector3();
        frame.PoleRPosition = PoleRPosition.localPosition.ToFPVector3();

        Frames.Add(frame);
        frameIndex = Frames.Count - 1;
        CurrentFrame = Frames[frameIndex];
    }

    [PropertyOrder(10)]
    [Button("Select Next Frame")]
    private void SelectNextFrame()
    {
        frameIndex++;
        frameIndex = Mathf.Clamp(frameIndex, 0, Frames.Count - 1);

        CurrentFrame = Frames[frameIndex];
        SetLocalTransform();
    }

    [PropertyOrder(11)]
    [Button("Select Previous Frame")]
    private void SelectPreviousFrame()
    {
        frameIndex--;
        frameIndex = Mathf.Clamp(frameIndex, 0, Frames.Count - 1);

        CurrentFrame = Frames[frameIndex];
        SetLocalTransform();
    }

    [PropertyOrder(12)]
    [Button("Apply Transform To Current Frame")]
    private void ApplyTransformToCurrentFrame()
    {
        CurrentFrame.Position = SaberTransform.localPosition.ToFPVector3();
        CurrentFrame.Rotation = SaberTransform.localRotation.eulerAngles.ToFPVector3();

        CurrentFrame.PoleLPosition = PoleLPosition.localPosition.ToFPVector3();
        CurrentFrame.PoleRPosition = PoleRPosition.localPosition.ToFPVector3();

        Frames[frameIndex] = CurrentFrame;
    }

    [PropertyOrder(14)]
    [Button("Play Animation")]
    private void PlayAnimation()
    {
        isPlaying = true;
    }

    [PropertyOrder(15)]
    [Button("Stop Animation")]
    private void StopAnimation()
    {
        isPlaying = false;
        playingTime = 0f;
    }
}
#endif