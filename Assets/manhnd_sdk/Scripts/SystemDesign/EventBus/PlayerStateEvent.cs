namespace manhnd_sdk.Scripts.SystemDesign.EventBus
{
    public struct PlayerStateEventDto : IEventDTO
    {
        public int health;

        public PlayerStateEventDto(int health)
        {
            this.health = health;
        }
    }
}