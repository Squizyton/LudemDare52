using System;

namespace SaveFile
{
    [Serializable]
    public class FarmerInfo
    {
        public string farmerName;
        public float totalTimeAlive;
        public int totalCropsHarvested;
        public int totalCropsPlanted;
        public int bulletsShot;
    
        public string didTutorial;
    }
}
