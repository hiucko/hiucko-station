﻿using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Chat;

namespace Content.Shared._RMC14.Chat;

public abstract class SharedCMChatSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<MarineComponent, ChatGetPrefixEvent>(OnMarineGetPrefix);
        SubscribeLocalEvent<XenoComponent, ChatGetPrefixEvent>(OnXenoGetPrefix);
    }

    private void OnMarineGetPrefix(Entity<MarineComponent> ent, ref ChatGetPrefixEvent args)
    {
        if (args.Channel?.ID == SharedChatSystem.HivemindChannel)
            args.Channel = null;
    }

    private void OnXenoGetPrefix(Entity<XenoComponent> ent, ref ChatGetPrefixEvent args)
    {
        if (args.Channel?.ID != SharedChatSystem.HivemindChannel)
            args.Channel = null;
    }

    public virtual string SanitizeMessageReplaceWords(EntityUid source, string msg)
    {
        return msg;
    }
}
