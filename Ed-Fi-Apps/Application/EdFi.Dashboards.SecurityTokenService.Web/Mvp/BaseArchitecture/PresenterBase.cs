// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

namespace EdFi.Dashboards.SecurityTokenService.Web.Mvp.BaseArchitecture
{
	public abstract class PresenterBase<TModel, TView> 
		where TView : IViewBase<TModel>
	{
		public TView View { get;set;}
	}
}