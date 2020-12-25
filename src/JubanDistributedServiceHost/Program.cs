using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace JubanDistributedServiceHost {
    class Program {
        static async Task Main (string[] args) {
            var servicelibsVar = "";
            servicelibsVar=Environment.GetEnvironmentVariable("JUBAN_SERVICE_LIBS");

            foreach (var arg in args) {
                if (arg.StartsWith ("--servicelibs=")) {
                    servicelibsVar = arg.Replace ("--servicelibs=", "");
                }
            }
            Environment.SetEnvironmentVariable ("JUBAN_EXTRA_CONFIG_FOLDER", servicelibsVar);
            if (servicelibsVar==null || servicelibsVar.Length == 0) {
                servicelibsVar = System.AppContext.BaseDirectory;
            }
            new DirectoryCatalog (servicelibsVar);

            var TypesDict = new Dictionary<string, Type> ();
            var types = AppDomain.CurrentDomain.GetAssemblies ()
                .SelectMany (s => {
                    try {
                        if (s.FullName.Contains ("Juban")) {

                            Console.WriteLine (s.FullName);
                        }
                        return s.GetTypes ();
                    } catch (ReflectionTypeLoadException e) {

                        return new Type[0];
                    }
                })
                .ToList ();
            Type cmlType = null;
            foreach (var element in types) {
                if (element.FullName.Equals ("Jubanlabs.JubanDistributed.CommandLineInterface")) {
                    Console.WriteLine ("found "+element.FullName);
                    cmlType = element;
                }
            }

            var cml = Activator.CreateInstance (cmlType);
//cml.SetLogTarget();
            var methodSetLogTarget = cmlType.GetMethod ("SetLogTarget");
//cml.Main(args);
            var methodMain = cmlType.GetMethod ("Main", new Type[] { typeof (string[]) });
            methodSetLogTarget.Invoke (cml, null);
            
            
            methodMain.Invoke (cml, new object[] { args });

            var builder = new HostBuilder ();
            await builder.RunConsoleAsync ();
        }

    }
}