using Shared.Common.Extensions;
using BarMicroservice.Client;
using FooMicroservice.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomControllers();
builder.Services.AddCustomSwagger("FrontendGateway", "1");

builder.Services.AddVerticalSlices(
    builder.Configuration,
    typeof(Program).Assembly
);

builder.Services.AddBarMicroservice();
builder.Services.AddFooMicroservice();

builder.Services.AddHeaderPropagation(options => options.Headers.Add("kubernetes-route-as"));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseReviewEnvironment();
app.UseHeaderPropagation();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseCloudEvents();

app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
    endpoints.MapControllers();
});

app.UseCustomSwaggerUI("FrontendGateway", "1");

app.Run();