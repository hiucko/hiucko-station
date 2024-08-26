﻿using System.Collections.Immutable;
using Content.Shared.Coordinates;
using Content.Shared.Directions;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Tag;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Map;

public sealed class RMCMapSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly TurfSystem _turf = default!;

    private static readonly ProtoId<TagPrototype> StructureTag = "Structure";

    private EntityQuery<MapGridComponent> _mapGridQuery;

    public readonly ImmutableArray<Direction> CardinalDirections = ImmutableArray.Create(
        Direction.South,
        Direction.East,
        Direction.North,
        Direction.West
    );

    public override void Initialize()
    {
        _mapGridQuery = GetEntityQuery<MapGridComponent>();
    }

    public RMCAnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(EntityUid ent, Direction? offset = null, DirectionFlag facing = DirectionFlag.None)
    {
        return GetAnchoredEntitiesEnumerator(ent.ToCoordinates(), offset, facing);
    }

    public RMCAnchoredEntitiesEnumerator GetAnchoredEntitiesEnumerator(EntityCoordinates coords, Direction? offset = null, DirectionFlag facing = DirectionFlag.None)
    {
        if (_transform.GetGrid(coords) is not { } gridId ||
            !_mapGridQuery.TryComp(gridId, out var gridComp))
        {
            return RMCAnchoredEntitiesEnumerator.Empty;
        }

        if (offset != null)
            coords = coords.Offset(offset.Value);

        var indices = _map.CoordinatesToTile(gridId, gridComp, coords);
        var anchored = _map.GetAnchoredEntitiesEnumerator(gridId, gridComp, indices);
        return new RMCAnchoredEntitiesEnumerator(_transform, anchored, facing);
    }

    public bool HasAnchoredEntityEnumerator<T>(EntityCoordinates coords, Direction? offset = null, DirectionFlag facing = DirectionFlag.None) where T : IComponent
    {
        var anchored = GetAnchoredEntitiesEnumerator(coords, offset, facing);
        while (anchored.MoveNext(out var uid))
        {
            if (HasComp<T>(uid))
                return true;
        }

        return false;
    }

    public bool TryGetTileRefForEnt(EntityUid ent, out Entity<MapGridComponent> grid, out TileRef tile)
    {
        grid = default;
        tile = default;
        if (_transform.GetGrid(ent) is not { } gridId ||
            !_mapGridQuery.TryComp(ent, out var gridComp))
        {
            return false;
        }

        var coords = _transform.GetMoverCoordinates(ent);
        grid = (gridId, gridComp);
        if (!_map.TryGetTileRef(gridId, gridComp, coords, out tile))
            return false;

        return true;
    }

    public bool IsTileBlocked(EntityCoordinates coordinates, CollisionGroup group = CollisionGroup.Impassable)
    {
        if (!coordinates.TryGetTileRef(out var turf, EntityManager, _mapManager))
            return false;

        return _turf.IsTileBlocked(turf.Value, group);
    }

    public bool TileHasStructure(EntityCoordinates coordinates)
    {
        var anchored = GetAnchoredEntitiesEnumerator(coordinates);
        while (anchored.MoveNext(out var uid))
        {
            if (_tag.HasTag(uid, StructureTag))
                return true;
        }

        return false;
    }
}
