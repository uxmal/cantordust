//scurve __init__
using System;

public class Scurve{
    protected Cantordust cantordust;
    protected int size = 512;
    protected byte[] data;
    protected string type;
    // private HashMap<String,Scurve> curveMap = new HashMap<String, Scurve>();
    public Scurve(Cantordust cantordust){
        this.cantordust = cantordust;
        data = cantordust.getData();
        // curveMap.put("hcurve", new Hcurve());
        // curveMap.put("zigzag", new ZigZag());
        // curveMap.put("zorder", new ZOrder());
        // curveMap.put("natural", new Natural());
        // curveMap.put("gray", new GrayCurve());
        // curveMap.put("hilbert", new Hilbert(cantordust));
    }
    public virtual void setWidth(int width){}
    public virtual void setHeight(int height){}
    public virtual int getWidth(){return this.size;}
    public virtual int getHeight(){return this.size;}
    public bool isType(string t){
        if(this.type == t){
            return true;
        } else { return false; }
    }
    public virtual Scurve fromSize(string curve, int dimension, int size){
        return new Hilbert(this.cantordust, dimension, size);
    }
    public virtual int getLength(){
        return 0;
    }
    public virtual Tuple point(int idx){
        if(idx >= getLength()){
            throw new ArgumentException("Index Error");
        }
        return new TwoIntegerTuple(0, 0);
    }
    public virtual int index(TwoIntegerTuple p){
        return 0;
    }
    public virtual TwoIntegerTuple dimensions(){
        return new TwoIntegerTuple(0, 0);
    }
}