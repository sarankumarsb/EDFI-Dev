using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EdFi.Dashboards.Application.Resources.Admin;
using EdFi.Dashboards.Application.Resources.Models.Admin;
using EdFi.Dashboards.Resources.Admin;
using EdFi.Dashboards.Resources.Models.Admin;

namespace EdFi.Dashboards.Presentation.Core.Areas.Admin.Controllers
{
    public class TitleClaimSetController : Controller
    {
        private readonly ITitleClaimSetService _titleClaimSetService;

        public TitleClaimSetController(ITitleClaimSetService titleClaimSetService)
        {
            _titleClaimSetService = titleClaimSetService;
        }

        [HttpGet]
        public ViewResult Get(int localEducationAgencyId)
        {
            var model = _titleClaimSetService.Get(new TitleClaimSetRequest { LocalEducationAgencyId = localEducationAgencyId });
            model.CurrentOperation = "EditSingleClaimSet";

            return View("Get", model);
        }

        [HttpPost]
        public ViewResult EditSingleClaimSet(TitleClaimSetModel setModel)
        {
            _titleClaimSetService.Post(setModel);

            var refreshedModel = _titleClaimSetService.Get(new TitleClaimSetRequest { LocalEducationAgencyId = setModel.LocalEducationAgencyId });

            refreshedModel.CurrentOperation = "EditSingleClaimSet";

            refreshedModel.IsPost = true;
            refreshedModel.IsSuccess = true;
            refreshedModel.Messages = new List<string> { string.Format("Position Title {0} was updated successfully", setModel.PositionTitle) };

            return View("Get", refreshedModel);
        }

        [HttpPost]
        public ViewResult EditBatchClaimSet(TitleClaimSetModel setModel, HttpPostedFileBase file)
        {
            TitleClaimSetModel refreshedModel = null;

            if (file == null || string.IsNullOrEmpty(file.FileName) || file.InputStream == null)
            {
                refreshedModel = _titleClaimSetService.Get(new TitleClaimSetRequest { LocalEducationAgencyId = setModel.LocalEducationAgencyId });

                refreshedModel.CurrentOperation = "EditBatchClaimSet";

                refreshedModel.IsPost = true;
                refreshedModel.IsSuccess = false;
                refreshedModel.Messages = new List<string> { "The file selected is null." };

                return View("Get", refreshedModel);
            }

            setModel.FileName = file.FileName;
            setModel.FileInputStream = file.InputStream;

            var postResultsModel = _titleClaimSetService.PostBatch(setModel);

            refreshedModel = _titleClaimSetService.Get(new TitleClaimSetRequest { LocalEducationAgencyId = setModel.LocalEducationAgencyId });

            refreshedModel.CurrentOperation = "EditBatchClaimSet";

            refreshedModel.IsPost = true;
            refreshedModel.IsSuccess = postResultsModel.IsSuccess;
            refreshedModel.Messages = postResultsModel.Messages;

            return View("Get", refreshedModel);
        }
    }
}
