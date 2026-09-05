using Domain.OpenAI;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Repository.Accounts;
using Repository.ChatBuilder;
using Repository.ItemAttributes;
using Repository.ItemMaster;
using Repository.PartyMaster;
using Repository.PurchaseBill;
using Repository.PurchaseOrder;
using Repository.Reports.Stock;
using Repository.UserAuthentication;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Common
{
    public static class DependencyInjection
    {
        public static void AddDependency(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddJsonOptions(optionss =>
            {
                optionss.JsonSerializerOptions.PropertyNamingPolicy = null;
            });

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddScoped<IAIService, AIServiceClass>();
            services.AddDbContext<BulkyContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("SqlConnection"));
            });
            services.AddScoped<IDbConnection>(db => new SqlConnection(configuration.GetConnectionString("SqlConnection")));
            services.Configure<OpenAIOptions>(configuration.GetSection(OpenAIOptions.SectionName));

            services.AddCors(p => p.AddPolicy("corsapp", options =>
            {
                options.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            }));

            services.AddEndpointsApiExplorer();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetValue<string>("SecurityKey") ?? string.Empty)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context =>
                    {
                        string authHeaders = context.Request.Headers["Authorization"]!;
                        if (string.IsNullOrWhiteSpace(authHeaders))
                        {
                            if (context.Request.Cookies.ContainsKey("JWT"))
                            {
                                context.Token = context.Request.Cookies["JWT"];
                            }
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ATTRMASTER-VIEW_POLICY", policy =>
                {
                    policy.RequireClaim("ATTRMASTER-VIEW_POLICY", "View");
                });
                options.AddPolicy("ATTRMASTER-ALL_POLICY", policy =>
                {
                    policy.RequireClaim("ATTRMASTER-ALL_POLICY", "Edit");
                });

                options.AddPolicy("CATEGORYMASTER-VIEW_POLICY", policy =>
                {
                    policy.RequireClaim("CATEGORYMASTER-VIEW_POLICY", "View");
                });
                options.AddPolicy("CATEGORYMASTER-ALL_POLICY", policy =>
                {
                    policy.RequireClaim("CATEGORYMASTER-ALL_POLICY", "Edit");
                });

                options.AddPolicy("ITEMMASTER-VIEW_POLICY", policy =>
                {
                    policy.RequireClaim("ITEMMASTER-VIEW_POLICY", "View");
                });
                options.AddPolicy("ITEMMASTER-ALL_POLICY", policy =>
                {
                    policy.RequireClaim("ITEMMASTER-ALL_POLICY", "Edit");
                });

                options.AddPolicy("LEDGERMASTER-VIEW_POLICY", policy =>
                {
                    policy.RequireClaim("LEDGERMASTER-VIEW_POLICY", "View");
                });
                options.AddPolicy("LEDGERMASTER-ALL_POLICY", policy =>
                {
                    policy.RequireClaim("LEDGERMASTER-ALL_POLICY", "Edit");
                });
            });

            services.AddScoped<IAccountsResponse, Repository.Accounts.NAccounts.DALClass>();
            services.AddScoped<IAccountsCategoryMasterResponse, Repository.Accounts.NAccounts.DALClass>();
            services.AddScoped<IUserResponse, Repository.UserAuthentication.NUserAuthentication.DALClass>();
            services.AddScoped<IItemAttributeResponse, Repository.ItemAttributes.NItemAttributes.DALClass>();
            services.AddScoped<IItemMasterResponse, Repository.ItemMaster.NItemMaster.DALClass>();
            services.AddScoped<IStockReports, Repository.Reports.Stock.NStock.DALClass>();
            services.AddScoped<IPartyMaster, Repository.PartyMaster.NPartyMaster.DALClass>();
            services.AddScoped<IPurchaseOrder, Repository.PurchaseOrder.NPurchaseOrder.DALClass>();
            services.AddScoped<IPurchaseBill, Repository.PurchaseBill.NPurchaseBill.DALClass>();
        } // AddDependency...
    } // class...
}
