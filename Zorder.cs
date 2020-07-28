
// Zorder
using System;
using System.Collections.Generic;

public class Zorder : Scurve{
    protected int dimension;
    protected int bits;
    protected Utils utils;
    public Zorder(Cantordust cantordust) :base(cantordust) {
        this.type = "zorder";
    }
    public Zorder(Cantordust cantordust, int dimension, double size) :
        base(cantordust)
    {
        this.type = "zorder";
        this.utils = new Utils(this.cantordust);
        this.cantordust.cdprint("checking zorder size.\n");
        double x = Math.Log(size)/Math.Log(2);
        double bits = x/dimension;
        if(!(bits == (int)bits)){
            throw new ArgumentException("Size does not fit a square Zorder curve");
        }
        this.dimension = dimension;
        this.bits = (int)bits;
    }
    public override int getLength(){
        return (int)Math.Pow(2, this.bits*this.dimension);
    }
    public override TwoIntegerTuple dimensions(){
        /*
            Size of this curve in each dimension.
        */
        int x = (int)Math.Pow(2, this.bits);
        return new TwoIntegerTuple(x, x);
    }
    public override int index(TwoIntegerTuple p){

        int idx = 0;
        List<int> arrlist = new List<int>(2);
        p.reverse();
        arrlist.Add(p.get(0));
        arrlist.Add(p.get(1));
        int iwidth = this.bits*this.dimension;
        for(int i=0;i<iwidth;i++){
            int bitoff = this.bits-(i/this.dimension)-1;
            int poff = this.dimension-(i%this.dimension)-1;
            int b = this.utils.bitrange(arrlist[poff], this.bits, bitoff, bitoff+1) << i;
            idx |= b;
        }
        return idx;
    }
    public override Tuple point(int idx){
        // cantordust.cdprint("\n----\n");
        // TwoIntegerTuple p = new TwoIntegerTuple();
        List<int> arrlist = new List<int>(2);
        arrlist.Add(0);
        arrlist.Add(0);
        int iwidth = this.bits*this.dimension;
        for(int i=0;i<iwidth;i++){
            int b = this.utils.bitrange(idx, iwidth, i, i+1) << (iwidth-i-1)/this.dimension;
            int x = arrlist[i%this.dimension];
            arrlist[i%this.dimension] = x |= b;
        }
        TwoIntegerTuple p = new TwoIntegerTuple(arrlist[0], arrlist[1]);
        p.reverse();
        // cantordust.cdprint("p: "+p.get(0)+", "+p.get(1)+"\n");
        return p;
    }
}