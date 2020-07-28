using System;
using System.Collections.Generic;

// import java.util.ArrayList;

// HCurve
public class HCurve : Scurve{
    protected int dimension;
    protected int size;
    protected Utils utils;
    public Dictionary<TwoIntegerTuple, int> indexes;
    public HCurve(Cantordust cantordust) : base(cantordust) {
        this.type = "hcurve";
    }
    public HCurve(Cantordust cantordust, int dimension, double size) : base(cantordust) {
        this.type = "hcurve";
        this.utils = new Utils(this.cantordust);
        this.indexes = new Dictionary<TwoIntegerTuple, int>();
        this.cantordust.cdprint("checking HCurve size.\n");
        double x = Math.Ceiling(Math.Pow(size, 1/(double)dimension));
        double y = Math.Pow(x, dimension);
        if(!(Math.Pow(x, dimension) == size)){
            throw new ArgumentException("Size does not fit a square HCurve curve");
        }
        if(dimension != 2){
            throw new ArgumentException("Invalid dimension - we can only draw the H-curve in 2 dimensions.");
        }
        double c = Math.Log(x)/Math.Log(2);
        if(!(c == (int)c)){
            throw new ArgumentException("Invalid size - has to be a power of 2.");
        }
        this.cantordust.cdprint("HCurve check passed\n");
        this.dimension = dimension;
        this.size = (int)x;
    }
    public override int getLength(){
        return (int)Math.Pow(this.size, this.dimension);
    }
    public override TwoIntegerTuple dimensions(){
        /*
            Size of this curve in each dimension.
        */
        return new TwoIntegerTuple(this.size, this.size);
    }
    public int cor(int d, int i, int n){
        int tsize = (int)(Math.Pow(n, this.dimension)/2);
        if(i<0){
            return 0;
        } else if(i<d+1){
            return d;
        } else if(i>=tsize){
            return n - this.cor(d, i-tsize, n) - 1;
        } else {
            // Which of the four sub-triangles of this trangle will win?
            int x = 4*i/tsize;
            if(x==0){
                return this.cor(d, i, n/2);}
            if(x==1){
                return this.cor(d, tsize/2 - 1 - i, n/2);}
            if(x==2){
                return n/2 - this.cor(d, 3 * tsize/4 - 1 - i, n/2) - 1;}
            if(x==3){
                return n/2 + this.cor(d, i-3 * tsize/4, n/2);}
            return 0;
        }
    }
    public int xcor(int i, int n){
        // Size of this sub-triangle
        int tsize = (int)(Math.Pow(n, this.dimension)/2);
        if(i<0){
            return 0;
        } else if(i>=tsize){
            return n - this.xcor(i - tsize, n) - 1;
        } else {
            // Which of the four sub-triangles of this triangle are we in?
            int x = 4*i/tsize;
            if(x == 0){ return this.xcor(i, n/2);}
            if(x == 1){ return this.xcor(tsize/2 - 1 - i, n/2);}
            if(x == 2){ return n/2 - this.xcor(3 * tsize/4 - 1 - i, n/2) - 1;}
            if(x == 3){ return n/2 + this.xcor(i - 3 * tsize/4, n/2);}
        }
        return 0;
    }
    public int ycor(int i, int n){
        int tsize = (int)(Math.Pow(n, this.dimension)/2);
        if(i<2){
            return i;
        } else if(i>=tsize){
            return n - this.ycor(i - tsize, n) - 1;
        } else {
            int x = 4*i/tsize;
            if(x==0){return this.ycor(i, n/2);}
            if(x==1){return n - this.ycor(tsize/2 - 1 - i, n/2) - 1;}
            if(x==2){return n/2 + this.ycor(3 * tsize/4 - 1 - i, n/2);}
            if(x==3){return n/2 + this.ycor(i - 3 * tsize/4, n/2);}
        }
        return 0;
    }
    public override int index(TwoIntegerTuple p){
        if(!this.indexes.ContainsKey(p)){return 42;}
        int idx = this.indexes[p];
        return idx;
    }
    public override Tuple point(int idx){
        TwoIntegerTuple p = new TwoIntegerTuple(this.cor(0, idx, this.size), ycor(idx, this.size));
        this.indexes[p] = idx;
        return p;
    }
}