namespace Monty_Hall_Problem_Tester;

public class MontyHallProblemRunner
{
    private const int rounds = 1;
    
    public void Run()
    {
        for (int i = 0; i < rounds; i++)
        {
            RunOnce(i+1);
        }
    }


    private void RunOnce(int n)
    {
        Console.WriteLine($"Starting round #{n}...");
        var round = new GameRound();
        
        LogStatus(round);

        var playerPickedDoor = Picker.PickADoor();
        round.StepOne_PlayerPickADoor(playerPickedDoor);
        Console.WriteLine($"Step 1 - Player picks door {playerPickedDoor}");
        
        LogStatus(round);
    }

    private static void LogStatus(GameRound round)
    {
        round.OutputDoorsStatusText();
        round.OutputDoorsStatusGraphic();

        Console.WriteLine();
        Console.WriteLine("=============================================================================");
        Console.WriteLine();
    }
}