using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jueci.ApiService.Pay.Entities;

namespace Jueci.ApiService.Mappings
{
    public class UserPayOrderInfoMap : EntityTypeConfiguration<UserPayOrderInfo>
    {
        public UserPayOrderInfoMap()
        {
            ToTable("UserPayOrderInfo");
        }
    }
}
