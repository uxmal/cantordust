public class ThreeByteTuple {
    byte x;
    byte y;
    byte z;
    public ThreeByteTuple(byte x, byte y, byte z) {
        this.x = x; this.y = y; this.z = z;
    }

    public override bool Equals(object o) {
        ThreeByteTuple _o = (ThreeByteTuple)o;
        return x == _o.x && y == _o.y && z == _o.z;
    }
    
    public override int GetHashCode() {
        int xy = ((x+y)*(x+y+1))/2 + y;
        return ((xy + z)*(xy+z+1))/2 + z;
    }
}
