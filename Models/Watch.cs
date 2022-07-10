using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityFromScratch.Models
{
    public class Watch
    {
        
            public int WatchId { get; set; }
            public DateTime Watched { get; set; }
            
            public string UserEmail { get; set; }
            public int MoviesId { get; set; }

            [ForeignKey("MoviesId")]
            public Movies Movie { get; set; }

        
    }
}
