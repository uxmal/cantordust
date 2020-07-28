using System.Drawing;

public class BitMapColorMapper : ColorMapper {

    public BitMapColorMapper(Cantordust cantordust) : base(cantordust) {
        data = cantordust.getData();
    }

    public override Color colorAtIndex(int index) {
        int unsignedByte = data[index] & 0xFF;
        return Color.FromArgb(0, unsignedByte, 0);
    }
}
