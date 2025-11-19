namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class BoxSystem : SystemMainThreadFilter<BoxSystem.Filter>
    {
        public override void Update(Frame frame, ref Filter filter)
        {
            var rConstants = frame.FindAsset(filter.Ronin->Constants);
            var sConstants = frame.FindAsset(filter.Saber->Constants);
            
            var rCurrentState = frame.FindAsset(filter.Ronin->CurrentState);
            var rStateFrame = filter.Ronin->StateFrame;

            var sCurrentState = frame.FindAsset(filter.Saber->CurrentState);
            var sStateFrame = filter.Saber->StateFrame;
            
            var hitBoxes = frame.ResolveList<HitBox>(filter.Ronin->HitBoxes);
            var hurtBoxes = frame.ResolveList<HurtBox>(filter.Ronin->HurtBoxes);
            
            hitBoxes.Clear();
            hurtBoxes.Clear();

            if (rCurrentState is AttackStateBase)
            {
                var attack = rCurrentState as AttackStateBase;

                foreach (var data in attack.HitBoxes)
                {
                    if (rStateFrame >= data.StartEndFrame.X && rStateFrame <= data.StartEndFrame.Y)
                    {
                        var box = new HitBox()
                        {
                            Rect = ConvertRect(filter.Ronin, data.Rect)
                        };
                        
                        hitBoxes.Add(box);
                    }
                }
            }

            foreach (var data in rCurrentState.HurtBoxes)
            {
                if (data.AlwaysActive || rStateFrame >= data.StartEndFrame.X && rStateFrame <= data.StartEndFrame.Y)
                {
                    var rect = data.UseBaseRect ? rConstants.BaseRect : data.Rect;
                    var box = new HurtBox()
                    {
                        Rect = ConvertRect(filter.Ronin, rect),
                        IsSaber = false
                    };
                        
                    hurtBoxes.Add(box);
                }
            }

            if (frame.TryResolveList(filter.Saber->Direction.Boxes, out var currentBoxes))
            {
                foreach (var data in currentBoxes)
                {
                    var box = new HurtBox()
                    {
                        Rect = ConvertRect(filter.Ronin, data),
                        IsSaber = true
                    };

                    hurtBoxes.Add(box);
                }
            }
        }

        private BoxRect ConvertRect(RoninData* ronin, BoxRect rect)
        {
            var sign = ronin->FacingSign;
            var newRect = new BoxRect()
            {
                Position = new FPVector2(ronin->Position.X + rect.Position.X * sign, //BIG MISTAKE HERE <<<<<<<<<<<<<<<<
                    ronin->Position.Y + rect.Position.Y),
                WidthHeight = rect.WidthHeight
            };

            return newRect;
        }

        public struct Filter
        {
            public EntityRef Entity;
            public PlayerData* Player;
            public RoninData* Ronin;
            public SaberData* Saber;
        }
    }
}
