using System.Drawing;

public abstract class ColorMapper {
    protected Cantordust cantordust;
    protected byte[] data;

    public ColorMapper(Cantordust cantordust) {
        this.cantordust = cantordust;
        this.data = cantordust.getData();
    }

    public abstract Color colorAtIndex(int index);
}
