namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine.Scripting;
    using Quantum.Collections;

    [Preserve]
    public unsafe class HitRegSystem : SystemMainThread
    {
        public override void Update(Frame frame)
        {
            var e1 = frame.Global->Ronin1;
            var e2 = frame.Global->Ronin2;
            
            // try a context approach

            var c1 = SetupContext(frame, e1);
            var c2 = SetupContext(frame, e2);

            var c1Result = GetCombatResult(frame, c1, c2);
            var c2Result = GetCombatResult(frame, c2, c1);
            
            ParseCombatResult(frame, c1Result);
            ParseCombatResult(frame, c2Result);
        }

        private void ParseCombatResult(Frame frame, CombatResult result)
        {
            if (result.Type == CombatResultType.None)
                return;
            
            frame.Signals.OnHit(result.Attacker, result);
            frame.Signals.OnReceivedHit(result.Defender, result);

            frame.Events.OnHit(result.Attacker, result);
            frame.Events.OnReceivedHit(result.Defender, result);
        }

        private CombatContext SetupContext(Frame frame, EntityRef entity)
        {
            var context = new CombatContext()
            {
                Entity = entity,
            };

            var ronin = frame.Unsafe.GetPointer<RoninData>(entity);
            var saber = frame.Unsafe.GetPointer<SaberData>(entity);

            var roninState = frame.FindAsset(ronin->CurrentState);
            context.IsAttacking = roninState is AttackStateBase;
            if (context.IsAttacking)
            {
                var attackState = roninState as AttackStateBase;
                context.AttackState = attackState;
            }
            
            var saberState = frame.FindAsset(saber->CurrentState);
            context.IsDeflecting = saberState.BlockType == SaberBlockType.Deflect;
            if (context.IsDeflecting)
            {
                context.SaberState = saberState;
            }

            return context;
        }

        private CombatResult GetCombatResult(Frame frame, CombatContext cAttacker, CombatContext cDefender)
        {
            var result = new CombatResult()
            {
                Type = CombatResultType.None,
                Attacker = cAttacker.Entity,
                Defender = cDefender.Entity
            };

            var attRonin = frame.Unsafe.GetPointer<RoninData>(cAttacker.Entity);
            var defRonin = frame.Unsafe.GetPointer<RoninData>(cDefender.Entity);

            if (!cAttacker.IsAttacking)
                return result;

            if (attRonin->HasHit)
                return result;
            
            var attAttackState = frame.FindAsset(cAttacker.AttackState);
            var attHitBoxes = frame.ResolveList(attRonin->HitBoxes);
            var defHurtBoxes = frame.ResolveList(defRonin->HurtBoxes);

            if (attHitBoxes.Count == 0)
                return result;

            if (!CheckHit(attHitBoxes, defHurtBoxes, out var midPoint, out var saberHit))
                return result;

            result.Type = CombatResultType.Hit;
            result.AttackState = attAttackState;
            result.MidPoint = midPoint;
            result.SaberHit = saberHit;
            attRonin->HasHit = true;
            
            var defSaber = frame.Unsafe.GetPointer<SaberData>(cDefender.Entity);

            if (frame.TryFindAsset(cDefender.SaberState, out var defSaberState))
            {
                if (defSaberState.BlockType == SaberBlockType.Deflect)
                {
                    bool deflected = false;

                    switch (attAttackState.Height)
                    {
                        case AttackHeight.High:
                            if (defRonin->TargetingSign == defRonin->FacingSign)
                            {
                                deflected = defSaber->Direction.Id == SaberDirection.FwHigh;
                            }
                            else
                            {
                                deflected = defSaber->Direction.Id == SaberDirection.BwHigh;
                            }

                            break;

                        case AttackHeight.Mid:
                            if (defRonin->TargetingSign == defRonin->FacingSign)
                            {
                                deflected = defSaber->Direction.Id == SaberDirection.FwMid;
                            }
                            else
                            {
                                deflected = defSaber->Direction.Id == SaberDirection.BwMid;
                            }

                            break;

                        case AttackHeight.Low:
                            if (defRonin->TargetingSign == defRonin->FacingSign)
                            {
                                deflected = defSaber->Direction.Id == SaberDirection.FwLow;
                            }
                            else
                            {
                                deflected = defSaber->Direction.Id == SaberDirection.BwLow;
                            }

                            break;
                    }

                    if (deflected)
                    {
                        result.Type = CombatResultType.Deflected;
                    }
                    else if (saberHit)
                    {
                        result.Type = CombatResultType.None;
                    }

                    return result;
                }
            }

            if (cDefender.IsAttacking)
            {
                // check if clashing or something
            }

            if (saberHit)
            {
                result.Type = CombatResultType.None;
            }
            
            return result;
        }

        private bool CheckHit(QList<HitBox> hitBoxes, QList<HurtBox> hurtBoxes, out FPVector2 midPoint, out bool saberHit)
        {
            foreach (var hitBox in hitBoxes)
            {
                foreach (var hurtBox in hurtBoxes)
                {
                    if (hitBox.Rect.Overlaps(hurtBox.Rect))
                    {
                        midPoint = hitBox.Rect.GetOverlapCenter(hurtBox.Rect);
                        saberHit = hurtBox.IsSaber;
                        return true;
                    }
                }
            }
            
            midPoint = FPVector2.Zero;
            saberHit = false;
            return false;
        }
    }
}
