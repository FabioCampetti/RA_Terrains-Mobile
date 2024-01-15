[System.Serializable]
public class ElevationResult {
    public double resolution;
    public double elevation;
    public Location location;

public ElevationResult(double elevation, Location location) {
    this.elevation = elevation;
    this.location = location;
    resolution = 0.0;
}
    public string ToString() {
        return location.ToString() + "," + elevation;
    }
}
