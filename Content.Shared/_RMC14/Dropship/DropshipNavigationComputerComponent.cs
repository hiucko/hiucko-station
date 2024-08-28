using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Dropship;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedDropshipSystem))]
public sealed partial class DropshipNavigationComputerComponent : Component
{
    [DataField, AutoNetworkedField]
    public int foo = 2; //не удалять же бебебе
}
