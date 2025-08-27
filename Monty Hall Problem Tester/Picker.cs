namespace Monty_Hall_Problem_Tester;

public static class Picker
{
    private readonly static Random _rnd = new Random();

    public static Door PickADoor()
    {
        return PickADoor(Door.Door1, Door.Door2, Door.Door3);
    }

    public static Door PickADoor(params Door[] doors)
    {
        var d= doors[_rnd.Next(0, doors.Length)];
        var doorChoicesText = doors.Select(d => d.ToString());
        //Console.WriteLine($"  Picked door => {d} <= - from {string.Join(",",doorChoicesText)}");
        return d;
    }

    public static bool YesOrNo()
    {
        return _rnd.Next(0, 2) == 1;
    }
}

