namespace Monty_Hall_Problem_Tester;

public class MontyHallProblemRunner
{
    private const int rounds = 10000;
    
    
    public void Run(bool quietLogging)
    {
        for (int i = 0; i < rounds; i++)
        {
            RunOnce(i+1, quietLogging);
        }

        CalculateResults();
    }

    struct RoundResult
    {
        public RoundResult(bool playerChangedDoor, bool playerWon, Door winningDoor, Door playerInitialDoorSelection, Door playerFinalDoorSelection, Door hostOpenedDoor)
        {
            PlayerChangedDoor = playerChangedDoor;
            PlayerWon = playerWon;
            WinningDoor = winningDoor;
            PlayerInitialDoorSelection = playerInitialDoorSelection;
            PlayerFinalDoorSelection = playerFinalDoorSelection;
            HostOpenedDoor = hostOpenedDoor;
        }

        public bool PlayerChangedDoor { get; }
        public bool PlayerWon { get; }
        public Door WinningDoor { get;  }
        public Door PlayerFinalDoorSelection { get; }
        public Door PlayerInitialDoorSelection { get; }
        public Door HostOpenedDoor { get; }
    }

    private void CalculateResults()
    {
        var numberOfTimePlayerChangedDoor = _roundResults.Count(r => r.PlayerChangedDoor);
        var numberOfTimePlayerWon = _roundResults.Count(r => r.PlayerWon);
        var numberOfTimesPlayerChangedDoorAndWon = _roundResults.Count(r => r.PlayerChangedDoor && r.PlayerWon);
        var numberOfTimesPlayerDidNotChangeDoorAndWon = _roundResults.Count(r => !r.PlayerChangedDoor && r.PlayerWon);
        var numberOfTimesPlayerDidNotChangeDoorAndLost = _roundResults.Count(r => !r.PlayerChangedDoor && !r.PlayerWon);
        var numberOfTimesPlayerChangedDoorAndLost = _roundResults.Count(r => r.PlayerChangedDoor && !r.PlayerWon);
        var totalRounds = _roundResults.Count;
        
        var percentTimesPlayerChangedDoorAndWon = (double)numberOfTimesPlayerChangedDoorAndWon / (double)totalRounds;
        var percentTimesPlayerChangedDoorAndLost = (double)numberOfTimesPlayerChangedDoorAndLost / (double)totalRounds;
        var percentTimesPlayerDidNotChangeDoorAndWon = (double)numberOfTimesPlayerDidNotChangeDoorAndWon / (double)totalRounds;
        var percentTimesPlayerDidNotChangeDoorAndLost = (double)numberOfTimesPlayerDidNotChangeDoorAndLost / (double)totalRounds;
        
        
        Console.WriteLine("*************************************************************************");
        Console.WriteLine();
        Console.WriteLine(" Results: ");
        Console.WriteLine($"   Rounds: {totalRounds}");
        Console.WriteLine($"   Player Changed Door: {numberOfTimePlayerChangedDoor}");
        Console.WriteLine($"   Player Won: {numberOfTimePlayerWon}");
        Console.WriteLine($"   Player Changed Door And Won: {numberOfTimesPlayerChangedDoorAndWon} - {percentTimesPlayerChangedDoorAndWon:P0}");
        Console.WriteLine($"   Player Changed Door And Lost: {numberOfTimesPlayerChangedDoorAndLost} - {percentTimesPlayerChangedDoorAndLost:P0}");
        Console.WriteLine($"   Player Did Not Change Door And Won: {numberOfTimesPlayerDidNotChangeDoorAndWon} - {percentTimesPlayerDidNotChangeDoorAndWon:P0}");
        Console.WriteLine($"   Player Did Not Change Door And Lost: {numberOfTimesPlayerDidNotChangeDoorAndLost} - {percentTimesPlayerDidNotChangeDoorAndLost:P0}");
                
        
    }


    private void RunOnce(int n, bool quietLogging)
    {
        if(!quietLogging) Console.WriteLine($"Starting round #{n}...");
        var round = new GameRound(quietLogging);
        
        if(!quietLogging)  LogStatus(round);

        var playerPickedDoorInitial = Picker.PickADoor();
        round.StepOne_PlayerPickADoor(playerPickedDoorInitial);
        if(!quietLogging) Console.WriteLine($"Step 1 - Player picks door {playerPickedDoorInitial.ToText()}");
        
        if(!quietLogging) LogStatus(round);
        
        var hostPickedDoorStep2 = round.StepTwo_HostOpenLosingDoor();
        
        if(!quietLogging) Console.WriteLine($"Step 2 - Host picks door {hostPickedDoorStep2.ToText()}");
        
        if(!quietLogging) LogStatus(round);

        var playerDecidesToChangeDoor = Picker.YesOrNo();
        var playerPickedStep3 = round.StepThree_PlayerSelectsCurrentOrOtherDoor(playerDecidesToChangeDoor);
        
        if(!quietLogging) Console.WriteLine($"Step 3 - Player picked door {playerPickedStep3.ToText()}");
        
        if(!quietLogging) LogStatus(round);

        var playerWon = round.StepFour_OpenPlayerSelectedDoor();
        string playerText = playerWon ? "won" : "lost";
        if(!quietLogging) Console.WriteLine($"Step 4 - Player {playerText}");
        
        var roundStat = new RoundResult(playerDecidesToChangeDoor, playerWon, round.GetWinningDoor(), playerPickedDoorInitial, playerPickedStep3, hostPickedDoorStep2 );
        _roundResults.Add(roundStat);
    }
    
    private List<RoundResult> _roundResults = new List<RoundResult>();

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