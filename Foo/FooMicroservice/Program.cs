using Shared.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomControllers();
builder.Services.AddCustomSwagger("FooMicroservice", "1");

builder.Services.AddVerticalSlices(
    builder.Configuration,
    typeof(Program).Assembly
);

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

app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
    endpoints.MapControllers();
});

app.UseCustomSwaggerUI("FooMicroservice", "1");

await app.RunAsync();
