using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Poster.Core
{
    public class PosterMiddleware
    {
        private static readonly TimeSpan DefaultExpiry = TimeSpan.FromSeconds(15); // TODO: Config

        public HttpResponseCache ResponseCache { get; private set; }

        public Func<IDictionary<string, object>, Task> Next { get; set; }

        public PosterMiddleware(IContentStore store, string defaultDocumentFile = null, string defaultTemplateFile = null, string publishedDocumentsFile = null, Encoding encoding = null)
        {
            ResponseCache = new HttpResponseCache(store, defaultDocumentFile, defaultTemplateFile, publishedDocumentsFile, encoding);
        }

        public void Initialize(Func<IDictionary<string, object>, Task> next)
        {
            Next = next;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            IOwinContext context = new OwinContext(environment);

            string requestPath = context.Request.Path.Value;

            // Our friendly Map function already runs PathString.StartsWithSegments, which strips off the mapped path (e.g. "/blog/post-one" will be passed as "/post-one")
            // If we're browsing the root of our mapped path (e.g. "/blog"), PathString.StartsWithSegments will return an empty PathString
            // We expect a pre-URL decoded value for requestPath (e.g. %20 is decoded to a space character), which PathString does for us
            requestPath = requestPath ?? string.Empty;

            // All our incoming requestPaths (except the "index" request, e.g. "/") will start with a directory separator (e.g. "/blog/post-one" will be passed as "/post-one")
            // Some URLs might end with a directory separator (e.g. "/blog/post-one/" will be passed as "/post-one/")
            // Path.Combine doesn't accept a child path that is "rooted" (e.g. starts with a directory separator), which most of our incoming requestPaths will be (e.g. "/post-one")
            // In all cases, strip off the directory separators
            requestPath = requestPath.Trim(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Do we have anything to respond with?
            // This will build a view for us if the requestPath matches a document
            HttpResponse response = await ResponseCache.GetCachedResponseAsync(context, requestPath);

            if (response == null)
            {
                // Maybe someone further down the pipeline is supposed to handle this instead
                await Next.Invoke(environment);

                // Bail out
                return;
            }

            // This comes from the ETag we sent to the client previously
            // Value is the hash of the full, composited "view"
            string ifNoneMatch = context.Request.Headers["If-None-Match"];

            if (!string.IsNullOrWhiteSpace(ifNoneMatch) && ifNoneMatch == response.ETag)
            {
                context.Response.StatusCode = 304;
                context.Response.ReasonPhrase = "Not Modified";

                // https://tools.ietf.org/html/rfc7232#section-4.1
                // The server generating a 304 response MUST generate any of the following header fields that would have been sent in a 200(OK) response to the same request: 
                // Cache-Control
                // Content-Location
                // Date
                // ETag
                // Expires
                // Vary
                context.Response.ETag = response.ETag;
                context.Response.Expires = DateTime.UtcNow.Add(DefaultExpiry);
            }
            else
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                context.Response.ETag = response.ETag;
                context.Response.Expires = DateTime.UtcNow.Add(DefaultExpiry);

                // Play Jeopardy theme while we write to the output stream...
                await context.Response.WriteAsync(response.Content);
            }
        }
    }
}
