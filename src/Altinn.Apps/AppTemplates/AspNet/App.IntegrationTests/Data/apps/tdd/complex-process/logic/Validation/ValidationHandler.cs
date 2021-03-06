using System;
using System.Threading.Tasks;
using Altinn.App.Services.Interface;
using Altinn.Platform.Storage.Interface.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace App.IntegrationTests.Mocks.Apps.tdd.complex_process.AppLogic.Validation
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    public class ValidationHandler
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IInstance _instanceService;

        public ValidationHandler(IInstance instanceService, IHttpContextAccessor httpContextAccessor = null)
        {
            _httpContextAccessor = httpContextAccessor;
            _instanceService = instanceService;
        }

        public async Task ValidateData(object instance, ModelStateDictionary validationResults)
        {   
            await Task.CompletedTask;
        }

        public async Task ValidateTask(Instance instance, string task, ModelStateDictionary validationResults)
        {
            DateTime valid = instance.Process.CurrentTask.Started.Value.AddSeconds(10);

            switch (task)
            {
                case "Task_1":
                    break;
                case "Task_2":
                    if (DateTime.UtcNow < valid)
                    {
                        validationResults.AddModelError("Time", "Validation time has not yet occured.");
                    }

                    break;
                default:
                    break;
            }
           
            await Task.CompletedTask;
        }
    }
}
