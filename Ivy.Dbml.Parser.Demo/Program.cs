using Ivy.Dbml.Parser.Demo.Apps;

var server = new Server();
server.UseCulture("en-US");
#if DEBUG
server.UseHotReload();
#endif
server.AddAppsFromAssembly();
server.UseDefaultApp(typeof(DefaultApp));
await server.RunAsync();
