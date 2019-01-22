using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using Trepub.Common.Extensions;

namespace Trepub.Web.API.Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "Trepub.Web.API.Test";
            app.HelpOption("-?|-h|--help");

            app.Command("run", runTestCommand);
            app.Command("runTask", runTask);

            try
            {
                app.Execute(args);
            }
            catch (Exception exc)
            {
                Console.WriteLine(" >>>>>>>> Trepub integration tests failed. <<<<<<<<<< \nERROR:" + exc.ToString());
            }

        }

        protected static Action<CommandLineApplication> runTask => (command) =>
       {
           command.Description = "Encrypt given text parameter.";
           command.HelpOption("-?|-h|--help");

           var taskName = command.Argument("[taskName]",
                                      "specifies the text to be encrypted");
           var parameters = command.Argument("[parameters]",
                                      "specifies params used in task", multipleValues: true);
           CommandOption hostOption = command.Option("-url", "The host target used for task.",
                                        CommandOptionType.SingleValue);

           command.OnExecute(() =>
           {
               if (!string.IsNullOrEmpty(taskName.Value))
               {
                   Type taskType = typeof(Program).Assembly.GetTypes().Where(
                          t => t.IsSubclassOf(typeof(BaseAppClientTaskRunner)) &&
                          !t.IsAbstract &&
                          t.Name.Contains(taskName.Value)).FirstOrDefault();
                   Console.WriteLine($"\n---------- starting:{taskType.Name}");
                   command.Arguments.RemoveAt(0);
                   (Activator.CreateInstance(taskType) as BaseAppClientTaskRunner).RunTask(hostOption.Value(), parameters.Values);
                   Console.WriteLine($"---------- finished:{taskType.Name}");
               }
               else
               {
                   throw new Exception("please specify task name");
               }
               return 0;
           });
       };


        protected static Action<CommandLineApplication> runTestCommand => (command) =>
            {
                command.Description = "Runs specified integration test(s) against server. leave it empty to run all the tests.";
                command.HelpOption("-?|-h|--help");

                var testNameArgument = command.Argument("[testName]",
                                           "specifies testCase name or part of it");

                command.OnExecute(() =>
                {
                    List<Type> tests = new List<Type>();
                    if (!string.IsNullOrEmpty(testNameArgument.Value))
                    {
                        tests = typeof(Program).Assembly.GetTypes().Where(
                            t => t.IsSubclassOf(typeof(BaseWebTest)) &&
                            !t.IsAbstract &&
                            t.Name.Contains(testNameArgument.Value)).ToList();
                    }
                    else
                    {
                        //To run all test cases
                        tests = typeof(Program).Assembly.GetTypes().Where(
                        t => t.IsSubclassOf(typeof(BaseWebTest)) &&
                        !t.IsAbstract
                        ).ToList();
                    }
                    foreach (var t in tests)
                    {
                        Console.WriteLine($"\n---------- starting:{t.Name}");
                        (Activator.CreateInstance(t) as BaseWebTest).RunTest();
                        Console.WriteLine($"---------- finished:{t.Name}");
                    }

                    return 0;
                });
            };

    }
}
