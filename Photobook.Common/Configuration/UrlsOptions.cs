using System;

namespace Photobook.Common.Configuration
{
    public class UrlsOptions
    {
        public string WebUrl { get; set; }
        public string IdentityServerUrl { get; set; }

        public string GenerateLink(string path)
        {
            var baseUri = new Uri(WebUrl);
            var link = new Uri(baseUri, path);

            return link.AbsoluteUri;
        }
    }
}
