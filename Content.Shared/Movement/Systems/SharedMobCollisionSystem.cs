using System.Numerics;
using Content.Shared.Movement.Components;
using Content.Shared.Physics;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Movement.Systems;

public abstract class SharedMobCollisionSystem : EntitySystem
{
    [Dependency] private readonly FixtureSystem _fixtures = default!;
    [Dependency] private readonly RayCastSystem _rayCast = default!;
    [Dependency] protected readonly SharedPhysicsSystem Physics = default!;
    [Dependency] private readonly SharedTransformSystem _xformSystem = default!;

    protected EntityQuery<MobCollisionComponent> MobQuery;
    protected EntityQuery<PhysicsComponent> PhysicsQuery;

    /*
     * Looks like movespeed not panning out?
     * Also try hard contact but ignore it on server and have client handle it, but that's just clientside movement KEKW
     *
     * Push away version somewhat works but only for 30tps not 60tps
     * Also try old pushing with KC / clientside movement I think.
     */

    public override void Initialize()
    {
        base.Initialize();
        MobQuery = GetEntityQuery<MobCollisionComponent>();
        PhysicsQuery = GetEntityQuery<PhysicsComponent>();
        SubscribeAllEvent<MobCollisionMessage>(OnCollision);
        SubscribeAllEvent<MobCollisionToggleMessage>(OnCollisionToggle);
        SubscribeLocalEvent<MobCollisionComponent, ComponentStartup>(OnCollisionStartup);
        SubscribeLocalEvent<MobCollisionComponent, RefreshMovementSpeedModifiersEvent>(OnMoveSpeed);

        UpdatesAfter.Add(typeof(SharedPhysicsSystem));
    }

    private void OnCollisionToggle(MobCollisionToggleMessage msg, EntitySessionEventArgs args)
    {
        var player = args.SenderSession.AttachedEntity;

        if (!MobQuery.TryComp(player, out var comp))
            return;

        SetColliding((player.Value, comp), value: msg.Enabled, update: false);
    }

    private void OnCollision(MobCollisionMessage msg, EntitySessionEventArgs args)
    {
        var player = args.SenderSession.AttachedEntity;

        if (!MobQuery.TryComp(player, out var comp))
            return;

        // TODO: Validation
        MoveMob((player.Value, comp), msg.Direction);
    }

    private void OnMoveSpeed(Entity<MobCollisionComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
    {
        if (!ent.Comp.Colliding)
            return;

        args.ModifySpeed(0.25f);
    }

    private void OnCollisionStartup(Entity<MobCollisionComponent> ent, ref ComponentStartup args)
    {
        _fixtures.TryCreateFixture(ent.Owner,
            ent.Comp.Shape,
            "mob_collision",
            hard: false,
            collisionLayer: (int) CollisionGroup.MidImpassable,
            collisionMask: (int) CollisionGroup.MidImpassable);
    }

    protected void SetColliding(Entity<MobCollisionComponent> entity, bool value, bool update = false)
    {
        return;
        if (entity.Comp.Colliding == value)
            return;

        entity.Comp.Colliding = value;
        Dirty(entity);
        //_moveSpeed.RefreshMovementSpeedModifiers(entity.Owner);

        if (!update)
            return;

        if (IoCManager.Resolve<INetManager>().IsClient)
        {
            RaisePredictiveEvent(new MobCollisionToggleMessage()
            {
                Enabled = value,
            });
        }
        else
        {
            RaiseLocalEvent(entity.Owner, new MobCollisionToggleMessage()
            {
                Enabled = value,
            });
        }
    }

    protected void MoveMob(Entity<MobCollisionComponent> entity, Vector2 direction)
    {
        var xform = Transform(entity.Owner);

        // TODO: Raycast to the specified spot so we don't clip into a wall.
        // TODO: Does wakebody break this???
        Physics.WakeBody(entity.Owner);

        _xformSystem.SetLocalPosition(entity.Owner, xform.LocalPosition + direction);
    }

    protected bool HandleCollisions(Entity<MobCollisionComponent, PhysicsComponent> entity, float frameTime)
    {
        var physics = entity.Comp2;

        //if (physics.LinearVelocity == Vector2.Zero)
        //    return;

        if (physics.ContactCount == 0)
            return false;

        var xform = Transform(entity.Owner);
        var (worldPos, worldRot) = _xformSystem.GetWorldPositionRotation(entity.Owner);
        var ourTransform = new Transform(worldPos, worldRot);
        var contacts = Physics.GetContacts(entity.Owner);
        var direction = Vector2.Zero;
        var contactCount = 0;

        while (contacts.MoveNext(out var contact))
        {
            if (!contact.IsTouching)
                continue;

            var other = contact.OtherEnt(entity.Owner);

            if (!MobQuery.TryComp(other, out var otherComp))
                continue;

            // TODO: Get overlap amount
            var otherTransform = Physics.GetPhysicsTransform(other);
            var diff = ourTransform.Position - otherTransform.Position;
            var penDepth = MathF.Max(0f, 0.6f - diff.Length());

            //penDepth = MathF.Pow(penDepth, 1.2f);

            // Sum the strengths so we get pushes back the same amount (impulse-wise, ignoring prediction).
            var mobMovement = penDepth * diff.Normalized() * (entity.Comp1.Strength + otherComp.Strength);

            // Need the push strength proportional to penetration depth.
            direction += mobMovement;
            contactCount++;
        }

        if (direction == Vector2.Zero)
        {
            return contactCount > 0;
        }

        direction *= frameTime;
        entity.Comp1.EndAccumulator = MobCollisionComponent.BufferTime;
        var parentAngle = worldRot - xform.LocalRotation;
        var localDir = (-parentAngle).RotateVec(direction);
        RaiseCollisionEvent(entity.Owner, localDir);
        return true;
    }

    protected abstract void RaiseCollisionEvent(EntityUid uid, Vector2 direction);

    [Serializable, NetSerializable]
    protected sealed class MobCollisionMessage : EntityEventArgs
    {
        public Vector2 Direction;
    }

    [Serializable, NetSerializable]
    protected sealed class MobCollisionToggleMessage : EntityEventArgs
    {
        public bool Enabled;
    }
}
