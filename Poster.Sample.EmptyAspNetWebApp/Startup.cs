using System.Web.Hosting;
using Microsoft.Owin;
using Owin;
using Poster.Content.FileSystem;
using Poster.Core;

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
                    store: new FileSystemContentStore(
                        HostingEnvironment.MapPath(Default.VirtualPath)
                    ),
                    //defaultDocumentFile: ".md",
                    publishedDocumentsFile: ".published",
                    defaultTemplateFile: ".template"
                ));
            });
        }
    }
}