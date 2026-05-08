namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class AnimationDataSystem : SystemMainThreadFilter<AnimationDataSystem.Filter>
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            // GetSaberAnimation(frame, ref filter);
            // GetRoninAnimation(frame, ref filter);
        }
        //
        // private void GetUpperAnimation(Frame frame, ref Filter filter)
        // {
        //     var saberState = frame.FindAsset(filter.Saber->CurrentState);
        //     var animID = saberState.GetAnimationID(frame, filter.Entity);
        //     
        //     filter.AnimData->UpperAnimationID = animID;
        //     filter.AnimData->UpperAnimationFrameIndex = GetUpperFrameIndex(frame, ref filter);
        // }
        //
        // private void GetLowerAnimation(Frame frame, ref Filter filter)
        // {
        //     var roninState = frame.FindAsset(filter.Ronin->CurrentState);
        //     var animID = roninState.GetAnimationID(frame, filter.Entity);
        //
        //     filter.Ronin->CurrentRoninAnimationID = animID;
        //     filter.Ronin->CurrentRoninAnimationFrameIndex = GetRoninFrameIndex(frame, ref filter);
        // }
        //
        // private int GetRoninFrameIndex(Frame frame, ref Filter filter)
        // {
        //     var ronin = filter.Ronin;
        //     var roninConstants = frame.FindAsset(ronin->Constants);
        //     var stateFrame = ronin->StateContext.StateFrame;
        //
        //     if (!roninConstants.RoninAnimations.TryGetValue(ronin->CurrentRoninAnimationID, out var currentAnimation))
        //         return 0;
        //
        //     var frames = currentAnimation.Frames;
        //     var frameIndex = frames.Length - 1;
        //
        //     if (currentAnimation.IsLoop)
        //         stateFrame &= frames[^1].StartEndFrame.Y;
        //
        //     for (var i = 0; i < frames.Length; i++)
        //     {
        //         if (stateFrame >= frames[i].StartEndFrame.X && stateFrame <= frames[i].StartEndFrame.Y)
        //         {
        //             frameIndex = i;
        //             break;
        //         }
        //     }
        //
        //     return frameIndex;
        // }
        //
        // private int GetUpperFrameIndex(Frame frame, ref Filter filter)
        // {
        //     var saber = filter.Saber;
        //     var saberConstants = frame.FindAsset(saber->Constants);
        //     var stateFrame = saber->StateContext.StateFrame;
        //
        //     if (!saberConstants.SaberAnimations.TryGetValue(filter.Saber->CurrentAnimationID, out var currentAnimation))
        //     {
        //         Log.Debug("failed to find " + filter.Saber->CurrentAnimationID + " in constants animation dictionary");
        //         return 0;
        //     }
        //     
        //     //Log.Debug(currentAnimation.ID + ", amount of frames: " + currentAnimation.Frames.Length);
        //
        //     var frames = currentAnimation.Frames;
        //     var frameIndex = frames.Length - 1;
        //
        //     if (currentAnimation.IsLoop)
        //     {
        //         stateFrame &= frames[^1].StartEndFrame.Y;
        //     }
        //
        //     for (var i = 0; i < frames.Length; i++)
        //     {
        //         var frameData = frames[i];
        //         if (stateFrame >= frameData.StartEndFrame.X && stateFrame <= frameData.StartEndFrame.Y)
        //         {
        //             frameIndex = i;
        //             break;
        //         }
        //     }
        //
        //     return frameIndex;
        // }
        //
        public struct Filter
        {
            public EntityRef Entity;
            public PlayerData* Player;
            public RoninData* Ronin;
            public SaberData* Saber;
        }
    }
}
