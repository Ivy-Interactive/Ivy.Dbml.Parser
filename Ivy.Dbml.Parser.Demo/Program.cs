using Ivy.Dbml.Parser.Demo.Apps;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");
var server = new IvyServer();
#if !DEBUG
server.UseHttpRedirection();
#endif
#if DEBUG
server.UseHotReload();
#endif
server.AddAppsFromAssembly();
server.UseHotReload();
server.UseDefaultApp(typeof(DefaultApp));
await server.RunAsync();