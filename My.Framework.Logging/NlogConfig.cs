namespace My.Framework.Logging
{
    public class NlogConfig
    {
        /// <summary>
        /// Exceptionless ApiKey
        /// </summary>
        public string ExceptionlessKey { get; set; }

        /// <summary>
        /// Server Url
        /// Exceptionless：http://192.168.0.1:8090/
        /// ElasticSearch：http://192.168.0.1:9200
        /// Logstash：tcp://192.168.0.1:5044
        /// </summary>
        public string ServerUrl { get; set; }

        public List<NlogConfigRule> Rules { get; set; }

        public string ElasticIndex { get; set; }
        public string ElasticProject { get; set; }

        /// <summary>
        /// ElasticSearch BasicAuth
        /// </summary>
        public bool RequireAuth { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
    }

    public class NlogConfigRule
    {
        public string Name { get; set; }
        public string MinLevel { get; set; }
        public string MaxLevel { get; set; }
        public string WriteTo { get; set; }
    }
}
