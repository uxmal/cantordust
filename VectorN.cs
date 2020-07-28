/* byte vector of fixed length */

using System;
using System.Diagnostics;
using System.Text;

public class VectorN {
    private byte[] v;
    private int n;

    public VectorN(byte[] data) {
        this.n = data.Length;
        v = new byte[data.Length];
        Array.Copy(data, 0, v, 0, data.Length);
    }

    public VectorN(int n) {
        this.n = n;
        v = new byte[n];
    }

    public override int GetHashCode() {
        int hash = n;
        foreach (byte b in v) {
            hash = (hash << 5) ^ (hash >> 27) ^ b;
        }
        return hash;
    }

    public override bool Equals(object obj) {
        if (!(obj is VectorN right))
            return false;
        if(n == right.n) {
            for(int i = 0; i < n; i++) {
                if(v[i] != right.v[i]) {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public byte[] toArray() {
        byte[] ret = new byte[n];
        Array.Copy(v, 0, ret, 0, n);
        return ret;
    }

    public byte getAt(int index) {
        Debug.Assert(index >= 0 && index < n);
        return v[index];
    }

    public void setAt(int index, byte val) {
        Debug.Assert(index >= 0 && index < n);
        v[index] = val;
    }

    public int getN() {return n;}

    public override string ToString() {
        StringBuilder builder = new StringBuilder();
        builder.Append("< ");
        for(int i = 0; i < v.Length; i++) {
            builder.AppendFormat("{0:x} ", v[i]);
        }
        builder.Append(">");
        return builder.ToString();
    }
}
