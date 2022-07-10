using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityFromScratch.Models
{
    public class Movies
    {
        public int MoviesId { get; set; }
        public string Title { get; set; }
        public List<Watch> Watches { get; set; }
    }
    
}
