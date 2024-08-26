﻿using Robust.Shared.GameStates;

namespace Content.Shared._RMC14.ShakeStun;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(StunShakeableSystem))]
public sealed partial class StunShakeableComponent : Component
{
    [DataField, AutoNetworkedField]
    public TimeSpan DurationRemoved = TimeSpan.FromSeconds(6);
}
