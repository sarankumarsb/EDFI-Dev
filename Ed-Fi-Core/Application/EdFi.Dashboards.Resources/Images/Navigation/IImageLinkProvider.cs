using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Images.Navigation
{
    public interface IImageLinkProvider
    {
        string GetImageLink(ImageRequestBase request);
    }

    public class NullImageLinkProvider :IImageLinkProvider
    {
        public string GetImageLink(ImageRequestBase request)
        {
            throw new NotImplementedException(string.Format("Unhandled request {0}", request.GetType().Name));
        }
    }
}
