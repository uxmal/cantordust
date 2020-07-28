using System.Drawing;

import java.awt.*;
import java.util.HashMap;

public class ColorClassifierPrediction : ColorSource {
    Hilbert map;
    double step;

    private ClassifierModel classifier;

    public ColorClassifierPrediction(Cantordust cantordust, byte[] data) : base(cantordust, data) {
        this.type = "classifierPrediction";
        classifier = cantordust.getClassifier();
    }

    public override Rgb getPoint(int x) {
        int unsignedByte = data[x] & 0xFF;
        Color r = Color.FromArgb(0, unsignedByte, 0);
        Rgb rgb = new Rgb(r.R, r.G, r.B);
        ClassifierModel classifier = cantordust.getClassifier();
        int classification = classifier.classAtIndex(x);
        double c = (double)classification / (double)ClassifierModel.classes.Length;
        double waveLength = 400 + c*(800-400);
        Color color = WavelengthToRGB.waveLengthToRGB(waveLength);
        return new Rgb(color.R, color.G, color.B);
        //return new Rgb((int)Math.floor(c*255.0), (int)Math.floor(c*255.0), (int)Math.floor(c*255.0));
    }
}
