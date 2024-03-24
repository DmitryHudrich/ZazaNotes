using System.Text.RegularExpressions;

namespace Zaza.Web.Stuff;

internal static class ArgumentManager {
    private static readonly string[] cliArgs = Environment.GetCommandLineArgs();

    private static readonly Arg[] availableArgs = [
        new Arg {
            Flag = "--mongo",
            Decription = "MongoDb connection string. May be 'default' or 'exist' or 'mongodb://<host>:<port>'",
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
            Decription = "bebra",
            Values = new Dictionary<string, Action<string>> {
                { "bebra", (string value) => Console.WriteLine("BEBRA") },
            },
        },
        new Arg {
            Flag = "--swagger",
            Decription = "Should swagger ui be shown",
            IfFound = () => State.UseSwagger = true,
        },
        new Arg {
            Flag = "--bad-password",
            Decription = "Disables password validation. This option should be used only for testing purposes.",
            IfFound = () => State.DisablePasswordValidation = true,
        },
        new Arg {
            Flag = "--test-api",
            Decription =
                "Included some interesting stuff for testing: \n" +
                "   Ability to delete db.\n" +
                "This option should be used only for testing purposes.",
            IfFound = () => State.TestApi = true,
        },
        new Arg {
            Flag = "--h",
            Decription = "Show this help and exit",
            IfFound = () => {
                Console.WriteLine("Usage: dotnet Zaza.Web.dll [FLAG] [FLAG OPTIONS] ");
                Console.WriteLine("Example: dotnet Zaza.Web.dll --mongo default --swagger");
                Console.WriteLine();
                foreach (var arg in availableArgs!) {
                    Console.WriteLine($"{arg.Flag} - {arg.Decription}");
                }
                Environment.Exit(0);
            }
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
            if (arg.IfFound != null) {
                arg.IfFound?.Invoke();
            }
            if (arg.Values != null) {
                foreach (var pairs in arg.Values) {
                    if (Regex.IsMatch(cliArgs[argIndex + 1], pairs.Key)) {
                        pairs.Value?.Invoke(cliArgs[argIndex + 1]);
                    }
                }
            }
        }
    }
}

internal class Arg {
    public required string Flag;
    public string? Decription;
    public Dictionary<string, Action<string>>? Values;
    public Action? IfFound;
}
