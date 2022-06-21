using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using template.data.Context;
using template.data.Repositories;
using template.data.UnitOfWork;
using template.domain.Interfaces.Auth;
using template.domain.Interfaces.HttpClient;
using template.domain.Interfaces.Repositories;
using template.domain.Interfaces.UnitOfWork;
using template.domain.Interfaces.Users;
using template.services.Auth;
using template.services.AutoMapper;
using template.services.HttpClients;
using template.services.Users;

#region VARIABLES
var builder = WebApplication.CreateBuilder(args);
var origins = builder.Configuration.GetSection("AppSettings")["Origins"].Split(';');
var connectionString = builder.Configuration.GetConnectionString("company").ToString();
var secretKeyJWT = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("AppSettings")["JWT:Secret"].ToString());
#endregion

#region FILTERS
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
#endregion

#region CONTROLLERS
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
#endregion

#region SWAGGER
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
#endregion

#region INJECTION DEPENDENCY
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpClient<IHttpClientService, HttpClientService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IAuthService, AuthService>();
#endregion

#region DATABASE
builder.Services.AddDbContext<CompanyContext>(option => option.UseSqlServer(connectionString, b => b.MigrationsAssembly("template.api")));
#endregion

#region MAPPER
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_policyDevelopment",
        builder =>
        {
            builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();

        }
    );
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_policyStaging",
        builder =>
        {
            builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();

        }
    );
});
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "_policyProduction",
        builder =>
        {
            builder.WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
        }
    );
});
#endregion

#region AUTHENTICATION
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyJWT),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
#endregion

#region BUILD
var app = builder.Build();
#endregion

#region DEVELOPMENT ENVIRONMENT
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "template.api development"));

    app.UseCors("_policyDevelopment");
}
#endregion

#region STAGING ENVIRONMENT
if (app.Environment.IsStaging())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("http://development.company.com.br/1.0/api/swagger/v1/swagger.json", "template.api staging"));

    app.UseCors("_policyStaging");
}
#endregion

#region PRODUCTION ENVIRONMENT
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("http://company.com.br/1.0/api/swagger/v1/swagger.json", "template.api production"));

    app.UseCors("_policyProduction");

}
#endregion

#region ENVIRONMENTS
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
#endregion
