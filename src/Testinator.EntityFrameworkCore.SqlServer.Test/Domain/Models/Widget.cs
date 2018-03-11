using System;
using System.Collections.Generic;
using System.Text;

namespace Testinator.EntityFrameworkCore.SqlServer.Test.Domain.Models
{
    public class Widget
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public ICollection<StoreWidget> StoreWidgets { get; set; }
    }
}
