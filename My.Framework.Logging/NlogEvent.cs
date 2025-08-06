using System.Collections;

namespace My.Framework.Logging
{
    public class NlogEvent : IEnumerable<KeyValuePair<string, object>>
    {
        List<KeyValuePair<string, object>> _properties = new List<KeyValuePair<string, object>>();

        public string Message { get; }

        public NlogEvent(string message)
        {
            Message = message;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public NlogEvent AddProp(string name, object value)
        {
            _properties.Add(new KeyValuePair<string, object>(name, value));
            return this;
        }

        public static Func<NlogEvent, Exception, string> Formatter { get; } = (l, e) => l.Message;
    }
}
