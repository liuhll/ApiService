using System;
using Abp.Domain.Entities;
using Abp.Extensions;

namespace Jueci.ApiService.ApiAuthorization.Entities
{
    public class AppKey : Entity
    {
        public string AppId { get; set; }

        public string SecretKey { get; set; }

        public DateTime CreateTime { get; set; }
    }
}