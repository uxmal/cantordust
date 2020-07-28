// utils

using System;
using System.Collections.Generic;

public class Utils{
    protected Cantordust cantordust;
    protected byte[] data;
    public Utils(Cantordust cantordust){
        this.cantordust = cantordust;
        data = this.cantordust.getData();
    }
    public double entropy(byte[] data, int blocksize, int offset, int symbols) {
        int start;
        if(data.Length < blocksize){
            throw new ArgumentException("Data length must be larger than block size");
        }
        if(offset < blocksize/2){
            start = 0;
        }
        else if(offset > data.Length-blocksize/2){
            start = data.Length-blocksize/2;
        }
        else {
            start = offset-blocksize/2;
        }
        IDictionary<byte, int> hist = new Dictionary<byte, int>();
        for(int i = start; i < start+blocksize; i++){
            int count;
            if((i == data.Length)) {break;}
            if(hist.ContainsKey(data[i])){count = 0;}
            else {count = hist[data[i]];}
            hist[data[i]] = count+1;
        }
        int @base = Math.Min(blocksize, symbols);
        double entropy = 0;
        foreach (var next in hist.Values) {
            double p = next/(double)(blocksize);
            // If blocksize < 256, the number of possible byte values is restricted.
            // In that case, we adjust the log base to make sure we get a value
            // between 0 and 1.
            double log = (double)Math.Log(p)/(double)Math.Log(@base);
            entropy += (p * log);
        }
        return -entropy;
    }
    public uint graycode(uint x){
        return x^(x>>1);
    }
    public int igraycode(int x){
        if(x==0){
            return x;
        }
        int m = (int)(Math.ceil(Math.log(x)/Math.log(2)))+1;
        int i = x;
        int j = 1;
        while(j < m){
            i = i ^ (x>>>j);
            j+=1;
        }
        return i;
    }
    public int rrot(int x, int i, int width){
        /*
            Right bit-rotation.
            width: the bit width of x.
        */
        assert x < (int)(Math.Pow(2, width));
        i = i%width;
        x = (x>>>i) | (x<<width-i);
        return x&(int)(Math.Pow(2, width)-1);
    }
    public int lrot(int x, int i, int width){
        /*
            Left bit-rotation.
            width: the bit width of x.
        */
        assert x < Math.Pow(2, width);
        i = i%width;
        x = (x<<i) | (x>>>width-i);
        return x&((int)Math.Pow(2, width)-1);
    }
    public int tsb(int x, int width){
        /*
            Tailing set bits
        */
        assert x < (int)Math.Pow(2, width);
        int i = 0;
        while((x&1)==1 && i<=width){
            x = x >>> 1;
            i+=1;
        }
        return i;
    }
    public int setbit(int x, int w, int i, int b){
        /*
            Sets bit i in an integer x of width w to b.
            b must be 1 or 0
        */
        assert b==1 || b==0;
        assert i<w;
        if(b==1){
            return x | (int)Math.Pow(2, w-i-1);
        } else {
            return x & ~(int)Math.Pow(2, w-i-1);
        }
    }
    public int bitrange(int x, int width, int start, int end){
        /*
            Extract a bit range as an integer.
            (start, end) is inclusive lower bound, exclusive upper bound.
        */
        return x >>> (width-end) & (int)(Math.Pow(2, end-start)-1);
    }
}