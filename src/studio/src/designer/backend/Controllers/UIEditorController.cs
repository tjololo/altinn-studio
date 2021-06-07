using System;
using System.Collections.Generic;
using System.Text;
using Altinn.Studio.Designer.Configuration;
using Altinn.Studio.Designer.Helpers;
using Altinn.Studio.Designer.Models;
using Altinn.Studio.Designer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Altinn.Studio.Designer.Controllers
{
    /// <summary>
    /// Controller containing all react-ions
    /// </summary>
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class UIEditorController : Controller
    {
        private readonly IRepository _repository;
        private readonly ServiceRepositorySettings _repoSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIEditorController"/> class.
        /// </summary>
        /// <param name="repositoryService">The application repository service</param>
        /// <param name="settings">The application repository settings.</param>
        public UIEditorController(
            IRepository repositoryService,
            IOptions<ServiceRepositorySettings> settings)
        {
            _repository = repositoryService;
            _repoSettings = settings.Value;
        }

        /// <summary>
        /// The index action which will show the React form builder
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns>A view with the React form builder</returns>
        public IActionResult Index(string org, string repo)
        {
            return RedirectToAction("Index", "ServiceDevelopment");
        }

        /// <summary>
        /// Get form layout as JSON
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns>The model representation as JSON</returns>
        [HttpGet]
        public ActionResult GetFormLayout(string org, string repo)
        {
            return Content(_repository.GetJsonFormLayouts(org, repo), "text/plain", Encoding.UTF8);
        }

        /// <summary>
        /// Get third party components listed as JSON
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns>The model representation as JSON</returns>
        [HttpGet]
        public ActionResult GetThirdPartyComponents(string org, string repo)
        {
            return Content(_repository.GetJsonThirdPartyComponents(org, repo), "text/plain", Encoding.UTF8);
        }

        /// <summary>
        /// Get rule handler in JSON structure
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns>The model representation as JSON</returns>
        [HttpGet]
        public ActionResult GetRuleHandler(string org, string repo)
        {
            return Content(_repository.GetRuleHandler(org, repo), "application/javascript", Encoding.UTF8);
        }

        /// <summary>
        /// Get text resource as JSON for specified language
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="id">The language id for the text resource file</param>
        /// <returns>The model representation as JSON</returns>
        [HttpGet]
        public ActionResult GetTextResources(string org, string repo, string id)
        {
            var result = _repository.GetLanguageResource(org, repo, id);
            return Content(result);
        }

        /// <summary>
        /// Add text resources to existing resource documents
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="textResources">The collection of text resources to be added</param>
        /// <returns>A success message if the save was successful</returns>
        [HttpPost]
        public ActionResult AddTextResources(string org, string repo, [FromBody] List<TextResource> textResources)
        {
            var success = _repository.AddTextResources(org, repo, textResources);
            return Json(new
            {
                Success = success
            });
        }

        /// <summary>
        /// Save form layout as JSON
        /// </summary>
        /// <param name="jsonData">The code list data to save</param>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="id">The name of the form layout to be saved.</param>
        /// <returns>A success message if the save was successful</returns>
        [HttpPost]
        public ActionResult SaveFormLayout([FromBody] dynamic jsonData, string org, string repo, string id)
        {
            _repository.SaveFormLayout(org, repo, id, jsonData.ToString());

            return Json(new
            {
                Success = true,
                Message = "Skjema lagret",
            });
        }

        /// <summary>
        /// Delete a form layout
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="id">The form layout to be deleted</param>
        /// <returns>A success message if the save was successful</returns>
        [HttpDelete]
        public ActionResult DeleteFormLayout(string org, string repo, string id)
        {
            if (_repository.DeleteFormLayout(org, repo, id))
            {
                return Json(new
                {
                    Success = true,
                    Message = "Skjema slettet",
                });
            }

            return Json(new
            {
                Success = false,
                Message = "Ikke slettet",
            });
        }

        /// <summary>
        /// Update a form layout name
        /// </summary>
        /// <param name="newName">The new name of the form layout.</param>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="id">The current name of the form layuout</param>
        /// <returns>A success message if the save was successful</returns>
        [HttpPost]
        public ActionResult UpdateFormLayoutName([FromBody] string newName, string org, string repo, string id)
        {
            bool success = _repository.UpdateFormLayoutName(org, repo, id, newName);
            return Json(new
            {
                Success = success,
            });
        }

        /// <summary>
        /// Saves the layout settings
        /// </summary>
        /// <param name="jsonData">The data to be saved</param>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns>A success message if the save was successful</returns>
        public ActionResult SaveLayoutSettings([FromBody] dynamic jsonData, string org, string repo)
        {
            _repository.SaveLayoutSettings(org, repo, jsonData.ToString());
            return Json(new
            {
                Success = true,
                Message = "Setting lagret",
            });
        }

        /// <summary>
        /// Gets the layout settings
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns>The content of the settings file</returns>
        public ActionResult GetLayoutSettings(string org, string repo)
        {
            return Content(_repository.GetLayoutSettings(org, repo), "application/json", Encoding.UTF8);
        }

        /// <summary>
        /// Save form layout as JSON
        /// </summary>
        /// <param name="jsonData">The code list data to save</param>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns>A success message if the save was successful</returns>
        [HttpPost]
        public ActionResult SaveThirdPartyComponents([FromBody] dynamic jsonData, string org, string repo)
        {
            _repository.SaveJsonThirdPartyComponents(org, repo, jsonData.ToString());

            return Json(new
            {
                Success = true,
                Message = "Tredjeparts komponenter lagret",
            });
        }

        /// <summary>
        /// Save JSON data as file
        /// </summary>
        /// <param name="jsonData">The code list data to save</param>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="fileName">The filename to be saved as</param>
        /// <returns>A success message if the save was successful</returns>
        [HttpPost]
        public ActionResult SaveJsonFile([FromBody] dynamic jsonData, string org, string repo, string fileName)
        {
            if (!ApplicationHelper.IsValidFilename(fileName))
            {
                return BadRequest();
            }

            if (fileName.Equals(_repoSettings.GetRuleConfigFileName()))
            {
                _repository.SaveRuleConfigJson(org, repo, jsonData.ToString());
            }
            else
            {
                _repository.SaveJsonFile(org, repo, jsonData.ToString(), fileName);
            }

            return Json(new
            {
                Success = true,
                Message = fileName + " saved",
            });
        }

        /// <summary>
        /// Get JSON file in JSON structure
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="fileName">The filename to read from</param>
        /// <returns>The model representation as JSON</returns>
        [HttpGet]
        public ActionResult GetJsonFile(string org, string repo, string fileName)
        {
            if (!ApplicationHelper.IsValidFilename(fileName))
            {
                return BadRequest();
            }

            return Content(_repository.GetJsonFile(org, repo, fileName), "application/javascript", Encoding.UTF8);
        }

        /// <summary>
        /// Adds the metadata for attachment
        /// </summary>
        /// <param name="applicationMetadata">the application meta data to be updated</param>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddMetadataForAttachment([FromBody] dynamic applicationMetadata, string org, string repo)
        {
            _repository.AddMetadataForAttachment(org, repo, applicationMetadata.ToString());
            return Json(new
            {
                Success = true,
                Message = " Metadata saved",
            });
        }

        /// <summary>
        /// Updates the metadata for attachment
        /// </summary>
        /// <param name="applicationMetadata">the application meta data to be updated</param>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateMetadataForAttachment([FromBody] dynamic applicationMetadata, string org, string repo)
        {
            _repository.UpdateMetadataForAttachment(org, repo, applicationMetadata.ToString());
            return Json(new
            {
                Success = true,
                Message = " Metadata saved",
            });
        }

        /// <summary>
        /// Deletes the metadata for attachment
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="id">the id of the component</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMetadataForAttachment(string org, string repo, string id)
        {
            _repository.DeleteMetadataForAttachment(org, repo, id);
            return Json(new
            {
                Success = true,
                Message = " Metadata saved",
            });
        }

        /// <summary>
        /// Gets widget settings for repo
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns>The widget settings for the repo.</returns>
        [HttpGet]
        public ActionResult GetWidgetSettings(string org, string repo)
        {
            var widgetSettings = _repository.GetWidgetSettings(org, repo);
            return Ok(widgetSettings);
        }
    }
}
