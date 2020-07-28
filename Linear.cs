
// Linear
using System;
using System.Collections.Generic;

public class Linear : Scurve{
    protected int dimension;
    protected int width;
    protected int height;
    protected int size;
    public Linear(Cantordust cantordust) : base(cantordust) {
        this.type = "linear";
    }
    public Linear(Cantordust cantordust, int dimension, double size) : base(cantordust) {
        this.type = "linear";
        this.cantordust.cdprint("checking zig zag size.\n");
        double x = Math.Ceiling(Math.Pow(size, 1/(double)dimension));
        double y = Math.Pow(x, dimension);
        if(!(Math.Pow(x, dimension) == size)){
            throw new ArgumentException("Size does not fit a square Linear curve");
        }
        this.dimension = dimension;
        this.size = (int)x;
        this.width = this.size;
        this.height = this.size;
    }
    public override void setWidth(int width){
        this.width = width;
    }
    public override void setHeight(int height){
        this.height = height;
    }
    public override int getWidth(){
        return this.width;
    }
    public override int getHeight(){
        return this.height;
    }
    public override int getLength(){
        return (int)Math.Pow(this.size, this.dimension);
    }
    public override TwoIntegerTuple dimensions(){
        /*
            Size of this curve in each dimension.
        */
        return new TwoIntegerTuple(this.width, this.height);
    }
    public override int index(TwoIntegerTuple p){
        int idx = 0;
        bool flip = false;
        int fi;
        List<int> arrlist = new List<int>(2);
        p.reverse();
        arrlist.Add(p.get(0));
        arrlist.Add(p.get(1));
        for(int power=0;power<2;power++){
            int i = arrlist[power];
            power = this.dimension-power-1;
            if(flip){
                fi = this.size-i-1;
            } else{
                fi = i;
            }
            int v = fi * (int)Math.Pow(this.size, power);
            idx += v;
            if(i%2==1){
                flip = !flip;
            }
        }
        return idx;
    }
    public override Tuple point(int idx){
        // cantordust.cdprint("\n----\n");
        // TwoIntegerTuple p = new TwoIntegerTuple();
        List<int> arrlist = new List<int>(2);
        bool flip = false;
        for(int i=this.dimension-1;i>-1;i-=1){
            int v = idx/(int)(Math.Pow(this.size, i));
            if(i>0){
                idx = idx - (int)(Math.Pow(this.size, i)*v);
            }
            if(flip){
                v = this.size-1-v;
            }
            arrlist.Add((int)v);
            if(v%2==1){
                flip = !flip;
            }
        }
        TwoIntegerTuple p = new TwoIntegerTuple(arrlist[0], arrlist[1]);
        p.reverse();
        // cantordust.cdprint("p: "+p.get(0)+", "+p.get(1)+"\n");
        return p;
    }
}