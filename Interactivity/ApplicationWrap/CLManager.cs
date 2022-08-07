namespace ApplicationWrap;

public abstract class CLCommand
{
    public string Name { get; }
    public string Description { get; }

    protected CLCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public abstract Task Execute(string[] args);
}

public interface ICommandLineEntity
{
    bool isAlive();
}

public class ClManager
{
    private class HelpCommand : CLCommand
    {
        private readonly ClManager _manager;
    
        public HelpCommand(ClManager manager) : base("help", "Displays a list of all available commands.")
        {
            _manager = manager;
        }

        public override Task Execute(string[] args)
        {
            Console.WriteLine("-> [ Command Help ]");
            foreach(var command in _manager.Commands())
            {
                Console.WriteLine($"{command.Name} - {command.Description}");
            }
            Console.WriteLine("<-");
            return Task.CompletedTask;
        }
    }
    
    private readonly ICommandLineEntity _commandLineEntity;
    private readonly Dictionary<string, CLCommand> _commandMap = new();

    public ClManager(ICommandLineEntity commandLineEntity)
    {
        _commandLineEntity = commandLineEntity;
        RegisterCommand(new HelpCommand(this));
    }
    
    /**
     * Register a command to the command manager.
     */
    public void RegisterCommand(CLCommand command)
    {
        _commandMap.Add(command.Name, command);
    }

    private List<CLCommand> Commands()
    {
        return _commandMap.Values.ToList();
    }

    public void Run()
    {
        while (_commandLineEntity.isAlive())
        {
            var input = Console.ReadLine();
            if (input == null)
            {
                Thread.Sleep(200);
                continue;
            }

            var split = input.Split(" ");
            var command = _commandMap!.GetValueOrDefault(split[0], null);

            if (command == null)
            {
                Console.WriteLine("Unknown command. Please type 'help' for a list of all commands.");
                continue;
            }

            var args = new string[split.Length - 1];
            if (args.Length > 0)
            {
                Array.Copy(split, 1, args, 0, args.Length);
            }

            try
            {
                var task = command.Execute(args);
                if (task.Status == TaskStatus.Faulted)
                {
                    Console.WriteLine("Command execution failed:");
                    Console.WriteLine(task.Exception);
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine("[CommandManager] An error occured while attempting to execute the command: {0}", command.Name);
                Console.WriteLine(ex);
            }
        }
    }
}