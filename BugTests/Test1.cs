using BugPro;

namespace BugTests;

[TestClass]
public sealed class Test1
{
    // Тест 1: начальное состояние — Open
    [TestMethod]
    public void NewBug_InitialState_IsOpen()
    {
        var bug = new Bug("Тестовый баг");
        Assert.AreEqual(BugState.Open, bug.State);
    }

    // Тест 2: Submit переводит из Open в Triaging
    [TestMethod]
    public void Submit_FromOpen_MovesToTriaging()
    {
        var bug = new Bug("Баг 1");
        bug.Submit();
        Assert.AreEqual(BugState.Triaging, bug.State);
    }

    // Тест 3: Defer из Triaging оставляет состояние Triaging
    [TestMethod]
    public void Defer_FromTriaging_StaysInTriaging()
    {
        var bug = new Bug("Баг 2");
        bug.Submit();
        bug.Defer();
        Assert.AreEqual(BugState.Triaging, bug.State);
    }

    // Тест 4: несколько Defer подряд — всё равно Triaging
    [TestMethod]
    public void Defer_MultipleTimes_StaysInTriaging()
    {
        var bug = new Bug("Баг 3");
        bug.Submit();
        bug.Defer();
        bug.Defer();
        bug.Defer();
        Assert.AreEqual(BugState.Triaging, bug.State);
    }

    // Тест 5: StartFix из Triaging переводит в InProgress
    [TestMethod]
    public void StartFix_FromTriaging_MovesToInProgress()
    {
        var bug = new Bug("Баг 4");
        bug.Submit();
        bug.StartFix();
        Assert.AreEqual(BugState.InProgress, bug.State);
    }

    // Тест 6: Resolve из InProgress переводит в Closed
    [TestMethod]
    public void Resolve_FromInProgress_MovesToClosed()
    {
        var bug = new Bug("Баг 5");
        bug.Submit();
        bug.StartFix();
        bug.Resolve();
        Assert.AreEqual(BugState.Closed, bug.State);
    }

    // Тест 7: Return из InProgress возвращает в Triaging
    [TestMethod]
    public void Return_FromInProgress_MovesToTriaging()
    {
        var bug = new Bug("Баг 6");
        bug.Submit();
        bug.StartFix();
        bug.Return();
        Assert.AreEqual(BugState.Triaging, bug.State);
    }

    // Тест 8: Reject из Triaging переводит в Closed
    [TestMethod]
    public void Reject_FromTriaging_MovesToClosed()
    {
        var bug = new Bug("Баг 7");
        bug.Submit();
        bug.Reject();
        Assert.AreEqual(BugState.Closed, bug.State);
    }

    // Тест 9: Reopen из Closed возвращает в Triaging
    [TestMethod]
    public void Reopen_FromClosed_MovesToTriaging()
    {
        var bug = new Bug("Баг 8");
        bug.Submit();
        bug.StartFix();
        bug.Resolve();
        bug.Reopen();
        Assert.AreEqual(BugState.Triaging, bug.State);
    }

    // Тест 10: полный цикл — Open → Triaging → InProgress → Closed → Triaging → Closed
    [TestMethod]
    public void FullCycle_ReopenAndResolveAgain_EndsClosed()
    {
        var bug = new Bug("Баг 9");
        bug.Submit();
        bug.StartFix();
        bug.Resolve();
        bug.Reopen();
        bug.StartFix();
        bug.Resolve();
        Assert.AreEqual(BugState.Closed, bug.State);
    }

    // Тест 11: Return и повторный StartFix + Resolve
    [TestMethod]
    public void Return_ThenStartFixAndResolve_EndsClosed()
    {
        var bug = new Bug("Баг 10");
        bug.Submit();
        bug.StartFix();
        bug.Return();
        bug.StartFix();
        bug.Resolve();
        Assert.AreEqual(BugState.Closed, bug.State);
    }

    // Тест 12: Submit из Open — недопустимый повтор Submit бросает исключение
    [TestMethod]
    public void Submit_FromTriaging_ThrowsException()
    {
        var bug = new Bug("Баг 11");
        bug.Submit();
        Assert.ThrowsExactly<InvalidOperationException>(() => bug.Submit());
    }

    // Тест 13: StartFix из Open бросает исключение (нет такого перехода)
    [TestMethod]
    public void StartFix_FromOpen_ThrowsException()
    {
        var bug = new Bug("Баг 12");
        Assert.ThrowsExactly<InvalidOperationException>(() => bug.StartFix());
    }

    // Тест 14: CanFire — Submit доступен из Open
    [TestMethod]
    public void CanFire_Submit_TrueWhenOpen()
    {
        var bug = new Bug("Баг 13");
        Assert.IsTrue(bug.CanFire(BugTrigger.Submit));
    }

    // Тест 15: CanFire — Submit недоступен из Triaging
    [TestMethod]
    public void CanFire_Submit_FalseWhenTriaging()
    {
        var bug = new Bug("Баг 14");
        bug.Submit();
        Assert.IsFalse(bug.CanFire(BugTrigger.Submit));
    }
}
