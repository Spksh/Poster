using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
            // Branch to new pipeline 
            app.Map("/api", app2 =>
            {
                app2.Use(async (context, next) =>
                {
                    Trace.WriteLine(context.Request.Uri);

                    using (StreamReader reader = new StreamReader(context.Request.Body))
                    {
                        Trace.WriteLine(await reader.ReadToEndAsync());
                    }

                    context.Request.Body.Position = 0;

                    await next.Invoke();
                });

                app2.Use(new XmlRpcFaultMiddleware()
                    // Do some logging
                    .OnFaulted((s, e) =>
                    {
                        XmlRpcException fault = e as XmlRpcException;

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
                            Trace.WriteLine($"ERROR: {e.Source} {e.Message}");
                            Trace.WriteLine(e.StackTrace);
                        }
                    }));

                app2.Use(new XmlRpcRequestFilterMiddleware());

                app2.Use(new XmlRpcMiddleware()
                    .Add(new MetaWeblogApiResponder().WithGetUsersBlogs(async (appkey, username, password) => { return new List<Blog>(); })));
            });

            // Branch to new pipeline 
            app.Map("/blog", app2 =>
            {
                app2.Use(new PosterMiddleware(

                    contentStore: new FileSystemContentStore(
                        HostingEnvironment.MapPath(Default.VirtualPath)
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