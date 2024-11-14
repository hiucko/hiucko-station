using Content.Shared.Interaction.Events; // Подписка на взаимодействия, нам нужно знать когда с предметом взаимодействовали.
using Content.Shared.Popups; // Отправка всплывающего сообщения
using Content.Shared.hiucko.Tutorial; // Компонент в котором это всё будет работать

namespace Content.Server.hiucko.Tutorial; // Название

public sealed class SendWorldMessageOnUseSystem  : EntitySystem // Сама система отправки сообщения
{
    [Dependency] private SharedPopupSystem _popup = default!; // Нам нужен метод вызывающий сообщение, подписка не систему в которой он содержится

    public override void Initialize() // При инициализации компонента
    {
        base.Initialize; //

        SubscribeLocalEvent<SendWorldMessageOnUseComponent, UseInHandEvent>(OnUseInHand); // Через наш компонент вызывает OnUseInHand при UseInHandEvent  
    }

    private void OnUseInHand(Entity<SendWorldMessageOnUseComponent> entity, ref UseInHandEvent args) // Исполняет код на сущности entity, получая её аргументы из UseInHandEvent 
    {
        _popup.PopupEntity("Hello World!", entity.Owner);  // Само всплывающее сообщение владельцу сущности. Владелец сущности получается из "ref UseInHandEvent args"
    }
}