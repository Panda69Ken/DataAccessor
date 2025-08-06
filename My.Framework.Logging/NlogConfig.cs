namespace My.Framework.Logging
{
    public class NlogConfig
    {
        /// <summary>
        /// Exceptionless ApiKey
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Exceptionless ServerUrl
        /// </summary>
        public string ServerUrl { get; set; }

        public List<NlogConfigRule> Rules { get; set; }
        public string ELKIndex { get; set; }
        public string ELKUrl { get; set; }
        public string ELKProject { get; set; }
        public bool RequireAuth { get; set; }
        public string Username { get; set; }
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
