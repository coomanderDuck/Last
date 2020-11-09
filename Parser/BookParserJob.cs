using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Parser;
using Quartz;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeeebLibrary.Parser
{
    public class BookParserJob : IJob
    {
        private readonly IServiceProvider serviceProvider;

        public BookParserJob(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var parsCount = 50;
                var parser = scope.ServiceProvider.GetService<IParserBooks>();
                await parser.Pars(parsCount);
            }
            await Task.CompletedTask;
        }
    }
}