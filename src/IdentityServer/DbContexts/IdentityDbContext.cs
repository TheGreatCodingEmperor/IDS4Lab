using  Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using IdentityServer.Model;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer{
    public class IdentityDbContext:IdentityDbContext<User,Role,string>{
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options):base (options){}
    }
}