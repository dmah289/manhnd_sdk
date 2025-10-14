namespace manhnd_sdk.Scripts.SystemDesign.EventBus
{
    public struct DummyDTO : IEventDTO
    {
        public int num;
        public DummyDTO(int num)
        {
            this.num = num;
        }
    }
}