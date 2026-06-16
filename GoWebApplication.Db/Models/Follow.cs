using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Models
{
    public class Follow
    {
        public string FollowingUserId {  get; set; }
        public string FollowedUserId { get; set; }
        public DateOnly CreatedAt {  get; set; }

        public User FollowingUser { get; set; }
        public User FollowedUser { get; set; }
    }
}
