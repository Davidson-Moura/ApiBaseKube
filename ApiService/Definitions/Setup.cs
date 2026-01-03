using ApiService.Domain.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

namespace ApiService.Definitions
{
    internal class Setup
    {
        internal static void ConfigServices(WebApplicationBuilder builder)
        {
            builder.AddServiceDefaults();
            DefaultValues.SetOSPlatform();

            builder.Services.AddProblemDetails();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHealthChecks();
            builder.Services.AddSwaggerGen(opt =>
            {
                string schemeId = "Bearer";
                opt.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo()
                {
                    Title = "Api Service",
                    Version = "v1",
                    Description = "Api base para projetos",
                    Contact = new OpenApiContact
                    {
                        Name = "Inovy",
                        Email = "davidson@inovy.com.br",
                        Url = new Uri("https://inovy.com.br")
                    }

                });
                opt.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = schemeId,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Informe o token JWT no formato: Bearer {seu_token}"
                });
                /*
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement { 
                    {
                        new OpenApiSecurityScheme
                            {
                                new OpenApiReference
                                {
                                    Id = schemeId,
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>()
                    }});
                */

                opt.AddSecurityRequirement(document => new() { [new OpenApiSecuritySchemeReference(schemeId, document)] = [] });
            });

            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Api"));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Bearer";
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Api:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Api:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Api:IssuerSigningKey"]!))
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            builder.Services.AddControllers();
            builder.Services.AddHttpLogging();

            builder.Services.AddHttpContextAccessor();

            InjectionRegister.Register(builder.Services, builder.Configuration);
        }
    }
}
