using System.Collections.Generic;

namespace Testinator.EntityFrameworkCore.SqlServer.Test.Domain.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public ICollection<Widget> Widgets { get; set; }

    }
}
