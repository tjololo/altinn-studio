using System.Threading.Tasks;
using Altinn.Studio.Designer.Services.Models;

namespace Altinn.Studio.Designer.Services.Interfaces
{
    /// <summary>
    /// IApplicationInformationService
    /// </summary>
    public interface IApplicationInformationService
    {
        /// <summary>
        /// Updates all relevant application information on a new deployment
        /// </summary>
        /// <param name="org">Organisation</param>
        /// <param name="repo">Application</param>
        /// <param name="shortCommitId">Commit Id</param>
        /// <param name="deploymentEnvironment">EnvironmentModel</param>
        Task UpdateApplicationInformationAsync(
            string org,
            string repo,
            string shortCommitId,
            EnvironmentModel deploymentEnvironment);
    }
}
