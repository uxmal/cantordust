using System;

public class ColorSpectrum : ColorSource { /* see binvis - ColorHilbert class */
    Hilbert map;
    double step;
    public ColorSpectrum(Cantordust cantordust, byte[] data) :
        base(cantordust, data) {
        this.type = "spectrum";
        this.map = new Hilbert(this.cantordust, 3, (Math.Pow(256, 3)));
        this.step = map.getLength()/(double)(symbol_map.Count);
    }

    public override Rgb getPoint(int x) {
        int c = symbol_map.get(data[x]);
        Rgb r = (Rgb)map.point((int)(c*this.step));
        return (Rgb)map.point((int)(c*this.step));
    }
}