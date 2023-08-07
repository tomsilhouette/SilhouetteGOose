namespace SilhouetteGOose.Model.GameState
{
    public class GameState
    {
        // Game Activity
        public bool IsPlayer { get; set; } = false;
        public bool GameOver { get; set; } = true;

        // Game Setup
        public int LevelNumberOfGeese { get; set; } = 20;
        public float GooseMovementSpeed { get; set; } = 10;
    }
}
