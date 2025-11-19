using UnityEngine;

namespace Quantum
{
    using Photon.Deterministic;

    public class GameConfig : AssetObject // Immutable global data
    {
        [Header("Entity Asset")]
        public AssetRef<EntityPrototype> BaseRonin;
        
        [Header("Ronin Setup")]
        public AssetRef<RoninConstants> BaseRoninConstants;
        public AssetRef<RoninStateBase> StartingRoninState;
        
        [Header("Saber Setup")]
        public AssetRef<SaberConstants> BaseSaberConstants;
        public AssetRef<SaberStateBase> StartingSaberState;
        public SaberConstants.SaberDirectionEditorData BaseSaberDirection;

        [Header("Ronin Base Values")] 
        public FP BaseDevotion = 50;
        public FP DevotionMax = 100;
        
        [Header("Stage Settings")]
        public FP StageWidth = 10;
        public FP MaxDistance = 5;
        public FP StartDistance = 2;

        [Header("Timers")]
        public int PreBattleTimeFrames = 180;
        public int BattleTimeFrames = 0;
        public int PostBattleTimeFrames = 300;
    }
}
