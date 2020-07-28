public class ColorGradient : ColorSource {
    public ColorGradient(Cantordust cantordust, byte[] data) :
        base(cantordust, data) {
        this.type = "gradient";
    }

    public override Rgb getPoint(int x) {
        double c = (int)(data[x])/255.0;
        return new Rgb(
            (int)(255*c), 
            (int)(255*c), 
            (int)(255*c));
    }
}