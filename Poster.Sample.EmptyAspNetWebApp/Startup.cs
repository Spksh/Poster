using System.Diagnostics;
using System.IO;
using System.Runtime.Caching;
using System.Web.Hosting;
using Microsoft.Owin;
using Owin;
using Poster.Content.FileSystem;
using Poster.Core;
using Poster.DefaultDocument.Published;
using Poster.MetaWeblogApi;
using TentacleSoftware.XmlRpc.Core;
using TentacleSoftware.XmlRpc.Owin;

[assembly: OwinStartup(typeof(Poster.Sample.EmptyAspNetWebApp.Startup))]
namespace Poster.Sample.EmptyAspNetWebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JsonConfigurationProvider configuration = new JsonConfigurationProvider()
                .With(HostingEnvironment.MapPath("/App_Data/config.json"))
                .With(() => MemoryCache.Default);

            // Branch to new pipeline 
            app.Map("/api", app2 =>
            {
                //app2.Use(async (context, next) =>
                //{
                //    Trace.WriteLine(context.Request.Uri);

                //    using (StreamReader reader = new StreamReader(context.Request.Body))
                //    {
                //        Trace.WriteLine(await reader.ReadToEndAsync());
                //    }

                //    context.Request.Body.Position = 0;

                //    await next.Invoke();
                //});

                app2.Use(new XmlRpcFaultMiddleware()
                    .OnFaulted((sender, exception) =>
                    {
                        XmlRpcException fault = exception as XmlRpcException;

                        if (fault != null)
                        {
                            Trace.WriteLine($"FAULT: {fault.Code} {fault.Message}");

                            if (fault.InnerException != null)
                            {
                                Trace.WriteLine($"ERROR: {fault.InnerException.Message}");
                            }

                            Trace.WriteLine(fault.StackTrace);
                        }
                        else
                        {
                            Trace.WriteLine($"ERROR: {exception.Source} {exception.Message}");
                            Trace.WriteLine(exception.StackTrace);
                        }
                    }));

                app2.Use(new XmlRpcRequestFilterMiddleware());

                app2.Use(new XmlRpcMiddleware()
                    .Add(new MetaWeblogApiResponder()
                        .With(new JsonAuthenticationProvider(configuration))
                        .With(new JsonMetaWeblogProvider(configuration)))
                );
            });

            // Branch to new pipeline 
            app.Map("/blog", app2 =>
            {
                app2.Use(new PosterMiddleware(

                    contentStore: new FileSystemContentStore(
                        HostingEnvironment.MapPath("/App_Data")
                    ),

                    defaultDocumentProvider: new PublishedDefaultDocumentProvider(
                        ".published"
                    ),

                    defaultTemplateFile: ".template"
                ));
            });
        }
    }
}