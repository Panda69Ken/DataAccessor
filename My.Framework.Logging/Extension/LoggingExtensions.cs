using Exceptionless.NLog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog.Targets.ElasticSearch;
using NLog.Targets.Wrappers;
using System.Net;
using System.Text.RegularExpressions;
using LogLevel = NLog.LogLevel;

namespace My.Framework.Logging.Extension
{
    public static class LoggingExtensions
    {
        /// <summary>在.NET内核中启用Nlog作为日志记录提供程序.</summary>
        /// <param name="factory"></param>
        /// <returns>ILoggerFactory for chaining</returns>
        public static ILoggerFactory UseNLog(this ILoggerFactory factory)
        {
            return factory.AddNLog();
        }

        /// <summary>从XML配置中应用Nlog配置.</summary>
        /// <param name="env"></param>
        /// <param name="configPath">nlog配置文件的相对路径.</param>
        /// <returns>LoggingConfiguration for chaining</returns>
        public static LoggingConfiguration ConfigNLog(this ILoggingBuilder builder, string configPath = "nlog.config")
        {
            var factory = LogManager.LoadConfiguration(configPath);
            builder.AddNLog(factory.Configuration);
            return factory.Configuration;
        }

        /// <summary>
        /// 从Apollo配置中应用Nlog配置.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="config"></param>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static LoggingConfiguration ConfigNLog(this ILoggingBuilder builder, NlogConfig config,
            string configPath = "nlog.config")
        {
            var configuration = ReloadConfig(config, configPath);
            builder.AddNLog(configuration);
            return configuration;
        }


        /// <summary>从Apollo配置中应用Nlog配置.</summary>
        /// <param name="env"></param>
        /// <param name="config">apollo配置节点</param>
        /// <param name="configPath">nlog配置文件的相对路径.</param>
        /// <returns>LoggingConfiguration for chaining</returns>
        public static LoggingConfiguration ConfigNLog(this IHostingEnvironment env, NlogConfig config,
            string configPath = "nlog.config")
        {
            return ReloadConfig(config, configPath);
        }

