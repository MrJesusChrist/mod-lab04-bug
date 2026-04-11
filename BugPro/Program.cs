using Stateless;

namespace BugPro;

public enum BugState
{
    Open,
    Triaging,
    InProgress,
    Closed
}

public enum BugTrigger
{
    Submit,
    Defer,
    StartFix,
    Resolve,
    Return,
    Reject,
    Reopen
}

public class Bug
{
    private readonly StateMachine<BugState, BugTrigger> _machine;

    public BugState State => _machine.State;

    public Bug(string title)
    {
        Title = title;
        _machine = new StateMachine<BugState, BugTrigger>(BugState.Open);

        _machine.Configure(BugState.Open)
            .Permit(BugTrigger.Submit, BugState.Triaging);

        _machine.Configure(BugState.Triaging)
            .PermitReentry(BugTrigger.Defer)
            .Permit(BugTrigger.StartFix, BugState.InProgress)
            .Permit(BugTrigger.Reject, BugState.Closed);

        _machine.Configure(BugState.InProgress)
            .Permit(BugTrigger.Resolve, BugState.Closed)
            .Permit(BugTrigger.Return, BugState.Triaging);

        _machine.Configure(BugState.Closed)
            .Permit(BugTrigger.Reopen, BugState.Triaging);
    }

    public string Title { get; }

    public void Submit() => _machine.Fire(BugTrigger.Submit);
    public void Defer() => _machine.Fire(BugTrigger.Defer);
    public void StartFix() => _machine.Fire(BugTrigger.StartFix);
    public void Resolve() => _machine.Fire(BugTrigger.Resolve);
    public void Return() => _machine.Fire(BugTrigger.Return);
    public void Reject() => _machine.Fire(BugTrigger.Reject);
    public void Reopen() => _machine.Fire(BugTrigger.Reopen);

    public bool CanFire(BugTrigger trigger) => _machine.CanFire(trigger);
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Bug WorkFlow Demo ===\n");

        var bug = new Bug("Приложение падает при запуске");
        Console.WriteLine($"Баг: {bug.Title}");
        Console.WriteLine($"Состояние: {bug.State}");

        bug.Submit();
        Console.WriteLine($"После Submit: {bug.State}");

        bug.Defer();
        Console.WriteLine($"После Defer: {bug.State}");

        bug.StartFix();
        Console.WriteLine($"После StartFix: {bug.State}");

        bug.Return();
        Console.WriteLine($"После Return: {bug.State}");

        bug.StartFix();
        Console.WriteLine($"После повторного StartFix: {bug.State}");

        bug.Resolve();
        Console.WriteLine($"После Resolve: {bug.State}");

        bug.Reopen();
        Console.WriteLine($"После Reopen: {bug.State}");

        bug.Reject();
        Console.WriteLine($"После Reject: {bug.State}");

        Console.WriteLine("\n=== Тест отклонения ===");
        var bug2 = new Bug("Дублирующий баг");
        bug2.Submit();
        bug2.Reject();
        Console.WriteLine($"Баг '{bug2.Title}' отклонён. Состояние: {bug2.State}");
    }
}
