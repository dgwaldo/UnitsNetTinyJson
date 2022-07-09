using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(jsonOptions =>
{
    jsonOptions.SerializerSettings.Converters.Add(new UnitsNetTinyJsonConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
//{
//    Converters = new List<JsonConverter> { new UnitsNetTinyJsonConverter() }
//};

TypeDescriptor.AddAttributes(typeof(Length), new TypeConverterAttribute(typeof(UnitsNetTinyTypeConvert<Length>)));
//TypeDescriptor.AddAttributes(typeof(IEnumerable<Length>), new TypeConverterAttribute(typeof(UnitsNetTinyTypeConvert<IEnumerable<Length>>)));


//public void DoMagic()
//{
//    // NOTE: After this, you can use your typeconverter.
//    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
//}