using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

public class ByteCloudVisualizer : Visualizer {

    public ByteCloudVisualizer(int windowSize, Cantordust cantordust) :
        base(windowSize, cantordust) {
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        // JOptionPane.showMessageDialog(null, String.format("length = %d", mainInterface.getData().length), "InfoBox: " + "test", JOptionPane.INFORMATION_MESSAGE);
        base.OnPaint(pe);
        dataMicroSlider.Minimum = dataMacroSlider.Value;
        dataMicroSlider.Maximum = dataMacroSlider.getUpperValue();
        int low = dataMicroSlider.getValue();
        int high = dataMicroSlider.getUpperValue();
        byteCloud(pe.Graphics, low, high);
        dataMacroSlider.ValueChanged += (sender, e) => {
            RangeSlider slider = (RangeSlider)sender;
            int rlow = slider.Value;
            int rhigh = slider.getUpperValue();
            byteCloud(pe.Graphics, rlow, rhigh);
            Invalidate();
        };
        dataMicroSlider.ValueChanged += (sender, e) => {
            RangeSlider slider = (RangeSlider)sender;
            int rlow = slider.getValue();
            int rhigh = slider.getUpperValue();
            byteCloud(pe.Graphics, rlow, rhigh);
            Invalidate();
        };
    }

    private void byteCloud(Graphics g, int low, int high) {
        byte[] data = mainInterface.getData();
        int distance;
        Dictionary<byte, int> bytes = new Dictionary<byte, int>();
        int freq;
        int i = 0;
        int j = 0;
        float fontSize = 18.0f;
        float alpha = 1.0f;
        byte b;
        int maxFreq = 0;
        //Integer[] freqs;

        //initialize map so every byte is there with a frequency of 0
        for(i = 0; i < 256; i++){
            b = (byte) i;
            bytes[b] = 0;
        }
        
        distance = low + 9999;
        if(low + 9999 > high){
            distance = high;
        }

        //Set the frequency for each byte within the specified area
        b = (byte)255;
        for(int byteIdx=low; byteIdx <= distance; byteIdx++) {
            if(data[byteIdx] != b && data[byteIdx] != 0x00){
                freq = bytes[data[byteIdx]];
                bytes[data[byteIdx]] = freq + 1;
                if(freq+1 > maxFreq){
                    maxFreq = freq+1;
                }
            }
        }

        //Set font details
        Font f = new Font("Courier New", 12, FontStyle.Bold);
        g.TextRenderingHint = TextRenderingHint.AntiAlias;

        for(i = 0; i < 16; i++){
            for(j = 0; j < 16; j++){
                int textByte = i * 16 + j;
                float frequency = (float) bytes[(byte)textByte]/maxFreq;
                fontSize *= frequency;
                string s = string.Format("{0:X2}", textByte);

                //set transparity
                alpha *= frequency;
                if(alpha >= 1.0f){
                    alpha = 1.0f;
                }
                if(alpha <= 0.0f){
                    alpha = 0.1f;
                }

                //$TODO
                //try {
                //    g.al.co.setComposite(AlphaComposite.getInstance(AlphaComposite.SRC_OVER, alpha));
                //} catch (IllegalArgumentException e) {
                //    g.setComposite(AlphaComposite.getInstance(AlphaComposite.SRC, 0));
                //}
                if(fontSize < 7.0f){
                    fontSize = 0;
                }
                var ff = new Font(f.FontFamily, fontSize);
                g.DrawString(s, ff, Brushes.Green, 20 * (j+1),20 * (i+1));
                ff.Dispose();
                fontSize = 18;
                alpha = 1.0f;
            }
        }
        f.Dispose();
        // String z = String.format("%d", maxFreq);
        // g.drawString(z, 720, 720);
    }

    public static int getWindowSize() {
        return 360;
    }
}