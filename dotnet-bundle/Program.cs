﻿using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;

namespace Dotnet.Bundle
{
    class Program
    {
        static int Main(string[] args)
        {
            var application = new CommandLineApplication();
            
            var runtime = application.Option(
                "-r |--runtime <runtime>",
                "Publishes the application for a given runtime.",
                CommandOptionType.SingleValue);

            var framework = application.Option(
                "-f | --framework <framework>",
                "Publishes the application for the specified target framework.",
                CommandOptionType.SingleValue);

            var configuration = application.Option(
                "-c | --configuration <configuration>",
                "Defines the build configuration. The default value is Debug.",
                CommandOptionType.SingleValue);

            var output = application.Option(
                "-o | --output <output>",
                "Specifies the path for the output directory.",
                CommandOptionType.SingleValue);

            application.HelpOption("-h | --help");
            
            application.OnExecute(() =>
            {
                var command = new StringBuilder();
                
                command.Append($"msbuild /t:BundleApp ");

                if (runtime.HasValue())
                {
                    command.Append($"/p:RuntimeIdentifier={runtime.Value()} ");
                }

                if (framework.HasValue())
                {
                    command.Append($"/p:TargetFramework={framework.Value()} ");
                }

                if (configuration.HasValue())
                {
                    command.Append($"/p:Configuration={configuration.Value()} ");
                }

                if (output.HasValue())
                {
                    command.Append($"/p:OutputPath={output.Value()} ");
                    command.Append($"/p:AppendTargetFrameworkToOutputPath=false ");
                    command.Append($"/p:AppendRuntimeIdentifierToOutputPath=false ");
                }

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = command.ToString()
                    }
                };

                process.Start();
                process.WaitForExit();

                return process.ExitCode;
            });

            return application.Execute(args);
        }
    }
}