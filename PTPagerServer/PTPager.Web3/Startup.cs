using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Polly;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PTPager.Alerting.Interfaces;
using PTPager.Alerting.PollySpeech;
using PTPager.Alerting.PollySpeech.Configuration;
using PTPager.Alerting.Polycom;
using PTPager.Alerting.Polycom.Configuration;
using PTPager.Alerting.Services;
using PTPager.Web3.Configuration;

namespace PTPager.Web3
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();

			services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
			services.AddAWSService<IAmazonPolly>();

			services.Configure<PolycomAudioTransmitterConfiguration>(Configuration.GetSection("polycomAudioTransmitter"));
			services.Configure<PollySpeechSynthesizerConfiguration>(Configuration.GetSection("pollySpeechSynthesizer"));
			services.Configure<OAuthConfiguration>(Configuration.GetSection("oAuth"));

			services.AddTransient<ISynthesizeSpeech, PollySpeechSynthesizer>();
			services.AddTransient<IAudioTransmitter, PolycomAudioTransmitter>();
			services.AddTransient<AlertingService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
