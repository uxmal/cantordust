using System.Drawing;

import java.awt.Color;
public class Color8bpp : ColorSource { /* see binvis - ColorHilbert class */
    Hilbert map;
    double step;
    public Color8bpp(Cantordust cantordust, byte[] data) {
        super(cantordust, data);
        this.type = "8bpp";
    }

    public override Rgb getPoint(int x) {
        int unsignedByte = data[x] & 0xFF;
        Color r = Color.FromArgb(0, unsignedByte, 0);
        Rgb rgb = new Rgb(r.getRed(), r.getGreen(), r.getBlue());
        return rgb;
    }
}