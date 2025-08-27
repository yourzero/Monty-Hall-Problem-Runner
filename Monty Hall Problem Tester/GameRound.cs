namespace Monty_Hall_Problem_Tester;

public class GameRound
{
    //private Picker _picker = new Picker();

    public Dictionary<Door, DoorStatus> Doors = new Dictionary<Door, DoorStatus>(3);
    private DoorStatus hostDoorStatus;

    public GameRound()
    {
        var prizeDoor = Picker.PickADoor();

        foreach (Door door in Enum.GetValuesAsUnderlyingType<Door>())
        {
            if (door == prizeDoor)
                Doors[door] = new DoorStatus(DoorOpenState.Unopened, DoorKnowledge.KnownWinner,
                    DoorPickedState.Unpicked, door);
            else
                Doors[door] = new DoorStatus(DoorOpenState.Unopened, DoorKnowledge.Unknown, DoorPickedState.Unpicked,
                    door);
        }


        // this.LosingDoors = new List<Door>(2);
        // foreach (var door in Enum.GetValues<Door>())
        // {
        //     if(door != PrizeDoor) LosingDoors.Add(door);
        // }
    }

    public List<Door> PlayerSelectedDoors { get; private set; } = new List<Door>(2);

    // public Door PlayerSelectedDoor { get; private set; }
    // public Door PlayerSelectedDoor_Step1 { get; private set; }
    // public Door PrizeDoor { get; private set; }
    // public List<Door> LosingDoors { get; private set; }

    private List<Door> GetDoorsNotPickedByPlayer()
    {
        return Doors.Where(kv => kv.Value.DoorPickedState != DoorPickedState.PickedByPlayer).Select(kv => kv.Key)
            .ToList();
    }

    private List<DoorStatus> GetUnopenedDoors()
    {
        return Doors.Where(kv => kv.Value.DoorOpenState == DoorOpenState.Unopened).Select(kv => kv.Value)
            .ToList();
    }

    private Door GetWinningDoor()
    {
        var d = Doors.Where(kv => kv.Value.DoorKnowledge == DoorKnowledge.KnownWinner).Select(kv => kv.Key).Single();
        return d;
    }


    public void StepOne_PlayerPickADoor(Door selectedDoor)
    {
        if (PlayerSelectedDoors.Any()) throw new Exception("Player Selected Doors already contains selections!");

        PlayerSelectedDoors.Add(selectedDoor);
        Doors[selectedDoor].DoorPickedState = DoorPickedState.PickedByPlayer;

        Console.WriteLine($"Step 1: Player has selected door => {selectedDoor.ToText()} <=");
    }

    public Door StepTwo_HostOpenLosingDoor()
    {
        var doorsNotPickedByPlayer = GetDoorsNotPickedByPlayer();
        var winningDoor = GetWinningDoor();
        var hostDoorOptions = doorsNotPickedByPlayer.Where(d => d != winningDoor).ToArray();
        var hostPickedDoor = Picker.PickADoor(hostDoorOptions);

        hostDoorStatus = Doors[hostPickedDoor];
        (hostDoorStatus.DoorPickedState, hostDoorStatus.DoorKnowledge, hostDoorStatus.DoorOpenState) =
            (DoorPickedState.PickedByHost, DoorKnowledge.KnownLoser, DoorOpenState.Opened);

        Console.WriteLine($"Step 2: Host has opened unwinning door => {hostPickedDoor.ToText()} <=");

        return hostPickedDoor;
    }

    public Door StepThree_PlayerSelectsCurrentOrOtherDoor(bool changeDoorSelection)
    {
        var unopenedDoors = GetUnopenedDoors();
        if (unopenedDoors.Count != 2) throw new Exception("Unopened doors not equal to 2.");

        var doorChangedFrom = GetDoorPickedByPlayer(unopenedDoors);
        Door doorSelectedByPlayer;

        if (changeDoorSelection)
        {
            var doorToChangeTo = unopenedDoors.Where(d => d.DoorPickedState != DoorPickedState.PickedByPlayer).Single();

            doorToChangeTo.DoorPickedState = DoorPickedState.PickedByPlayer;
            doorChangedFrom.DoorPickedState = DoorPickedState.Unpicked;

            doorSelectedByPlayer = doorToChangeTo.Door;
            PlayerSelectedDoors.Add(doorSelectedByPlayer);
            
            Console.WriteLine(
                $"Step 3: Player has opted to switch their picked door to {doorToChangeTo.Door.ToText()} => {doorChangedFrom.Door.ToText()} <=");
        }
        else
        {
            doorSelectedByPlayer = doorChangedFrom.Door;
            Console.WriteLine($"Step 3: Player has opted to not switch their picked door: => {doorChangedFrom.Door.ToText()} <=");
        }

        return doorSelectedByPlayer;
    }

 
    public bool StepFour_OpenPlayerSelectedDoor()
    {
        var playerDoor = GetDoorPickedByPlayer(Doors.Values);
        if (playerDoor == null) throw new Exception();

        if (playerDoor.DoorOpenState == DoorOpenState.Opened)
            throw new Exception("When opening player-picked door, door is already marked as open.");
        
        playerDoor.DoorOpenState = DoorOpenState.Opened;

        var didPlayerWin = (GetWinningDoor() == playerDoor.Door);
        var playerWinText = didPlayerWin ? "WINNER!" : "loser";
        
        Console.WriteLine($"Drumroll... opening the player's selected door => {playerDoor.Door.ToText()} <= ... {playerWinText}");

        return didPlayerWin;

    }
    
