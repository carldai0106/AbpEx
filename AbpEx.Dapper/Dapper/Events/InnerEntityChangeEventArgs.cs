using Abp.Events.Bus;

namespace Abp.Dapper.Events
{
    internal class InnerEntityChangeEventArgs : EventData
    {
        public object Entity { get; set; }
        public EventTypes EventType { get; set; }
        public InnerEntityChangeEventArgs(object entity, EventTypes type)
        {
            Entity = entity;
            EventType = type;
        }
    }
}
