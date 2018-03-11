using System;
using System.Collections.Generic;
using System.Text;

namespace Testinator.EntityFrameworkCore.SqlServer.Test.Domain.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public ICollection<StoreWidget> StoreWidgets { get; set; }
    }
}
