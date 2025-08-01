namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;

    [Preserve]
    public unsafe class CollisionSystem : SystemMainThread
    {
        public override void Update(Frame frame)
        {
            var ronin1 = frame.Unsafe.GetPointer<RoninData>(frame.Global->Ronin1);
            var ronin2 = frame.Unsafe.GetPointer<RoninData>(frame.Global->Ronin2);
            
            if (!ronin1->IgnoreCollision && !ronin2->IgnoreCollision) RoninCollision(frame, ronin1, ronin2);
        }

        private void RoninCollision(Frame frame, RoninData* r1, RoninData* r2)
        {
            var constants1 = frame.FindAsset(r1->Constants);
            var constants2 = frame.FindAsset(r2->Constants);

            var pos1 = r1->Position;
            var pos2 = r2->Position;

            var range1 = constants1.PushRange;
            var range2 = constants2.PushRange;

            var right1 = pos1.X + range1;
            var left1  = pos1.X - range1;
            var right2 = pos2.X + range2;
            var left2  = pos2.X - range2;

            if (right1 > left2 && pos1.X < pos2.X)
            {
                var overlap = right1 - left2;
                r1->Position.X -= overlap / 2;
                r2->Position.X += overlap / 2;
            }
            else if (right2 > left1 && pos1.X > pos2.X)
            {
                var overlap = right2 - left1;
                r1->Position.X += overlap / 2;
                r2->Position.X -= overlap / 2;
            }
        }
    }
}
