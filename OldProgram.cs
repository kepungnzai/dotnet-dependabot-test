using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Newtonsoft.Json;
using AutoMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Swashbuckle.AspNetCore.Swagger;

// This is a .NET Core 2.2 / 3.1 style Startup (pre-minimal hosting)
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        
        // Configure Serilog (old 2.0 style)
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add Entity Framework (old EF Core 2.2 style)
        services.AddDbContext<OldAppDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // Add MVC with Newtonsoft.Json (old style)
        services.AddMvc()
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            })
            .AddFluentValidation(fv => 
            {
                fv.RegisterValidatorsFromAssemblyContaining<Startup>();
            });

        // Add AutoMapper (old 8.0 style)
        services.AddAutoMapper(typeof(Startup));

        // Add MediatR (old 5.1 style with string assembly name)
        services.AddMediatR(typeof(Startup).Assembly);

        // Add Swagger (old Swashbuckle 2.5 style)
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
            {
                Title = "Old API",
                Version = "v1"
            });
            var xmlPath = Path.Combine(AppContext.BaseDirectory, "OldDotNetApp.xml");
            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);
        });

        // Add API Versioning (old style)
        services.AddApiVersioning();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        // Exception handling (old style middleware)
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        // Use Serilog (old style)
        app.UseSerilogRequestLogging();

        // Static files
        app.UseStaticFiles();

        // Swagger (old style)
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Old API V1");
        });

        // Routing (old style)
        app.UseMvc(routes =>
        {
            routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}

// OLD EF Core 2.2 style DbContext
public class OldAppDbContext : DbContext
{
    public OldAppDbContext(DbContextOptions<OldAppDbContext> options) : base(options)
    {
    }

    // Old EF Core 2.2 style - DbSet as virtual properties
    public virtual DbSet<OldUser> Users { get; set; }
    public virtual DbSet<OldProduct> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Old way of configuring entities
        modelBuilder.Entity<OldUser>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("getutcdate()");
        });

        modelBuilder.Entity<OldProduct>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Price)
                .HasPrecision(18, 2);
        });
    }
}

// Old style model classes
public class OldUser
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OldProduct
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Old AutoMapper 8.0 style Profile
public class OldMappingProfile : Profile
{
    public OldMappingProfile()
    {
        // Old AutoMapper 8.0 style mapping
        CreateMap<OldUser, OldUserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ReverseMap();

        CreateMap<OldProduct, OldProductDto>()
            .ReverseMap();
    }
}

// DTO classes
public class OldUserDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
}

public class OldProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Old MediatR 5.1 style request/handler
public class GetUsersQuery : IRequest<System.Collections.Generic.List<OldUserDto>>
{
}

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, System.Collections.Generic.List<OldUserDto>>
{
    private readonly OldAppDbContext _context;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(OldAppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // Old MediatR 5.1 style handle method
    public async System.Threading.Tasks.Task<System.Collections.Generic.List<OldUserDto>> Handle(
        GetUsersQuery request, 
        System.Threading.CancellationToken cancellationToken)
    {
        var users = await _context.Users.ToListAsync();
        return _mapper.Map<System.Collections.Generic.List<OldUserDto>>(users);
    }
}

// Old Controller style
[Route("api/[controller]")]
[ApiController]
public class OldUsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OldUsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async System.Threading.Tasks.Task<ActionResult<System.Collections.Generic.List<OldUserDto>>> GetUsers()
    {
        var result = await _mediator.Send(new GetUsersQuery());
        return Ok(result);
    }
}

// Main entry point (old .NET Core 2.2 style)
public class Program
{
    public static void Main(string[] args)
    {
        CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseSerilog();
}
