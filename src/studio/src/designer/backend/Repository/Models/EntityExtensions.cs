using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AltinnCore.Authentication.Constants;
using Microsoft.AspNetCore.Http;

namespace Altinn.Studio.Designer.Repository.Models
{
    /// <summary>
    /// Contains extension methods for entity classes
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Populates base properties
        /// </summary>
        /// <param name="entity">BaseEntity</param>
        /// <param name="org">org</param>
        /// <param name="repo">repo</param>
        /// <param name="httpContext">HttpContext</param>
        /// <returns></returns>
        public static BaseEntity PopulateBaseProperties(this BaseEntity entity, string org, string repo, HttpContext httpContext)
        {
            List<Claim> claims = httpContext.User.Claims.ToList();
            entity.Org = org;
            entity.Repo = repo;
            entity.Created = DateTime.Now;
            entity.CreatedBy = claims.FirstOrDefault(x => x.Type == AltinnCoreClaimTypes.Developer)?.Value;

            return entity;
        }
    }
}
