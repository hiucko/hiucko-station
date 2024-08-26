﻿using Content.Shared.Damage;
using Content.Shared.FixedPoint;
using Content.Shared._RMC14.Armor;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using Robust.Shared.Audio.Systems;

namespace Content.Shared._RMC14.Shields;

public sealed partial class XenoShieldSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    public enum ShieldType
    {
        Generic,
        Ravager,
        Hedgehog,
        Vanguard,
        Praetorian,
        Crusher,
        Warden,
        Gardener,
        ShieldPillar,
        CumulativeGeneric
    }

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoShieldComponent, DamageModifyEvent>(OnDamage, after: [typeof(CMArmorSystem)]);
    }

    public void OnDamage(Entity<XenoShieldComponent> ent, ref DamageModifyEvent args)
    {
        if (!ent.Comp.Active)
            return;

        if (!args.Damage.AnyPositive())
            return;

        ent.Comp.ShieldAmount -= args.Damage.GetTotal();

        if (ent.Comp.ShieldAmount <= 0)
        {
            foreach (var type in args.Damage.DamageDict)
            {
                if (ent.Comp.ShieldAmount == 0)
                    break;

                if (type.Value > 0)
                {
                    var tempVal = Math.Min(type.Value.Double(), -ent.Comp.ShieldAmount.Double());
                    args.Damage.DamageDict[type.Key] -= tempVal;
                    ent.Comp.ShieldAmount += tempVal;
                }
            }

            _audio.PlayPredicted(ent.Comp.ShieldBreak, ent, null);
            RemoveShield(ent, ent.Comp.Shield);
        }
        else
            args.Damage.ClampMax(0);

        Dirty(ent, ent.Comp);
    }

    public void ApplyShield(EntityUid uid, ShieldType type, FixedPoint2 amount, TimeSpan? duration = null,
        double decay = 0, bool addShield = false, double maxShield = 200)
    {
        if (!EnsureComp<XenoShieldComponent>(uid, out var shieldComp))
            return;

        if (shieldComp.Active && shieldComp.Shield == type)
        {
            if (addShield)
                shieldComp.ShieldAmount = Math.Min((shieldComp.ShieldAmount + amount).Double(), maxShield);
            else
                shieldComp.ShieldAmount = Math.Max(shieldComp.ShieldAmount.Double(), amount.Double());

            return;
        }

        RemoveShield(uid, shieldComp.Shield);

        shieldComp.Shield = type;
        shieldComp.ShieldAmount = amount;
        shieldComp.Duration = duration;
        shieldComp.DecayPerSecond = decay;

        if (duration != null)
            shieldComp.ShieldDecayAt = _timing.CurTime + duration.Value;

        shieldComp.Active = true;

		Dirty(uid, shieldComp);
	}

    public void RemoveShield(EntityUid uid, ShieldType shieldType)
    {
        if (!TryComp<XenoShieldComponent>(uid, out var shieldComp))
            return;

        if (!shieldComp.Active)
            return;

        if (shieldComp.Shield == shieldType)
        {
            shieldComp.Active = false;
            shieldComp.ShieldAmount = 0;
            Dirty(uid, shieldComp);
            var ev = new RemovedShieldEvent(shieldType);
            RaiseLocalEvent(uid, ref ev);
        }
    }

    public override void Update(float frameTime)
    {
        if (_net.IsClient)
            return;

        var time = _timing.CurTime;

        var shieldQuery = EntityQueryEnumerator<XenoShieldComponent>();
        while (shieldQuery.MoveNext(out var uid, out var shield))
        {
            if (shield.Duration == null)
                continue;

            if (time < shield.ShieldDecayAt)
                continue;

            shield.ShieldAmount -= shield.DecayPerSecond * frameTime;

            if (shield.ShieldAmount <= 0)
            {
                RemoveShield(uid, shield.Shield);
                continue;
            }

            Dirty(uid, shield);
        }
    }
}
