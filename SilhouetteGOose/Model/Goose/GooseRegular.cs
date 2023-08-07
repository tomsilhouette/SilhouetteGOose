namespace SilhouetteGOose.Model.Goose
{
    public class GooseRegular
    {
        public int GooseID { get; set; }
        public float GooseXpos { get; set; }
        public float GooseYpos { get; set; }
        public int GooseScore { get; set; } = 25;

        // Int 0 = up, 1 = left, 2 = down, 3 = right
        public int TravellingDirection { get; set; }

        public GooseRegular(int gooseCounter, float gooseStartingXpos, float gooseStartingYpos, int travellingDirection)
        {
            GooseID = gooseCounter;
            GooseXpos = gooseStartingXpos;
            GooseYpos = gooseStartingYpos;
            TravellingDirection = travellingDirection;
        }
    }
}
