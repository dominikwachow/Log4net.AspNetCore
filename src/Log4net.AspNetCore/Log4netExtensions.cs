using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Log4net.AspNetCore
{
    public static class Log4netExtensions
    {
        public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string configPath = Consts.DefaultConfigFilePath)
        {
            factory.AddProvider(new Log4NetProvider(configPath));

            return factory;
        }

        public static IApplicationBuilder AddRequestIdentifier(this IApplicationBuilder applicationBuilder)
        {
            var contextAccessor = new HttpContextAccessor();

            Log4NetLogger.ActionIdProvider = () => contextAccessor.HttpContext?.Items.ContainsKey(Consts.RequestKey) == true ? contextAccessor.HttpContext.Items[Consts.RequestKey].ToString() : null;

            applicationBuilder.Use(async (context, next) =>
            {
                var accessor = applicationBuilder.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
                accessor.HttpContext.Items.Add(Consts.RequestKey, Guid.NewGuid().ToString());

                await next();
            });

            return applicationBuilder;
        }
    }
}
