using System.Web.Hosting;
using Microsoft.Owin;
using Owin;
using Poster.Content.FileSystem;
using Poster.Core;
using Poster.DefaultDocument.Published;

[assembly: OwinStartup(typeof(Poster.Sample.EmptyAspNetWebApp.Startup))]
namespace Poster.Sample.EmptyAspNetWebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
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