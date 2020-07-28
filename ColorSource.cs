using System;
using System.Collections.Generic;
using System.Linq;


//@SuppressWarnings("unchecked")
public abstract class ColorSource {
    protected byte[] data;
    protected Cantordust cantordust;
    protected Dictionary<byte, int> symbol_map;
    protected string type;

    public ColorSource(Cantordust cantordust, byte[] data/* , block */) {
        this.cantordust = cantordust;
        this.data = data;
        this.symbol_map = new Dictionary<byte, int>();
        SortedSet<byte> sorted_uniques = new SortedSet<byte>();
        foreach (byte b in data) {
            sorted_uniques.Add(b);
        }
        byte[] listed_uniques = sorted_uniques.ToArray();
        /*
            we are ignoring unsafe casting to create the symbol_map
            the symbol_map is an array of every unique byte within the loaded program
            mapped in a HashMap to the unsigned byte value.
        */
        for (int i = 0; i < listed_uniques.Length; i++) {
            int var = (int)listed_uniques[i] & 0xff;
            this.symbol_map[listed_uniques[i]] = var;
            cantordust.cdprint(string.Format("b: {0:x2} : "+var+Environment.NewLine, listed_uniques[i]));
        }
    }

    public bool isType(String t){
        if(this.type == t){
            return true;
        } else { return false; }
    }

    public void setData(byte[] data){
        this.data = data;
    }

    public int getLength() {
        return data.Length;
    }

    public Rgb point(int x) {
        // implement blocksize
        /*
            * if self.block and (self.block[0]<=x<self.block[1]): return self.block[2]
            * else:
            */
        return getPoint(x);
    }

    public abstract  Rgb getPoint(int x);
    }