using EdFi.Dashboards.Presentation.Core.UITests.Support;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.Search
{
    public abstract class SearchBasedPageBase<TPage, TModel> : PageBase<TModel>
        where TPage : PageBase
    {
        protected SchoolType schoolType;

        // For fluent usage
        public TPage For(SchoolType schoolType)
        {
            this.schoolType = schoolType;
            return this as TPage;
        }
    }
}

