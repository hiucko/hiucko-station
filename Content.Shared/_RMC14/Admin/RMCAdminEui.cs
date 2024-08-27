using Content.Shared.Eui;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin;

[Serializable, NetSerializable]
public readonly record struct Squad(EntProtoId Id, bool Exists, int Members);

[Serializable, NetSerializable]
[Virtual]
public class RMCAdminEuiState(
    List<Squad> squads,
    int marines,
) : EuiStateBase
{
    public readonly List<Squad> Squads = squads;
    public readonly int Marines = marines;
}

[Serializable, NetSerializable]
public sealed class RMCAdminEuiTargetState(
    List<Squad> squads,
    int marines,
    List<(string Name, bool Present)> specialistSkills,
    int points,
    Dictionary<string, int> extraPoints
) : RMCAdminEuiState(squads, marines)
{
    public readonly List<(string Name, bool Present)> SpecialistSkills = specialistSkills;
    public readonly int Points = points;
    public readonly Dictionary<string, int> ExtraPoints = extraPoints;
}

[Serializable, NetSerializable]
public sealed class RMCAdminSetVendorPointsMsg(int points) : EuiMessageBase
{
    public readonly int Points = points;
}

[Serializable, NetSerializable]
public sealed class RMCAdminSetSpecialistVendorPointsMsg(int points) : EuiMessageBase
{
    public readonly int Points = points;
}

[Serializable, NetSerializable]
public sealed class RMCAdminAddSpecSkillMsg(string component) : EuiMessageBase
{
    public readonly string Component = component;
}

[Serializable, NetSerializable]
public sealed class RMCAdminRemoveSpecSkillMsg(string component) : EuiMessageBase
{
    public readonly string Component = component;
}

[Serializable, NetSerializable]
public sealed class RMCAdminCreateSquadMsg(EntProtoId squadId) : EuiMessageBase
{
    public readonly EntProtoId SquadId = squadId;
}

[Serializable, NetSerializable]
public sealed class RMCAdminAddToSquadMsg(EntProtoId squadId) : EuiMessageBase
{
    public readonly EntProtoId SquadId = squadId;
}

[Serializable, NetSerializable]
public sealed class RMCAdminRefresh : EuiMessageBase;
