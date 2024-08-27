using System.Numerics;
using Content.Client.Eui;
using Content.Shared._RMC14.Admin;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Vendors;
using Content.Shared.Eui;
using Content.Shared.Humanoid.Prototypes;
using JetBrains.Annotations;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;
using static Robust.Client.UserInterface.Controls.BaseButton;
using static Robust.Client.UserInterface.Controls.ItemList;
using static Robust.Client.UserInterface.Controls.LineEdit;

namespace Content.Client._RMC14.Admin;

[UsedImplicitly]
public sealed class RMCAdminEui : BaseEui
{
    [Dependency] private readonly IComponentFactory _compFactory = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;

    private static readonly Comparer<EntityPrototype> EntityComparer =
        Comparer<EntityPrototype>.Create(static (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

    private static readonly Comparer<SpeciesPrototype> SpeciesComparer =
        Comparer<SpeciesPrototype>.Create(static (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

    private RMCAdminWindow _adminWindow = default!;
    private bool _isFirstState = true;

    public override void Opened()
    {
        _adminWindow = new RMCAdminWindow();

        var humanoidRow = new RMCTransformRow();
        humanoidRow.Label.Text = Loc.GetString("rmc-ui-humanoid");
        _adminWindow.TransformTab.Container.AddChild(humanoidRow);

        var allSpecies = new SortedSet<SpeciesPrototype>(SpeciesComparer);
        foreach (var entity in _prototypes.EnumeratePrototypes<SpeciesPrototype>())
        {
            if (!entity.RoundStart)
                continue;

            allSpecies.Add(entity);
        }

        foreach (var species in allSpecies)
        {
            var button = new RMCTransformButton { Type = TransformType.Humanoid };
            button.TransformName.Text = Loc.GetString(species.Name);
            button.OnPressed += _ => SendMessage(new RMCAdminTransformHumanoidMsg(species.ID));

            humanoidRow.Container.AddChild(button);
        }

        var tiers = new SortedDictionary<int, SortedSet<EntityPrototype>>();
        foreach (var entity in _prototypes.EnumeratePrototypes<EntityPrototype>())
        {
            if (entity.Abstract || !entity.TryGetComponent(out XenoComponent? xeno, _compFactory))
                continue;

            if (!tiers.TryGetValue(xeno.Tier, out var xenos))
            {
                xenos = new SortedSet<EntityPrototype>(EntityComparer);
                tiers.Add(xeno.Tier, xenos);
            }

            xenos.Add(entity);
        }

        foreach (var (tier, xenos) in tiers)
        {
            var row = new RMCTransformRow();
            row.Label.Text = Loc.GetString("rmc-ui-tier", ("tier", tier));
            foreach (var xeno in xenos)
            {
                var button = new RMCTransformButton { Type = TransformType.Xeno };
                button.TransformName.Text = xeno.Name;
                row.Container.AddChild(button);

                button.OnPressed += _ => SendMessage(new RMCAdminTransformXenoMsg(xeno.ID));
            }

            _adminWindow.TransformTab.Container.AddChild(row);
        }

        _adminWindow.MarineTab.PointsSpinBox.ValueChanged +=
            args => SendMessage(new RMCAdminSetVendorPointsMsg(args.Value));

        _adminWindow.MarineTab.SpecialistPointsSpinBox.ValueChanged +=
            args => SendMessage(new RMCAdminSetSpecialistVendorPointsMsg(args.Value));

        _adminWindow.OpenCentered();
    }

        _adminWindow.MarineTab.SpecialistSkills.DisposeAllChildren();
        foreach (var comp in s.SpecialistSkills)
        {
            var specButton = new Button
            {
                Text = comp.Name,
                ToggleMode = true,
                StyleClasses = { "OpenBoth" },
            };

            specButton.Pressed = comp.Present;
            specButton.OnPressed += args =>
            {
                if (args.Button.Pressed)
                    SendMessage(new RMCAdminAddSpecSkillMsg(comp.Name));
                else
                    SendMessage(new RMCAdminRemoveSpecSkillMsg(comp.Name));
            };

            _adminWindow.MarineTab.SpecialistSkills.AddChild(specButton);
        }

        // TODO RMC14 if we don't do this the value jitters a lot, would be better to refresh on unfocus but i aint got time for that
        if (_isFirstState)
        {
            _adminWindow.MarineTab.PointsSpinBox.OverrideValue(s.Points);

            var specialistPoints = s.ExtraPoints.GetValueOrDefault(SharedCMAutomatedVendorSystem.SpecialistPoints);
            _adminWindow.MarineTab.SpecialistPointsSpinBox.OverrideValue(specialistPoints);
        }

        _adminWindow.MarineTab.Squads.DisposeAllChildren();
        foreach (var squad in s.Squads)
        {
            var squadRow = new RMCSquadRow()
            {
                HorizontalExpand = true,
                Margin = new Thickness(0, 0, 0, 10),
            };

            squadRow.AddToSquadButton.OnPressed += _ => SendMessage(new RMCAdminAddToSquadMsg(squad.Id));

            var squadName = string.Empty;
            var color = Color.White;
            if (_prototypes.TryIndex(squad.Id, out var squadPrototype))
            {
                squadName = squadPrototype.Name;

                if (squadPrototype.TryGetComponent(out SquadTeamComponent? squadComp, _compFactory))
                    color = squadComp.Color;
            }

            squadRow.CreateSquadButton(
                squad.Exists,
                () => SendMessage(new RMCAdminCreateSquadMsg(squad.Id)),
                squad.Members,
                squadName,
                color
            );

            _adminWindow.MarineTab.Squads.AddChild(squadRow);
        }

        _isFirstState = false;
    }

    public override void Closed()
    {
        _adminWindow.Dispose();
    }
}
