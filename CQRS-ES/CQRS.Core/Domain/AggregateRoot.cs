using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<BaseEvent> _changes = new();

        protected Guid _id;
        public Guid Id => _id;


        public int Version { get; set; } = -1;


        public IEnumerable<BaseEvent> GetUncommittedChanges() => _changes;

        public void MarkChangesIsCommitted() => _changes.Clear();


        private void ApplyChanges(BaseEvent @event, bool isNew)
        {
            var eventType = @event.GetType();
            var method = this.GetType().GetMethod("Apply", new Type[] { eventType });
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method),
                    $"The Apply method was not found in the aggregate for {eventType.Name}");
            }

            method.Invoke(this, new object[] { @event });

            if (isNew)
            {
                this._changes.Add(@event);
            }
        }

        protected void RaiseEvent(BaseEvent @event)
        {
            ApplyChanges(@event, true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> changes)
        {
            foreach (var baseEvent in changes)
            {
                ApplyChanges(baseEvent, false);
            }
        }

    }
}
