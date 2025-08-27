// namespace Monty_Hall_Problem_Tester;
//
// public class GameRound
// {
//     private Picker _picker = new Picker();
//     
//     public GameRound()
//     {
//         
//         this.PrizeDoor = _picker.PickADoor();
//         this.LosingDoors = new List<Door>(2);
//         foreach (var door in Enum.GetValues<Door>())
//         {
//             if(door != PrizeDoor) LosingDoors.Add(door);
//         }
//     }
//     
//     
//     public Door PlayerSelectedDoor { get; private set; }
//     public Door PlayerSelectedDoor_Step1 { get; private set; }
//     public Door PrizeDoor { get; private set; }
//     public List<Door> LosingDoors { get; private set; }
//
//
//     public void StepOne_PlayerPickADoor(Door selectedDoor)
//     {
//         this.PlayerSelectedDoor = selectedDoor;
//         this.PlayerSelectedDoor_Step1 = PlayerSelectedDoor;
//         
//         Console.WriteLine($"Step 1: Player has selected door {selectedDoor}");
//     }
//     
//     public void StepTwo_HostOpenLosingDoor()
//     {
//         
//     }
// }
//
// public enum Door
// {
//     Door1,
//     Door2,
//     Door3
// }