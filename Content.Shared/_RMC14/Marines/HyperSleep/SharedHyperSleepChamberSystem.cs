using Content.Shared.Movement.Events;
using Content.Shared._RMC14.Xenonids;
using Robust.Shared.Containers;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Marines.HyperSleep;

public abstract class SharedHyperSleepChamberSystem : EntitySystem
{
    [Dependency] private readonly SharedContainerSystem _containers = default!;
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private readonly HashSet<EntityUid> _intersecting = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<HyperSleepChamberComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<HyperSleepChamberComponent, ContainerIsInsertingAttemptEvent>(OnInsertAttempt);
        SubscribeLocalEvent<HyperSleepChamberComponent, EntInsertedIntoContainerMessage>(OnInserted);

        SubscribeLocalEvent<InsideHyperSleepChamberComponent, MoveInputEvent>(OnMoveInput);

        SubscribeLocalEvent<OutsideHyperSleepChamberComponent, PreventCollideEvent>(OnPreventCollide);
    }

    private void OnMapInit(Entity<HyperSleepChamberComponent> ent, ref MapInitEvent args)
    {
        _containers.EnsureContainer<Container>(ent, ent.Comp.ContainerId);
    }

    private void OnInsertAttempt(Entity<HyperSleepChamberComponent> ent, ref ContainerIsInsertingAttemptEvent args)
    {
        if (HasComp<XenoComponent>(args.EntityUid))
        {
            args.Cancel();
            return;
        }
    }

    private void OnInserted(Entity<HyperSleepChamberComponent> ent, ref EntInsertedIntoContainerMessage args)
    {
        if (!_timing.ApplyingState)
            EnsureComp<InsideHyperSleepChamberComponent>(args.Entity).Chamber = ent;
    }

    private void OnMoveInput(Entity<InsideHyperSleepChamberComponent> ent, ref MoveInputEvent args)
    {
        if (_timing.ApplyingState)
            return;

        if (ent.Comp.Chamber is not { } chamber)
            return;

        RemCompDeferred<InsideHyperSleepChamberComponent>(ent);

        var outside = EnsureComp<OutsideHyperSleepChamberComponent>(ent);
        outside.Chamber = chamber;
        Dirty(ent, outside);
    }

    private void OnPreventCollide(Entity<OutsideHyperSleepChamberComponent> ent, ref PreventCollideEvent args)
    {
        if (ent.Comp.Chamber == args.OtherEntity)
            args.Cancelled = true;
    }

    public override void Update(float frameTime)
    {
        var query = EntityQueryEnumerator<OutsideHyperSleepChamberComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (comp.Chamber is not { } chamber)
            {
                RemCompDeferred<OutsideHyperSleepChamberComponent>(uid);
                continue;
            }

            _intersecting.Clear();
            _entityLookup.GetEntitiesIntersecting(uid, _intersecting);

            if (!_intersecting.Contains(chamber))
                RemCompDeferred<OutsideHyperSleepChamberComponent>(uid);
        }
    }
}
