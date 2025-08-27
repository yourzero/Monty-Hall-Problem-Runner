namespace Monty_Hall_Problem_Tester;

public class MontyHallProblemRunner
{
    private const int rounds = 10;
    
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

        var playerPickedDoorInitial = Picker.PickADoor();
        round.StepOne_PlayerPickADoor(playerPickedDoorInitial);
        Console.WriteLine($"Step 1 - Player picks door {playerPickedDoorInitial.ToText()}");
        
        LogStatus(round);
        
        var hostPickedDoorStep2 = round.StepTwo_HostOpenLosingDoor();
        
        Console.WriteLine($"Step 2 - Host picks door {hostPickedDoorStep2.ToText()}");
        
        LogStatus(round);

        var playerDecidesToChangeDoor = Picker.YesOrNo();
        var playerPickedStep3 = round.StepThree_PlayerSelectsCurrentOrOtherDoor(playerDecidesToChangeDoor);
        
        Console.WriteLine($"Step 3 - Player picked door {playerPickedStep3.ToText()}");
        
        LogStatus(round);

        var playerWon = round.StepFour_OpenPlayerSelectedDoor();
        string playerText = playerWon ? "won" : "lost";
        Console.WriteLine($"Step 4 - Player {playerText}");
    }

    private static void LogStatus(GameRound round)
    {
        //round.OutputDoorsStatusText();
        round.OutputDoorsStatusGraphic();

        Console.WriteLine();
        Console.WriteLine("=============================================================================");
        Console.WriteLine();
    }
}