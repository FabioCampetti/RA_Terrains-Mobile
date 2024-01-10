public class TerrainElevationData {
    public double highestElevation { get; set; }
    public double lowestElevation { get; set; }
    public ElevationResult[,] elevationResults { get; set; }

    public TerrainElevationData(double highestElevation, double lowestElevation, ElevationResult[,] elevationResults) {
        this.highestElevation = highestElevation;
        this.lowestElevation = lowestElevation;
        this.elevationResults = elevationResults;
    }
}
