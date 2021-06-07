using Altinn.Platform.Storage.Interface.Models;
using Altinn.Studio.Designer.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Altinn.Studio.Designer.Controllers
{
    /// <summary>
    /// Controller for serving/editing the application metadata json file
    /// </summary>
    [ApiController]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    [Route("/designer/api/v1/{org}/{repo}")]
    public class ApplicationMetadataController : ControllerBase
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationMetadataController"/> class.
        /// </summary>
        /// <param name="repository">The repository implementation</param>
        public ApplicationMetadataController(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Gets the application metadata, url GET "/designer/api/v1/org/repo/"
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns>The application metadata</returns>
        [HttpGet]
        [ActionName("ApplicationMetadata")]
        public ActionResult GetApplicationMetadata(string org, string repo)
        {
            Application application = _repository.GetApplication(org, repo);
            if (application == null)
            {
                return NotFound();
            }

            return Ok(application);
        }

        /// <summary>
        /// Puts the application metadata, url PUT "/designer/api/v1/org/repo/
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <param name="applicationMetadata">The application metadata</param>
        /// <returns>The updated application metadata</returns>
        [HttpPut]
        [ActionName("ApplicationMetadata")]
        public ActionResult UpdateApplicationMetadata(string org, string repo, [FromBody] Application applicationMetadata)
        {
            if (_repository.UpdateApplication(org, repo, applicationMetadata))
            {
                Application updatedApplicationMetadata = _repository.GetApplication(org, repo);
                return Ok(updatedApplicationMetadata);
            }
            else
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Create an application metadata, url POST "/designer/api/v1/org/repo"
        /// </summary>
        /// <param name="org">Unique identifier of the organisation responsible for the repo.</param>
        /// <param name="repo">Application identifier which is unique within an organisation.</param>
        /// <returns>The created application metadata</returns>
        [HttpPost]
        [ActionName("ApplicationMetadata")]
        public ActionResult CreateApplicationMetadata(string org, string repo)
        {
            if (_repository.GetApplication(org, repo) != null)
            {
                return Conflict("ApplicationMetadata allready exists.");
            }
            else
            {
                // TO DO: Application title handling (issue #2053/#1725)
                _repository.CreateApplicationMetadata(org, repo, repo);
                Application createdApplication = _repository.GetApplication(org, repo);
                if (createdApplication == null)
                {
                    return StatusCode(500);
                }

                return Created($"/designer/api/v1/{org}/{repo}", createdApplication);
            }
        }
    }
}
