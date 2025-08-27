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

    public static void LogStatus(GameRound round)
    {
        // Use the combined renderer: doors on the left, status box on the right
        round.OutputDoorsStatusSideBySide(
            initialPadding: 7,          // left margin before the first door
            spaceBetweenDoors: 6,       // gap between each door
            doorWidth: 9,               // door width (characters)
            spaceBetweenDoorsAndStatus: 9 // gap between doors and the status box on the right
        );

        Console.WriteLine();
        Console.WriteLine("=============================================================================");
        Console.WriteLine();
    }
}