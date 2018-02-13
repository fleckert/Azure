using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.IotHub.Relay.Device;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Azure.IotHub.Relay
{
    public class Startup
    {
        string _connectionString = @"HostName=fleckert.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=R5M48NLlxESkgZQoOfMhqquJaBzNp0CwtV2rvSVAGwg=";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IDeviceClientProvider>(new DeviceClientCache(new DeviceClientProvider(_connectionString)));
			services.AddSingleton<IMessageSender, MessageSender>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
