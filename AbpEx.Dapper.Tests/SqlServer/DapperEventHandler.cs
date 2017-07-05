using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Dependency;
using Newtonsoft.Json;
using System.Diagnostics;
using Abp.Dapper.Tests.Entities;

namespace Abp.Dapper.Tests.SqlServer
{
    public class DapperEventHandler :
        IEventHandler<EntityDeletingEventData<Person>>,
        IEventHandler<EntityDeletedEventData<Person>>,
        IEventHandler<EntityCreatingEventData<Person>>,
        IEventHandler<EntityCreatedEventData<Person>>,
        ITransientDependency
    {
        public DapperEventHandler()
        {

        }

        public void HandleEvent(EntityDeletedEventData<Person> eventData)
        {
            Trace.WriteLine("Deleted");
            Trace.WriteLine(JsonConvert.SerializeObject(eventData.Entity, Formatting.Indented));
        }

        public void HandleEvent(EntityDeletingEventData<Person> eventData)
        {
            Trace.WriteLine("Deleting");
            Trace.WriteLine(JsonConvert.SerializeObject(eventData.Entity, Formatting.Indented));
        }

        public void HandleEvent(EntityCreatingEventData<Person> eventData)
        {
            Trace.WriteLine("Creating");
            Trace.WriteLine(JsonConvert.SerializeObject(eventData.Entity, Formatting.Indented));
        }

        public void HandleEvent(EntityCreatedEventData<Person> eventData)
        {
            Trace.WriteLine("Crated");
            Trace.WriteLine(JsonConvert.SerializeObject(eventData.Entity, Formatting.Indented));
        }
    }
}
