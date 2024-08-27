using Content.Shared._RMC14.Marines;
using Content.Shared.Chat;

namespace Content.Shared._RMC14.Chat;

public abstract class SharedCMChatSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<MarineComponent, ChatGetPrefixEvent>(OnMarineGetPrefix);
    }

    private void OnMarineGetPrefix(Entity<MarineComponent> ent, ref ChatGetPrefixEvent args)
    {
        if (args.Channel?.ID == SharedChatSystem.HivemindChannel)
            args.Channel = null;
    }

    public virtual string SanitizeMessageReplaceWords(EntityUid source, string msg)
    {
        return msg;
    }
}
