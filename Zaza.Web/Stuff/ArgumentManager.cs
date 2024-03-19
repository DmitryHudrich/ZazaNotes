using System.Text.RegularExpressions;

namespace Zaza.Web.Stuff;

internal static class ArgumentManager {
    private static string[] cliArgs = Environment.GetCommandLineArgs();

    private static Arg[] availableArgs = [
        new Arg {
            Flag = "--mongo",
            Values = new Dictionary<string, Action<string>> {
                { "default", (string _) => Environment.SetEnvironmentVariable(StaticStuff.MongoStingEnvName, StaticStuff.MongoStingDefault) },
                { "exist", (string _) => { if (Environment.GetEnvironmentVariable(StaticStuff.MongoStingEnvName) == null) {
                    Environment.Exit(-2);
                }}},
                { @"^mongodb://\w*", (string value) => Environment.SetEnvironmentVariable(StaticStuff.MongoStingEnvName, value) },
            }
        },
        new Arg {
            Flag = "--test",
            Values = new Dictionary<string, Action<string>> {
                { @"^mongodb://\w*", (string value) => System.Console.WriteLine("BEBRA") },
            },
        }
    ];

    public static void Check() {
        foreach (var arg in availableArgs) {
            Exec(arg);
        }
    }

    private static void Exec(Arg arg) {
        if (cliArgs.Contains(arg.Flag)) {
            var argIndex = Array.FindIndex(cliArgs, 0, el => el == arg.Flag);
            foreach (var pairs in arg.Values) {
                if (Regex.IsMatch(cliArgs[argIndex + 1], pairs.Key)) {
                    pairs.Value?.Invoke(cliArgs[argIndex + 1]);

                }
            }
        }
    }
}

internal class Arg {
    public required string Flag;
    public required Dictionary<string, Action<string>> Values;
}
