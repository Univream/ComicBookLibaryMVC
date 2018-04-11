using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Models
{
    /// <summary>
    /// Defines an Entity in this Database
    /// </summary>
    public interface IEntity
    {
        int Id { get; set; }
    }
}
