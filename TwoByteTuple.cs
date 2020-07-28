public class TwoByteTuple {
    byte x;
    byte y;
    public TwoByteTuple(byte x, byte y) {this.x = x; this.y = y;}
    public override bool Equals(object o) {
        TwoByteTuple _o = (TwoByteTuple)o;
        return x == _o.x && y == _o.y;
    }
    public override int GetHashCode() {
        return ((x+y)*(x+y+1))/2 + y;
    }
}
