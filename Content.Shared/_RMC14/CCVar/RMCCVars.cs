using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._RMC14.CCVar;

[CVarDefs]
public sealed class RMCCVars : CVars
{
    public static readonly CVarDef<bool> CMPlayVoicelinesArachnid =
        CVarDef.Create("rmc.play_voicelines_arachnid", true, CVar.REPLICATED | CVar.CLIENT | CVar.ARCHIVE);

    public static readonly CVarDef<bool> CMPlayVoicelinesDiona =
        CVarDef.Create("rmc.play_voicelines_diona", true, CVar.REPLICATED | CVar.CLIENT | CVar.ARCHIVE);

    public static readonly CVarDef<bool> CMPlayVoicelinesDwarf =
        CVarDef.Create("rmc.play_voicelines_dwarf", true, CVar.REPLICATED | CVar.CLIENT | CVar.ARCHIVE);

//    public static readonly CVarDef<bool> CMPlayVoicelinesFelinid =
//        CVarDef.Create("rmc.play_voicelines_felinid", true, CVar.REPLICATED | CVar.CLIENT | CVar.ARCHIVE);

    public static readonly CVarDef<bool> CMPlayVoicelinesHuman =
        CVarDef.Create("rmc.play_voicelines_human", true, CVar.REPLICATED | CVar.CLIENT | CVar.ARCHIVE);

    public static readonly CVarDef<bool> CMPlayVoicelinesMoth =
        CVarDef.Create("rmc.play_voicelines_moth", true, CVar.REPLICATED | CVar.CLIENT | CVar.ARCHIVE);

    public static readonly CVarDef<bool> CMPlayVoicelinesReptilian =
        CVarDef.Create("rmc.play_voicelines_reptilian", true, CVar.REPLICATED | CVar.CLIENT | CVar.ARCHIVE);

    public static readonly CVarDef<bool> CMPlayVoicelinesSlime =
        CVarDef.Create("rmc.play_voicelines_slime", true, CVar.REPLICATED | CVar.CLIENT | CVar.ARCHIVE);

    public static readonly CVarDef<int> CMMaxHeavyAttackTargets =
        CVarDef.Create("rmc.max_heavy_attack_targets", 3, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<float> CMBloodlossMultiplier =
        CVarDef.Create("rmc.bloodloss_multiplier", 1.5f, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<float> CMBleedTimeMultiplier =
        CVarDef.Create("rmc.bleed_time_multiplier", 1f, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<int> RMCPatronLobbyMessageTimeSeconds =
        CVarDef.Create("rmc.patron_lobby_message_time_seconds", 30, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<int> RMCPatronLobbyMessageInitialDelaySeconds =
        CVarDef.Create("rmc.patron_lobby_message_initial_delay_seconds", 5, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<string> RMCDiscordAccountLinkingMessageLink =
        CVarDef.Create("rmc.discord_account_linking_message_link", "", CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<int> RMCRequisitionsStartingBalance =
        CVarDef.Create("rmc.requisitions_starting_balance", 80000, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<int> RMCRequisitionsBalanceGain =
        CVarDef.Create("rmc.requisitions_balance_gain", 1500, CVar.REPLICATED | CVar.SERVER);
    /// <summary>
    ///     Comma-separated list of maps to load as the planet in the distress signal gamemode.
    /// </summary>
    public static readonly CVarDef<string> RMCPlanetMaps =
        CVarDef.Create("rmc.planet_maps", "/Maps/_RMC14/lv624.yml,/Maps/_RMC14/solaris.yml,/Maps/_RMC14/prison.yml,/Maps/_RMC14/shiva.yml", CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<bool> RMCDrawStorageIconLabels =
        CVarDef.Create("rmc.draw_storage_icon_labels", true, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<bool> RMCFTLCrashLand =
        CVarDef.Create("rmc.ftl_crash_land", true, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<float> RMCDropshipInitialDelayMinutes =
        CVarDef.Create("rmc.dropship_initial_delay_minutes", 15f, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<float> RMCLandingZonePrimaryAutoMinutes =
        CVarDef.Create("rmc.landing_zone_primary_auto_minutes", 25f, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<bool> RMCAtmosTileEqualize =
        CVarDef.Create("rmc.atmos_tile_equalize", false, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<bool> RMCGasTileOverlayUpdate =
        CVarDef.Create("rmc.gas_tile_overlay_update", false, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<bool> RMCActiveInputMoverEnabled =
        CVarDef.Create("rmc.active_input_mover_enabled", true, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<string> RMCAdminFaxAreaMap =
        CVarDef.Create("rmc.admin_fax_area_map", "Maps/_RMC14/admin_fax.yml", CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<int> RMCDropshipFabricatorStartingPoints =
        CVarDef.Create("rmc.dropship_fabricator_starting_points", 20000, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<float> RMCDropshipFabricatorGainEverySeconds =
        CVarDef.Create("rmc.dropship_fabricator_gain_every_seconds", 3.33333f, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<bool> RMCDropshipCASDebug =
        CVarDef.Create("rmc.dropship_cas_debug", false, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<int> RMCDropshipFlyByTimeSeconds =
        CVarDef.Create("rmc.dropship_fly_by_time_seconds", 100, CVar.REPLICATED | CVar.SERVER);

    public static readonly CVarDef<int> RMCDropshipHijackTravelTimeSeconds =
        CVarDef.Create("rmc.dropship_hijack_travel_time_seconds", 180, CVar.REPLICATED | CVar.SERVER);
}
