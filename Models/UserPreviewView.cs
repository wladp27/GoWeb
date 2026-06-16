using GoWebApplication.Db.Models;

namespace GoWeb.Models
{
    public class UserPreviewView
    {
        public string Id { get; set; }
        public string UserName {  get; set; }
        public List<Rating> Ratings { get; set; }
        public string DisplayName { get; set; }

    }
}
