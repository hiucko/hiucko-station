using System.Numerics;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.Player;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;

namespace Content.Client.Movement.Systems;

public sealed class MobCollisionSystem : SharedMobCollisionSystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _player = default!;

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_timing.IsFirstTimePredicted)
            return;

        // TODO: Try running these in updatebeforesolve and storing for later
        var player = _player.LocalEntity;

        if (!MobQuery.TryComp(player, out var comp) || !TryComp(player, out PhysicsComponent? physics))
            return;

        // TODO: For testing
        Physics.WakeBody(player.Value, body: physics);

        if (!HandleCollisions((player.Value, comp, physics), frameTime))
        {
            comp.EndAccumulator -= frameTime;

            if (comp.EndAccumulator <= 0)
            {
                SetColliding((player.Value, comp), value: false, update: true);
                comp.EndAccumulator = 0f;
            }
        }
        else
        {
            SetColliding((player.Value, comp), value: true, update: true);
            comp.EndAccumulator = MobCollisionComponent.BufferTime;
        }
    }

    protected override void RaiseCollisionEvent(EntityUid uid, Vector2 direction)
    {
        RaisePredictiveEvent(new MobCollisionMessage()
        {
            Direction = direction,
        });
    }
}
