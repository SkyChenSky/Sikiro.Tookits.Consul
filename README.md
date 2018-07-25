# Sikiro.Tookits.Consul
Sikiro.Tookits.Consul is base on consul.net Library.

## Getting Started

### Startup.cs

* AddConsul
```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddOptions().AddConsul(option =>
   {
       option.WithHost(Configuration["ConsulConfiguration:Host"]);
   }).AddMvc();
}
```

* UseConsul
```c#
public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IServiceProvider svp)
{
    if (env.IsDevelopment())
        app.UseDeveloperExceptionPage();
    else
        app.UseExceptionHandler("/Home/Error");

    app.UseConsul(lifetime, option =>
    {
        option.WithSelfHost(Configuration["SelfHost"]);
        option.WithServerName(Configuration["ConsulConfiguration:ServerName"]);
    });

    app.UseMvc(routes =>
    {
        routes.MapRoute(
            "default",
            "{controller=Home}/{action=Index}/{id?}");
    });
}
```

## End
If you have good suggestions, please feel free to mention to me.

