using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentaCar.RealEstateManager.Database.Data;
using RentaCar.RealEstateManager.Database.Data.Entities;

[assembly: HostingStartup(typeof(RentaCar.Areas.Identity.IdentityHostingStartup))]
namespace RentaCar.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}
