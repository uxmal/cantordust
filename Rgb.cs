using System;
using System.Diagnostics;

public class Rgb : Tuple {
    int r;
    int g;
    int b;
    public Rgb(int r, int g, int b) {
        Debug.Assert(r <= 255 && g <= 255 && b <= 255);
        Debug.Assert (r >= 0 && g >= 0 && b >= 0);
        this.r =r; this.g = g; this.b = b;
    }
    public int this[int idx] { get {
        if(idx == 0){return this.r;}
        else if(idx == 1){return this.g;}
        else if(idx == 2){return this.b;}
        else {throw new ArgumentException("Index Error for Rgb: "+idx);}
    } 
    set {
            Debug.Assert(idx >= 0 && idx <= 255);
        if(idx == 0){this.r = value;}
        else if(idx == 1){this.g = value;}
        else if(idx == 2){this.b = value;}
        else {throw new ArgumentException("Index Error for Rgb: "+idx);}
    } }
    public override bool Equals(Object o) {
        Rgb _o = (Rgb)o;
        return r == _o.r && g == _o.g && b == _o.b;
    }
}