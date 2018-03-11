using System.Collections.Generic;

namespace Testinator.EntityFrameworkCore.SqlServer.Test.Domain.Models
{
    public class StoreWidget
    {
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public int WidgetId { get; set; }
        public Widget Widget { get; set; }

    }
}
