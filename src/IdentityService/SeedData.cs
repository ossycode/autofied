using System.Security.Claims;
using IdentityModel;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityService;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if(userMgr.Users.Any()) return;
        
        var ossylab = userMgr.FindByNameAsync("ossylab").Result;
        if (ossylab == null)
        {
            ossylab = new ApplicationUser
            {
                UserName = "ossylab",
                Email = "ossylab@email.com",
                EmailConfirmed = true,
            };
            var result = userMgr.CreateAsync(ossylab, "Pass123$").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(ossylab, [
                            new Claim(JwtClaimTypes.Name, "Ossy Lab"),
                            // new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        ]).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            Log.Debug("ossylab created");
        }
        else
        {
            Log.Debug("ossylab already exists");
        }

        var bob = userMgr.FindByNameAsync("bob").Result;
        if (bob == null)
        {
            bob = new ApplicationUser
            {
                UserName = "bob",
                Email = "BobSmith@email.com",
                EmailConfirmed = true
            };
            var result = userMgr.CreateAsync(bob, "Pass123$").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(bob, [
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            // new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            // new Claim("location", "somewhere")
                        ]).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            Log.Debug("bob created");
        }
        else
        {
            Log.Debug("bob already exists");
        }
    }
}