    private static DoorStatus GetDoorPickedByPlayer(IEnumerable<DoorStatus> unopenedDoors)
    {
        return unopenedDoors.Where(d => d.DoorPickedState == DoorPickedState.PickedByPlayer).Single();
    }
    
    
    public void OutputDoorsStatusSideBySide(
        int initialPadding,
        int spaceBetweenDoors,
        int doorWidth,
        int spaceBetweenDoorsAndStatus)
    {
        // Build the same status lines you currently print in the box
        var statusLines = new List<string>();
        statusLines.Add("Current Status:");
        foreach (Door door in Enum.GetValues<Door>())
            statusLines.Add("  " + Doors[door]);

        var text = MontyHallRender.RenderDoorsWithStatus(
            GetDoorStatusesInOrder(),
            statusLines,
            includeLabels: true,
            initialPadding: initialPadding,
            spaceBetweenDoors: spaceBetweenDoors,
            doorWidth: doorWidth,
            spaceBetweenDoorsAndStatus: spaceBetweenDoorsAndStatus
        );

        Console.WriteLine(text);
    }
    

    public void OutputDoorsStatusText()
    {
        string leftPad = new string(' ', 10);
        var lines = new List<string>();

        lines.Add("Current Status:");
        foreach (Door door in Enum.GetValues<Door>())
        {
            lines.Add("  " + Doors[door]);
        }

        // Calculate max width of all lines
        int maxWidth = lines.Max(l => l.Length);
        string top = leftPad + "┌" + new string('─', maxWidth + 2) + "┐";
        string bottom = leftPad + "└" + new string('─', maxWidth + 2) + "┘";

        Console.WriteLine(top);
        foreach (var line in lines)
        {
            Console.WriteLine(leftPad + "│ " + line.PadRight(maxWidth) + " │");
        }

        Console.WriteLine(bottom);
    }

    public void OutputDoorsStatusGraphic()
    {
        // e.g., left pad 4, doors gap 6, door width 9, doors→status gap 6
        OutputDoorsStatusSideBySide(4, 6, 9, 6);
    }

    // public void OutputDoorsStatusGraphic()
    // {
    //     var text = MontyHallRender.RenderDoors(GetDoorStatusesInOrder(), true, 15, 10, 9, 6);
    //     Console.WriteLine(text);
    // }

    private List<DoorStatus> GetDoorStatusesInOrder()
    {
        return Doors.OrderBy(kv => kv.Key).Select(kv => kv.Value).ToList();
    }
}

public enum Door
{
    Door1,
    Door2,
    Door3
}

// public enum DoorStatus
// {
//     UnopenedAndUnknown,
//     UnopenedAndWinner,
//     OpenedAndLoser,
//     OpenedAndWinner
// }

public enum DoorOpenState
{
    Unopened,
    Opened
}

public enum DoorKnowledge
{
    Unknown,
    KnownWinner,
    KnownLoser
}

public enum DoorPickedState
{
    Unpicked,
    PickedByPlayer,
    PickedByHost
}

public class DoorStatus
{
    public DoorOpenState DoorOpenState { get; set; }
    public DoorKnowledge DoorKnowledge { get; set; }
    public DoorPickedState DoorPickedState { get; set; }


    public DoorStatus(DoorOpenState doorOpenState, DoorKnowledge doorKnowledge, DoorPickedState doorPickedState,
        Door door)
    {
        DoorOpenState = doorOpenState;
        DoorKnowledge = doorKnowledge;
        DoorPickedState = doorPickedState;
        Door = door;
    }

    public Door Door { get; private set; }

    public override string ToString()
    {
        return $"=> Door: {Door.ToText()}: {DoorOpenState.ToText()} | {DoorKnowledge.ToText()} | {DoorPickedState.ToText()}";
    }
}

public static class DoorStatusExtensions
{
    public static string ToText(this Door door) => door switch
    {
        Door.Door1 => "1",
        Door.Door2 => "2",
        Door.Door3 => "3",
        _ => door.ToString()
    };
    
    public static string ToText(this DoorOpenState state) => state switch
    {
        DoorOpenState.Unopened => "Unopened",
        DoorOpenState.Opened => "Opened",
        _ => state.ToString()
    };

    public static string ToText(this DoorKnowledge knowledge) => knowledge switch
    {
        DoorKnowledge.Unknown => "?",
        DoorKnowledge.KnownWinner => "★",
        DoorKnowledge.KnownLoser => "X",
        _ => knowledge.ToString()
    };

    public static string ToText(this DoorPickedState picked) => picked switch
    {
        DoorPickedState.Unpicked => "Unpicked",
        DoorPickedState.PickedByPlayer => "Picked by Player",
        DoorPickedState.PickedByHost => "Picked by Host",
        _ => picked.ToString()
    };
}