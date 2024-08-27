using Content.Shared.Administration;
using Content.Shared.Administration.Managers;
using Content.Shared.Verbs;
using Robust.Shared.Player;

namespace Content.Shared._RMC14.Admin;

public abstract class SharedRMCAdminSystem : EntitySystem
{
    [Dependency] private readonly ISharedAdminManager _admin = default!;

    protected bool CanUse(ICommonSession player)
    {
        return _admin.HasAdminFlag(player, AdminFlags.Debug);
    }

    protected virtual void OpenBui(ICommonSession player, EntityUid target)
    {
    }
}
