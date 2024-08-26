﻿using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Whitelist;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;

namespace Content.Shared._RMC14.Projectiles;

public sealed class RMCProjectileSystem : EntitySystem
{
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<DeleteOnCollideComponent, StartCollideEvent>(OnDeleteOnCollideStartCollide);
        SubscribeLocalEvent<ModifyTargetOnHitComponent, ProjectileHitEvent>(OnModifyTargetOnHit);
        SubscribeLocalEvent<ProjectileMaxRangeComponent, MapInitEvent>(OnProjectileMaxRangeMapInit);

        SubscribeLocalEvent<SpawnOnTerminateComponent, MapInitEvent>(OnSpawnOnTerminatingMapInit);
        SubscribeLocalEvent<SpawnOnTerminateComponent, EntityTerminatingEvent>(OnSpawnOnTerminatingTerminate);
    }

    private void OnDeleteOnCollideStartCollide(Entity<DeleteOnCollideComponent> ent, ref StartCollideEvent args)
    {
        if (_net.IsServer)
            QueueDel(ent);
    }

    private void OnModifyTargetOnHit(Entity<ModifyTargetOnHitComponent> ent, ref ProjectileHitEvent args)
    {
        if (!_whitelist.IsWhitelistPassOrNull(ent.Comp.Whitelist, args.Target))
            return;

        if (ent.Comp.Add is { } add)
            EntityManager.AddComponents(args.Target, add);
    }

    private void OnProjectileMaxRangeMapInit(Entity<ProjectileMaxRangeComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.Origin = _transform.GetMoverCoordinates(ent);
        Dirty(ent);
    }

    private void OnSpawnOnTerminatingMapInit(Entity<SpawnOnTerminateComponent> ent, ref MapInitEvent args)
    {
        ent.Comp.Origin = _transform.GetMoverCoordinates(ent);
        Dirty(ent);
    }

    private void OnSpawnOnTerminatingTerminate(Entity<SpawnOnTerminateComponent> ent, ref EntityTerminatingEvent args)
    {
        if (_net.IsClient)
            return;

        if (!TryComp(ent, out TransformComponent? transform))
            return;

        if (TerminatingOrDeleted(transform.ParentUid))
            return;

        var coordinates = transform.Coordinates;
        if (ent.Comp.ProjectileAdjust &&
            ent.Comp.Origin is { } origin &&
            coordinates.TryDelta(EntityManager, _transform, origin, out var delta) &&
            delta.Length() > 0)
        {
            coordinates = coordinates.Offset(delta.Normalized() / -2);
        }

        SpawnAtPosition(ent.Comp.Spawn, coordinates);

        if (ent.Comp.Popup is { } popup)
            _popup.PopupCoordinates(Loc.GetString(popup), coordinates, ent.Comp.PopupType ?? PopupType.Small);
    }

    public void SetMaxRange(Entity<ProjectileMaxRangeComponent> ent, float max)
    {
        ent.Comp.Max = max;
        Dirty(ent);
    }

    public override void Update(float frameTime)
    {
        if (_net.IsClient)
            return;

        var maxQuery = EntityQueryEnumerator<ProjectileMaxRangeComponent>();
        while (maxQuery.MoveNext(out var uid, out var comp))
        {
            var coordinates = _transform.GetMoverCoordinates(uid);
            if (comp.Origin is not { } origin ||
                !coordinates.TryDistance(EntityManager, _transform, origin, out var distance))
            {
                QueueDel(uid);
                continue;
            }

            if (distance < comp.Max)
                continue;

            QueueDel(uid);
        }
    }
}