        private static LoggingConfiguration ReloadConfig(NlogConfig config, string configPath = "nlog.config")
        {
            if (config == null)
            {
                throw new Exception("nlog配置不能为空");
            }

            var factory = LogManager.LoadConfiguration(configPath);

            if (factory.Configuration.AllTargets.FirstOrDefault(m => m.Name.ToLower() == "ownfile") == null)
            {
                factory.Configuration.AddTarget(new AsyncTargetWrapper("ownFile", new FileTarget
                {
                    Name = "ownFile",
                    FileName = "Logs/${level}/nlog-${shortdate}.log",
                    Layout = "${longdate}|${event-properties:item=EventId.Id}|${logger}|${uppercase:${level}}|  ${message} ${exception:format=toString,Data}"
                }));
            }

            if (factory.Configuration.AllTargets.FirstOrDefault(m => m.Name.ToLower() == "exceptionless") == null)
            {
                factory.Configuration.AddTarget(new AsyncTargetWrapper("exceptionless", new ExceptionlessTarget()
                {
                    Name = "exceptionless",
                    ApiKey = config.ApiKey,
                    ServerUrl = config.ServerUrl,
                    Fields =
                    {
                        new ExceptionlessField {Name = "host", Layout = "${machinename}"},
                        new ExceptionlessField {Name = "Request-Host", Layout = "${aspnet-Request-Host}"},
                        new ExceptionlessField {Name = "Request-useragent", Layout = "${aspnet-request-useragent}"},
                        new ExceptionlessField {Name = "Request-IP", Layout = "${aspnet-Request-IP}"},
                        new ExceptionlessField {Name = "Request-referrer", Layout = "${aspnet-request-referrer}"},
                        new ExceptionlessField {Name = "process", Layout = "${processname}"},
                    }
                }));
            }

            if (factory.Configuration.AllTargets.FirstOrDefault(m => m.Name.ToLower() == "console") == null)
            {
                factory.Configuration.AddTarget(new AsyncTargetWrapper("console", new ConsoleTarget()
                {
                    Name = "console",
                    Layout = "${longdate}|${event-properties:item=EventId.Id}|${logger}|${uppercase:${level}}|  ${message} ${exception:format=toString,Data}"
                }));
            }

            if (factory.Configuration.AllTargets.FirstOrDefault(m => m.Name.ToLower() == "elastic") == null)
            {
                //My.ProjectName.Rpc: host=my-projectName-rpc-6db45f8b-dhqbc, version=1.0.0
                var host = Dns.GetHostName();
                if (string.IsNullOrEmpty(config.ELKProject))
                {
                    var m = Regex.Replace(host, @"(.+[rpc|svc|api])-.+", "$1", RegexOptions.IgnoreCase);
                    config.ELKProject = m;
                }

                var target = new ElasticSearchTarget
                {
                    Name = "elastic",
                    Uri = config.ELKUrl,
                    Index = config.ELKIndex + "-${date:format=yyyy.MM.dd}",
                    Layout = "${message} ${exception:format=tostring}",
                    Fields = {
                        new Field { Name = "tags", Layout = "${event-properties:item=Tags}" },
                        new Field { Name = "namespace", Layout = "${logger}" }, 
                        //new Field{ Name = "memberid", Layout = "${event-properties:item=MemberId}" },
                        new Field { Name = "projectname", Layout = config.ELKProject },
                        new Field { Name = "host", Layout = host },
                        new Field { Name = "createTime", Layout = "${longdate}" },
                        //new Field{ Name="processId", Layout="${processid}"},
                        //new Field{ Name="threadId", Layout="${threadid}"},
                        new Field{ Name="stacktrace", Layout="${stacktrace}"},
                        //new Field{ Name="sequenceid", Layout="${sequenceid}"},
                        new Field{ Name="number", Layout="${counter:increment=1:sequence=Layout:value=1}"},
                        new Field{ Name="aspnetRequestIp", Layout="${aspnet-request-ip}"},
                        new Field{ Name="aspnetRequestHeader", Layout="${aspnet-request-headers:HeaderNames=request-uuid,request-url,request-time,request-operator}"}
                    }
                };

                if (config.RequireAuth)
                {
                    target.RequireAuth = config.RequireAuth;
                    target.Username = config.Username;
                    target.Password = config.Password;
                }

                factory.Configuration.AddTarget(new AsyncTargetWrapper("elastic", target));
            }

            if (config.Rules != null && config.Rules.Any())
            {
                var index = 0;
                foreach (var blackRule in config.Rules.Where(m => m.WriteTo.ToLower() == "blackhole"))
                {
                    if (!factory.Configuration.LoggingRules.Any(m =>
                        m.Targets.Contains(new NullTarget()) && m.LoggerNamePattern == blackRule.Name))
                    {
                        factory.Configuration.LoggingRules.Insert(index, new LoggingRule(blackRule.Name,
                                GetLogLevel(blackRule.MinLevel, LogLevel.Trace), GetLogLevel(blackRule.MaxLevel, LogLevel.Off),
                                new NullTarget())
                        { Final = true });
                        index++;
                    }
                }

                foreach (var blackRule in config.Rules.Where(m => m.WriteTo.ToLower() != "blackhole"))
                {
                    factory.Configuration.AddRule(GetLogLevel(blackRule.MinLevel, LogLevel.Trace),
                        GetLogLevel(blackRule.MaxLevel, LogLevel.Off), blackRule.WriteTo, blackRule.Name);
                }
            }

            factory.Configuration.Reload();
            return factory.Configuration;
        }

        private static LogLevel GetLogLevel(string level, LogLevel defaultLogLevel)
        {
            return (level?.ToLower()) switch
            {
                "trace" => LogLevel.Trace,
                "debug" => LogLevel.Debug,
                "info" => LogLevel.Info,
                "warn" => LogLevel.Warn,
                "error" => LogLevel.Error,
                "fatal" => LogLevel.Fatal,
                "off" => LogLevel.Off,
                _ => defaultLogLevel,
            };
        }
    }
}
