using EdFi.Dashboards.Presentation.Core.UITests.Support;

namespace EdFi.Dashboards.Presentation.Core.UITests.Pages.Error
{
    public abstract class ErrorBasedPageBase<TPage, TModel> : PageBase<TModel>
        where TPage : PageBase
    {
        protected SchoolType schoolType;

        // For fluent usage to set the context to a particular school type
        public TPage For(SchoolType schoolType)
        {
            this.schoolType = schoolType;
            return this as TPage;
        }
    }
}
